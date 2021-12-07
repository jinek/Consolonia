using Avalonia;

namespace Consolonia.Core.Drawing.PixelBuffer
{
    public readonly struct PixelBufferCoordinate
    {
        public PixelBufferCoordinate(ushort x, ushort y)
        {
            X = x;
            Y = y;
        }

        public ushort X { get; }
        public ushort Y { get; }

        public void Deconstruct(out ushort x, out ushort y)
        {
            x = X;
            y = Y;
        }

        public static explicit operator PixelBufferCoordinate((ushort x, ushort y) val) => new(val.x, val.y);
        public static explicit operator PixelBufferCoordinate(Point point) => new((ushort)point.X, (ushort)point.Y);

        public PixelBufferCoordinate WithXpp()
        {
            return new PixelBufferCoordinate((ushort)(X + 1), Y);
        }
    }
}