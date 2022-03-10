namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    public readonly struct PixelBufferSize
    {
        public PixelBufferSize(ushort width, ushort height)
        {
            RightBottom = new PixelBufferCoordinate(width, height);
        }

        private PixelBufferCoordinate RightBottom { get; }
        public ushort Width => RightBottom.X;
        public ushort Height => RightBottom.Y;
    }
}