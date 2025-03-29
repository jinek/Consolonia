#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;
using Consolonia.Controls;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Consolonia.Core.Infrastructure;

namespace Consolonia.Core.Drawing
{
    internal class RenderTarget : IDrawingContextLayerImpl
    {
        private readonly IConsoleOutput _console;

        private readonly ConsoleWindowImpl _consoleTopLevelImpl;

        // cache of pixels written so we can ignore them if unchanged.
        private Pixel?[,] _cache;

        internal RenderTarget(ConsoleWindowImpl consoleTopLevelImpl)
        {
            _console = AvaloniaLocator.Current.GetService<IConsoleOutput>()!;
            _consoleTopLevelImpl = consoleTopLevelImpl;
            _consoleTopLevelImpl.Resized += OnResized;

            _cache = InitializeCache(_consoleTopLevelImpl.PixelBuffer.Width, _consoleTopLevelImpl.PixelBuffer.Height);
        }

        public RenderTarget(IEnumerable<object> surfaces)
            : this(surfaces.OfType<ConsoleWindowImpl>()
                .Single())
        {
        }

        public PixelBuffer Buffer => _consoleTopLevelImpl.PixelBuffer;

        public void Dispose()
        {
            _consoleTopLevelImpl.Resized -= OnResized;
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
                lock (_consoleTopLevelImpl.PixelBuffer)
                {
                    RenderToDevice();
                }
            }
            catch (InvalidDrawingContextException)
            {
            }
        }

        bool IDrawingContextLayerImpl.CanBlit => true;

        public bool IsCorrupted => false;

        public IDrawingContextImpl CreateDrawingContext(bool useScaledDrawing)
        {
            if (useScaledDrawing)
                throw new NotImplementedException("Consolonia doesn't support useScaledDrawing");
            return new DrawingContextImpl(_consoleTopLevelImpl);
        }


        private void OnResized(Size size, WindowResizeReason reason)
        {
            // todo: should we check the reason?
            _cache = InitializeCache(_consoleTopLevelImpl.PixelBuffer.Width, _consoleTopLevelImpl.PixelBuffer.Height);
        }

        private static Pixel?[,] InitializeCache(ushort width, ushort height)
        {
            var cache = new Pixel?[width, height];

            // initialize the cache with Pixel.Empty as it literally means nothing
            for (ushort y = 0; y < height; y++)
            for (ushort x = 0; x < width; x++)
                cache[x, y] = Pixel.Empty;

            return cache;
        }

        private void RenderToDevice()
        {
            PixelBuffer pixelBuffer = _consoleTopLevelImpl.PixelBuffer;

            _console.HideCaret();

            PixelBufferCoordinate? caretPosition = null;

            var flushingBuffer = new FlushingBuffer(_console);

            for (ushort y = 0; y < pixelBuffer.Height; y++)
            for (ushort x = 0; x < pixelBuffer.Width;)
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

                //todo: indexOutOfRange during resize
                if (_cache[x, y] == pixel)
                {
                    x++;
                    continue;
                }

                _cache[x, y] = pixel;

                flushingBuffer.WritePixel(new PixelBufferCoordinate(x, y), pixel);

                x++;
            }

            flushingBuffer.Flush();

            CaretStyle pixelBufferCaretStyle = pixelBuffer.CaretStyle;

            if (caretPosition != null && pixelBufferCaretStyle != CaretStyle.None)
            {
                _console.SetCaretPosition((PixelBufferCoordinate)caretPosition);
                _console.SetCaretStyle(pixelBufferCaretStyle);
                _console.ShowCaret();
            }
            else
            {
                _console.HideCaret();
            }
        }

        private struct FlushingBuffer
        {
            //todo: move class out
            private readonly IConsoleOutput _console;
            private readonly StringBuilder _stringBuilder;
            private Color _lastBackgroundColor;
            private Color _lastForegroundColor;
            private FontStyle? _lastStyle;
            private FontWeight? _lastWeight;
            private TextDecorationLocation? _lastTextDecoration;
            private PixelBufferCoordinate _currentBufferPoint;
            private PixelBufferCoordinate _lastBufferPointStart;

            public FlushingBuffer(IConsoleOutput console)
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
                    _lastTextDecoration != pixel.Foreground.TextDecoration)
                    Flush();

                if (_stringBuilder.Length == 0)
                {
                    _lastBackgroundColor = pixel.Background.Color;
                    _lastForegroundColor = pixel.Foreground.Color;
                    _lastStyle = pixel.Foreground.Style;
                    _lastWeight = pixel.Foreground.Weight;
                    _lastTextDecoration = pixel.Foreground.TextDecoration;
                    _lastBufferPointStart = _currentBufferPoint = bufferPoint;
                }

                // the only pixels without width are Empty pixels, which we don't 
                // want to output as they are already invisible and represented
                // by the complex glyph coming before it (aka double-wide chars)
                if (pixel.Foreground.Symbol.Width > 0)
                    _stringBuilder.Append(pixel.Foreground.Symbol.Text);

                _currentBufferPoint = _currentBufferPoint.WithXpp();
            }

            public void Flush()
            {
                if (_stringBuilder.Length == 0)
                    return;

                _console.Print(_lastBufferPointStart, _lastBackgroundColor, _lastForegroundColor, _lastStyle,
                    _lastWeight, _lastTextDecoration, _stringBuilder.ToString());
                _stringBuilder.Clear();
            }
        }
    }
}