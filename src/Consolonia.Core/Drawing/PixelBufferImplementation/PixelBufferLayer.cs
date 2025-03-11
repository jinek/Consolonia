using Avalonia;

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    /// <summary>
    /// A pixelbuffer which has Position
    /// </summary>
    internal class PixelBufferLayer : PixelBuffer
    {
        private PixelBufferSurface _manager;

        internal PixelBufferLayer(PixelBufferSurface manager, ushort x, ushort y, ushort width, ushort height)
            : base(width, height)
        {
            _manager = manager;
            Position = new PixelPoint(x, y);
        }

        public PixelPoint Position { get; set; }

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
            => BitBlt(Position, _manager);
    }
}

