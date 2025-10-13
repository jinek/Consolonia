#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
        private ConsoleCursor _consoleCursor;

        internal RenderTarget(ConsoleWindowImpl consoleTopLevelImpl)
        {
            _console = AvaloniaLocator.Current.GetService<IConsoleOutput>()!;
            _consoleTopLevelImpl = consoleTopLevelImpl;
            _cache = InitializeCache(_consoleTopLevelImpl.PixelBuffer.Width, _consoleTopLevelImpl.PixelBuffer.Height);
            _consoleTopLevelImpl.Resized += OnResized;
            _consoleTopLevelImpl.CursorChanged += OnCursorChanged;
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
            _consoleTopLevelImpl.CursorChanged -= OnCursorChanged;
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

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void RenderToDevice()
        {
            PixelBuffer pixelBuffer = _consoleTopLevelImpl.PixelBuffer;
            Snapshot dirtyRegions = _consoleTopLevelImpl.DirtyRegions.GetSnapshotAndClear();

            _console.HideCaret();

            PixelBufferCoordinate? caretPosition = null;
            CaretStyle? caretStyle = null;

            var flushingBuffer = new FlushingBuffer(_console);
            for (ushort y = 0; y < pixelBuffer.Height; y++)
            for (ushort x = 0; x < pixelBuffer.Width;)
            {
                var point = new PixelPoint(x, y);
                Pixel pixel = pixelBuffer[point];

                if (pixel.IsCaret())
                {
                    if (caretPosition != null)
                        throw new InvalidOperationException("Caret is already shown");
                    caretPosition = new PixelBufferCoordinate(x, y);
                    caretStyle = pixel.CaretStyle;
                }

                // if it's not a dirty region, no need to paint it.
                if (!dirtyRegions.Contains(point, false))
                {
                    x += pixel.Width;
                    continue;
                }

                // if it's not changed, no reason to paint it.
                //todo: indexOutOfRange during resize
                if (_cache[x, y] == pixel)
                {
                    x += pixel.Width;
                    continue;
                }

                // cache the new value
                _cache[x, y] = pixel;


                flushingBuffer.WritePixel(new PixelBufferCoordinate(x, y), pixel);

                // for wide chars, fill skipped cells in the cache with empty pixels
                int end = Math.Min(pixelBuffer.Width, x + pixel.Width);
                for (int x2 = x; x2 < end; x2++)
                    _cache[x2, y] = Pixel.Empty;

                // advance for width of the char.
                x += pixel.Width;
            }

            // injecting cursor as last operation so it is always on top, it's not necessarily in a dirty region
            if (!_consoleCursor.IsEmpty())
            {
                Pixel currentPixel = pixelBuffer[_consoleCursor.Coordinate];

                // Calculate the inverse color
                Color invertColor = Color.FromRgb((byte)(255 - currentPixel.Background.Color.R),
                    (byte)(255 - currentPixel.Background.Color.G),
                    (byte)(255 - currentPixel.Background.Color.B));

                Pixel pixel =
                    currentPixel.Blend(new Pixel(new PixelForeground(new Symbol(_consoleCursor.Type), invertColor)));

                flushingBuffer.WritePixel(_consoleCursor.Coordinate, pixel);

                // we want to reset the cache for the cursor affected pixels 
                // because there can be widechars on either side, we invalidated cache by 2 on both left and right
                for (int x = Math.Max(0, _consoleCursor.Coordinate.X - 2);
                     x < Math.Min(pixelBuffer.Width, _consoleCursor.Coordinate.X + pixel.Width + 2);
                     x++)
                    _cache[x, _consoleCursor.Coordinate.Y] = Pixel.Empty;
            }

            flushingBuffer.Flush();

            if (caretPosition != null && caretStyle != CaretStyle.None)
            {
                _console.SetCaretPosition((PixelBufferCoordinate)caretPosition);
                _console.SetCaretStyle((CaretStyle)caretStyle!);
                _console.ShowCaret();
            }
            else
            {
                _console.HideCaret(); //todo: Caret was hidden at the beginning of this method, why to hide it again?
            }
        }

        private void OnCursorChanged(ConsoleCursor consoleCursor)
        {
            if (_consoleCursor.CompareTo(consoleCursor) == 0)
                return;

            ConsoleCursor oldConsoleCursor = _consoleCursor;
            _consoleCursor = consoleCursor;

            //todo: low excessive refresh, emptiness can be checked

            // Dirty rects expanded to handle potential wide char overlap
            var oldCursorRect = new PixelRect(oldConsoleCursor.Coordinate.X - 2,
                oldConsoleCursor.Coordinate.Y, oldConsoleCursor.Width + 4, 1);
            var newCursorRect = new PixelRect(consoleCursor.Coordinate.X - 2,
                consoleCursor.Coordinate.Y, consoleCursor.Width + 4, 1);
            _consoleTopLevelImpl.DirtyRegions.AddRect(oldCursorRect);
            _consoleTopLevelImpl.DirtyRegions.AddRect(newCursorRect);

            RenderToDevice();
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
                {
                    if (!string.IsNullOrEmpty(pixel.Foreground.Symbol.Complex))
                        _stringBuilder.Append(pixel.Foreground.Symbol.Complex);
                    else
                        _stringBuilder.Append(pixel.Foreground.Symbol.Character);
                }

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