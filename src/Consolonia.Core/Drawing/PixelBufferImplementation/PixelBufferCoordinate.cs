using System;
using Avalonia;

// ReSharper disable UnusedMember.Global

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    public readonly struct PixelBufferCoordinate(ushort x, ushort y) : IComparable<PixelBufferCoordinate>
    {
        public ushort X { get; } = x;
        public ushort Y { get; } = y;

        public void Deconstruct(out ushort x, out ushort y)
        {
            x = X;
            y = Y;
        }

        public static explicit operator PixelBufferCoordinate((ushort x, ushort y) val)
        {
            // ReSharper disable once ArrangeObjectCreationWhenTypeNotEvident
            return new(val.x, val.y);
        }

        public static explicit operator PixelBufferCoordinate(Point point)
        {
            // ReSharper disable once ArrangeObjectCreationWhenTypeNotEvident
            return new((ushort)point.X, (ushort)point.Y);
        }

        public PixelBufferCoordinate WithXpp()
        {
            // ReSharper disable once ArrangeObjectCreationWhenTypeNotEvident resharper behaves differently
            return new((ushort)(X + 1), Y);
        }

        public static PixelBufferCoordinate ToPixelBufferCoordinate((ushort x, ushort y) val)
        {
            return (PixelBufferCoordinate)val;
        }

        public static PixelBufferCoordinate ToPixelBufferCoordinate(Point point)
        {
            return (PixelBufferCoordinate)point;
        }

        public bool Equals(PixelBufferCoordinate secondPoint)
        {
            return X == secondPoint.X && Y == secondPoint.Y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public int CompareTo(PixelBufferCoordinate other)
        {
            int xComparison = X.CompareTo(other.X);
            return xComparison != 0 ? xComparison : Y.CompareTo(other.Y);
        }
    }
}