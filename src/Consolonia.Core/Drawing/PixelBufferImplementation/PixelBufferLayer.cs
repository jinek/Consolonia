using Avalonia;

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    internal class PixelBufferLayer : PixelBuffer
    {
        private PixelBufferLayerManager _manager;

        internal PixelBufferLayer(PixelBufferLayerManager manager, ushort x, ushort y, ushort width, ushort height)
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

        public void Blend(PixelBuffer targetPixelBuffer)
            => Blend(Position, targetPixelBuffer);
    }
}

