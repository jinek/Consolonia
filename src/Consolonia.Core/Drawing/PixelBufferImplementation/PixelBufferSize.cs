namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    // ReSharper disable once DefaultStructEqualityIsUsed.Global
    public readonly struct PixelBufferSize(ushort width, ushort height)
    {
        private PixelBufferCoordinate RightBottom { get; } = new(width, height);
        public ushort Width => RightBottom.X;
        public ushort Height => RightBottom.Y;

        public bool IsEmpty => Width == 0 && Height == 0;
    }
}