using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Consolonia.Core.Infrastructure;

namespace Consolonia.Core.Drawing
{
    internal class RenderTarget : IDrawingContextLayerImpl
    {
        private readonly IConsole _console;

        private readonly ConsoleWindow _consoleWindow;

        private PixelBuffer _bufferBuffer;

        private (Color background, Color foreground, FontWeight weight, FontStyle style, TextDecorationCollection
            textDecorations, char character)?[,] _cache;

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

        public void Save(string fileName, int? quality = null)
        {
            throw new NotImplementedException();
        }

        public void Save(Stream stream, int? quality = null)
        {
            throw new NotImplementedException();
        }

        public Vector Dpi { get; } = Vector.One;
        public PixelSize PixelSize { get; } = new(1, 1);
        public int Version => 0;

        void IDrawingContextLayerImpl.Blit(IDrawingContextImpl context)
        {
            try
            {
                RenderToDevice();
            }
            catch (InvalidDrawingContextException)
            {
            }
        }

        bool IDrawingContextLayerImpl.CanBlit => true;

        public IDrawingContextImpl CreateDrawingContext()
        {
            return new DrawingContextImpl(_consoleWindow, _bufferBuffer);
        }

        public bool IsCorrupted => false;


        private void OnResized(Size size, WindowResizeReason reason)
        {
            // todo: should we check the reason?
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
            _cache =
                new (Color background, Color foreground, FontWeight weight, FontStyle style, TextDecorationCollection
                    textDecorations, char character)?[width, height];
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

                /* todo: There is not IWindowImpl.Invalidate anymore.
                 if (!_consoleWindow.InvalidatedRects.Any(rect =>
                    rect.ContainsExclusive(new Point(x, y)))) continue;*/
                if (pixel.Background.Mode != PixelBackgroundMode.Colored)
                    throw new InvalidOperationException(
                        "All pixels in the buffer must have exact console color before rendering");

                if (pixel.Foreground.Symbol is null) // not using 'when' as it swallows the exceptions 
                    // buffer re-initialized after resizing
                    pixel = new Pixel(new PixelForeground(new SimpleSymbol('░')),
                        new PixelBackground(PixelBackgroundMode.Colored));

                (Color, Color, FontWeight Weight, FontStyle Style, TextDecorationCollection TextDecorations, char)
                    pixelSpread = (pixel.Background.Color, pixel.Foreground.Color, pixel.Foreground.Weight,
                        pixel.Foreground.Style, pixel.Foreground.TextDecorations,
                        pixel.Foreground.Symbol.GetCharacter());
                //todo: indexOutOfRange during resize
                if (_cache[x, y] == pixelSpread)
                    continue;

                _cache[x, y] = pixelSpread;

                flushingBuffer.WritePixel(new PixelBufferCoordinate(x, y), pixel);
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
        }

        private struct FlushingBuffer
        {
            //todo: move class out
            private readonly IConsole _console;
            private readonly StringBuilder _stringBuilder;
            private Color _lastBackgroundColor;
            private Color _lastForegroundColor;
            private FontStyle _lastStyle = FontStyle.Normal;
            private FontWeight _lastWeight = FontWeight.Normal;
            private TextDecorationCollection _lastTextDecorations = new();
            private PixelBufferCoordinate _currentBufferPoint;
            private PixelBufferCoordinate _lastBufferPointStart;

            public FlushingBuffer(IConsole console)
            {
                this = new FlushingBuffer();
                _console = console;
                _stringBuilder = new StringBuilder();
            }

            public void WritePixel(
                PixelBufferCoordinate bufferPoint,
                Pixel pixel)
            {
                if (!bufferPoint.Equals(_currentBufferPoint) /*todo: performance*/ ||
                    _lastForegroundColor != pixel.Foreground.Color ||
                    _lastBackgroundColor != pixel.Background.Color ||
                    _lastWeight != pixel.Foreground.Weight ||
                    _lastStyle != pixel.Foreground.Style ||
                    _lastTextDecorations != pixel.Foreground.TextDecorations)
                    Flush();

                if (_stringBuilder.Length == 0)
                {
                    _lastBackgroundColor = pixel.Background.Color;
                    _lastForegroundColor = pixel.Foreground.Color;
                    _lastStyle = pixel.Foreground.Style;
                    _lastWeight = pixel.Foreground.Weight;
                    _lastTextDecorations = pixel.Foreground.TextDecorations;
                    _lastBufferPointStart = _currentBufferPoint = bufferPoint;
                }

                char character = pixel.Foreground.Symbol.GetCharacter();
                if (char.IsControl(character) /*|| character is '保' or '哥'*/)
                    character = ' '; // some terminals do not print \0
                _stringBuilder.Append(character);
                _currentBufferPoint = _currentBufferPoint.WithXpp();
            }

            public void Flush()
            {
                if (_stringBuilder.Length == 0)
                    return;

                _console.Print(_lastBufferPointStart, _lastBackgroundColor, _lastForegroundColor, _lastStyle,
                    _lastWeight, _lastTextDecorations, _stringBuilder.ToString());
                _stringBuilder.Clear();
            }
        }
    }
}