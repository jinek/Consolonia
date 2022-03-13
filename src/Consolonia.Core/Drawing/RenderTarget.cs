using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Avalonia;
using Avalonia.Platform;
using Avalonia.Rendering;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Consolonia.Core.Infrastructure;

namespace Consolonia.Core.Drawing
{
    internal class RenderTarget : IDrawingContextLayerImpl
    {
        private readonly IConsole _console;

        private readonly ConsoleWindow _consoleWindow;

        private PixelBuffer _bufferBuffer;

        private (ConsoleColor background, ConsoleColor foreground, char character)?[,] _cache;

        internal RenderTarget(ConsoleWindow consoleWindow)
        {
            _console = AvaloniaLocator.Current.GetService<IConsole>()!;
            _consoleWindow = consoleWindow;
            consoleWindow.Resized += OnResized;
            InitializeBuffer(_consoleWindow.ClientSize);
        }

        public RenderTarget(IEnumerable<object> surfaces)
            : this(surfaces.OfType<ConsoleWindow>()
                .Single())
        {
        }

        public void Dispose()
        {
            _consoleWindow.Resized -= OnResized;
        }

        public IDrawingContextImpl CreateDrawingContext(IVisualBrushRenderer visualBrushRenderer)
        {
            return new DrawingContextImpl(_consoleWindow, visualBrushRenderer, _bufferBuffer);
        }

        public void Save(string fileName)
        {
            throw new NotImplementedException();
        }

        public void Save(Stream stream)
        {
            throw new NotImplementedException();
        }

        public Vector Dpi { get; } = Vector.One;
        public PixelSize PixelSize { get; } = new(1, 1);
        public int Version => 0;

        public void Blit(IDrawingContextImpl context)
        {
            try
            {
                RenderToDevice();
            }
            catch (InvalidDrawingContextException)
            {
            }
        }

        public bool CanBlit => true;

        private void OnResized(Size size, PlatformResizeReason platformResizeReason)
        {
            InitializeBuffer(size);
        }

        private void InitializeBuffer(Size size)
        {
            ushort width = (ushort)size.Width;
            ushort height = (ushort)size.Height;

            _bufferBuffer =
                new PixelBuffer(width, height);

            InitializeCache(width, height);
        }

        private void InitializeCache(ushort width, ushort height)
        {
            _cache = new (ConsoleColor background, ConsoleColor foreground, char character)?[width, height];
        }

        private void RenderToDevice()
        {
            PixelBuffer pixelBuffer = _bufferBuffer;

            _console.CaretVisible = false;
            PixelBufferCoordinate? caretPosition = null;

            var flushingBuffer = new FlushingBuffer(_console);

            for (ushort y = 0; y < pixelBuffer.Height; y++)
            for (ushort x = 0; x < pixelBuffer.Width; x++)
            {
                Pixel pixel = pixelBuffer[(PixelBufferCoordinate)(x, y)];

                if (pixel.IsCaret)
                {
                    if (caretPosition != null)
                        throw new InvalidOperationException("Caret is already shown");
                    caretPosition = new PixelBufferCoordinate(x, y);
                }

                if (!_consoleWindow.InvalidatedRects.Any(rect =>
                    rect.ContainsAligned(new Point(x, y)))) continue;
                if (pixel.Background.Mode != PixelBackgroundMode.Colored)
                    throw new InvalidOperationException(
                        "Buffer has not been rendered. All operations over buffer must finished with the buffer to be not transparent");

                if (x == pixelBuffer.Width - 1 && y == pixelBuffer.Height - 1)
                    break;

                char character = default;
                try
                {
                    character = pixel.Foreground.Symbol.GetCharacter();
                }
                catch (NullReferenceException)
                {
                    //todo: need to break current drawing
                    if (pixel.Foreground.Symbol is null) // not using 'when' as it swallows the exceptions 
                    {
                        // buffer re-initialized after resizing
                        character = '░';
                        pixel = new Pixel(new PixelForeground(), new PixelBackground(PixelBackgroundMode.Colored));
                    }
                }

                if (character is char.MinValue or '\r' or '\n')
                    character = ' '; // some terminals does not print \0

                ConsoleColor backgroundColor = pixel.Background.Color;
                ConsoleColor foregroundColor = pixel.Foreground.Color;

                //todo: indexOutOfRange during resize
                if (_cache[x, y] == (backgroundColor, foregroundColor, character))
                    continue;

                _cache[x, y] = (backgroundColor, foregroundColor, character);

                flushingBuffer.WriteCharacter(new PixelBufferCoordinate(x, y),
                    backgroundColor,
                    foregroundColor,
                    character);
            }

            flushingBuffer.Flush();

            if (caretPosition != null)
            {
                _console.SetCaretPosition((PixelBufferCoordinate)caretPosition);
                _console.CaretVisible = true;
            }
            else
            {
                _console.CaretVisible = false;
            }

            _consoleWindow.InvalidatedRects.Clear();
        }

        private struct FlushingBuffer
        {
            //todo: move class out
            private readonly IConsole _console;
            private readonly StringBuilder _stringBuilder;
            private ConsoleColor _lastBackgroundColor;
            private ConsoleColor _lastForegroundColor;
            private PixelBufferCoordinate _currentBufferPoint;
            private PixelBufferCoordinate _lastBufferPointStart;

            public FlushingBuffer(IConsole console)
            {
                this = new FlushingBuffer();
                _console = console;
                _stringBuilder = new StringBuilder();
            }

            public void WriteCharacter(
                PixelBufferCoordinate bufferPoint,
                ConsoleColor backgroundColor,
                ConsoleColor foregroundColor,
                char character)
            {
                if (!bufferPoint.Equals(_currentBufferPoint) /*todo: performance*/ ||
                    _lastForegroundColor != foregroundColor ||
                    _lastBackgroundColor != backgroundColor)
                    Flush();

                if (_stringBuilder.Length == 0)
                {
                    _lastBackgroundColor = backgroundColor;
                    _lastForegroundColor = foregroundColor;
                    _lastBufferPointStart = _currentBufferPoint = bufferPoint;
                }

                _stringBuilder.Append(character);
                _currentBufferPoint = _currentBufferPoint.WithXpp();
            }

            public void Flush()
            {
                if (_stringBuilder.Length == 0)
                    return;

                _console.Print(_lastBufferPointStart, _lastBackgroundColor, _lastForegroundColor,
                    _stringBuilder.ToString());
                _stringBuilder.Clear();
            }
        }
    }
}