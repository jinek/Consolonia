using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Platform;
using Avalonia.Rendering;
using Consolonia.Core.Drawing.PixelBuffer;
using Consolonia.Core.Infrastructure;

namespace Consolonia.Core.Drawing
{
    internal class RenderTarget : IDrawingContextLayerImpl
    {
        internal PixelBuffer.PixelBuffer _bufferBuffer { get; private set; }

        private readonly ConsoleWindow _consoleWindow;
        private readonly IConsole _console;

        internal RenderTarget(ConsoleWindow consoleWindow)
        {
            _console = AvaloniaLocator.Current.GetService<IConsole>()!;
            _consoleWindow = consoleWindow;
            consoleWindow.Resized += OnResized;
            InitializeBuffer();
        }

        private void OnResized(Size _)
        {
            InitializeBuffer();
        }

        private void InitializeBuffer()
        {
            (double width, double height) = _consoleWindow.ClientSize;
            _bufferBuffer =
                new PixelBuffer.PixelBuffer((ushort)width, (ushort)height);
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
            RenderToDevice();
        }

        public bool CanBlit => true;

        private void RenderToDevice()
        {
            PixelBuffer.PixelBuffer pixelBuffer = _bufferBuffer;

            _console.CaretVisible = false;
            PixelBufferCoordinate? caretPosition = null;

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

                _console.Print(new PixelBufferCoordinate(x, y), pixel.Background.Color, pixel.Foreground.Color,
                    pixel.Foreground.Symbol.GetCharacter());
            }

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
    }
}