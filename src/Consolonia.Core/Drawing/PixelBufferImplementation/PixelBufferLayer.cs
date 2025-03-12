using Avalonia;

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    /// <summary>
    /// A pixelbuffer which has Position
    /// </summary>
    public class PixelBufferLayer : PixelBuffer
    {
        private PixelBufferSurface _manager;

        public PixelBufferLayer(PixelBufferSurface surface, ushort x, ushort y, ushort width, ushort height)
            : base(new PixelBufferCoordinate(x, y), new PixelBufferSize(width, height))
        {
            _manager = surface;
        }

        public void BringToFront()
        {
            _manager.Layers.Remove(this);
            _manager.Layers.Add(this);
        }

        public void SendToBack()
        {
            _manager.Layers.Remove(this);
            _manager.Layers.Insert(0, this);
        }

        public void Close()
        {
            _manager.Layers.Remove(this);   
        }

        public virtual void Blend()
            => BitBlt(Position.X, Position.Y, _manager);
    }
}

