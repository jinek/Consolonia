using Avalonia;
// ReSharper disable UnusedMember.Global

namespace Consolonia.Core.Drawing.PixelBufferImplementation
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

        public static explicit operator PixelBufferCoordinate((ushort x, ushort y) val)
        {
            return new PixelBufferCoordinate(val.x, val.y);
        }

        public static explicit operator PixelBufferCoordinate(Point point)
        {
            return new PixelBufferCoordinate((ushort)point.X, (ushort)point.Y);
        }

        public PixelBufferCoordinate WithXpp()
        {
            return new PixelBufferCoordinate((ushort)(X + 1), Y);
        }

        public static PixelBufferCoordinate ToPixelBufferCoordinate((ushort x, ushort y) val)
        {
            return (PixelBufferCoordinate)val;
        }

        public static PixelBufferCoordinate ToPixelBufferCoordinate(Point point)
        {
            return (PixelBufferCoordinate)point;
        }
    }
}