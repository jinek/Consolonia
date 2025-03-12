using System;
using System.Text;
using Avalonia.Media;
using Consolonia.Core.Drawing.PixelBufferImplementation;

namespace Consolonia.Core.Infrastructure
{
    internal class RenderTargetWindows
    {
        private IConsole _console;
        private PixelBuffer _pixelBuffer;
        private Pixel?[,] _cache;

        public RenderTargetWindows(IConsole console)
        {
            _console = console;
            _cache = InitializeCache(console.Size.Width, console.Size.Height);
            _pixelBuffer = new PixelBuffer(console.Size.Width, console.Size.Height);

            console.Resized += () =>
            {
                lock (_console)
                {
                    _cache = InitializeCache(console.Size.Width, console.Size.Height);
                    _pixelBuffer = new PixelBuffer(console.Size.Width, console.Size.Height);
                }
            };
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

        internal void RenderWindows()
        {
            lock (_console)
            {

                foreach (var windowImpl in ConsoloniaPlatform.Windows)
                {
                    //  System.Diagnostics.Debug.WriteLine(windowImpl.PixelBuffer.Dump());

                    for (ushort y = 0; y < windowImpl.PixelBuffer.Size.Height; y++)
                        for (ushort x = 0; x < windowImpl.PixelBuffer.Size.Width; x++)
                            _pixelBuffer.Set((PixelBufferCoordinate)(x, y), (pixel) => pixel.Blend(windowImpl.PixelBuffer[(PixelBufferCoordinate)(x, y)]));
                }
                //System.Diagnostics.Debug.WriteLine(_pixelBuffer.Dump());

                _console.HideCaret();

                PixelBufferCoordinate? caretPosition = null;

                var flushingBuffer = new FlushingBuffer(_console);

                for (ushort y = 0; y < _pixelBuffer.Height; y++)
                    for (ushort x = 0; x < _pixelBuffer.Width;)
                    {
                        Pixel pixel = _pixelBuffer[(PixelBufferCoordinate)(x, y)];

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

                if (caretPosition != null)
                {
                    _console.SetCaretPosition((PixelBufferCoordinate)caretPosition);
                    _console.SetCaretStyle(_pixelBuffer.CaretStyle);
                    _console.ShowCaret();
                }
                else
                {
                    _console.HideCaret();
                }
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