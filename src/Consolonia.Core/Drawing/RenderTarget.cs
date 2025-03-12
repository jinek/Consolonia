#nullable enable
using System;
using System.IO;
using Avalonia;
using Avalonia.Platform;
using Consolonia.Core.Infrastructure;

namespace Consolonia.Core.Drawing
{
    internal class RenderTarget : IDrawingContextLayerImpl
    {
        private readonly ConsoleWindowImpl _consoleWindowImpl;

        internal RenderTarget(ConsoleWindowImpl consoleWindowImpl)
        {
            _consoleWindowImpl = consoleWindowImpl;
        }

        public void Dispose()
        {
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
                AvaloniaLocator.Current.GetService<RenderTargetWindows>()!.RenderWindows();
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
            return new DrawingContextImpl(_consoleWindowImpl);
        }

    }
}