using System;
using Avalonia;

// ReSharper disable UnusedMember.Global

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    public readonly struct PixelBufferCoordinate(ushort x, ushort y)
        : IComparable<PixelBufferCoordinate>, IEquatable<PixelBufferCoordinate>
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

        public static explicit operator PixelBufferCoordinate(PixelPoint point)
        {
            //todo: replace this check by math overflow at project level.
            if (point.X < 0 || point.Y < 0 || point.X > ushort.MaxValue || point.Y > ushort.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(point));
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

        public static PixelBufferCoordinate ToPixelBufferCoordinate(PixelPoint point)
        {
            return (PixelBufferCoordinate)point;
        }

        public bool Equals(PixelBufferCoordinate other)
        {
            return X == other.X && Y == other.Y;
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

        public override bool Equals(object obj)
        {
            if (obj is PixelBufferCoordinate other) return Equals(other);

            return false;
        }

        public static bool operator ==(PixelBufferCoordinate left, PixelBufferCoordinate right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PixelBufferCoordinate left, PixelBufferCoordinate right)
        {
            return !(left == right);
        }

        public static bool operator <(PixelBufferCoordinate left, PixelBufferCoordinate right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(PixelBufferCoordinate left, PixelBufferCoordinate right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(PixelBufferCoordinate left, PixelBufferCoordinate right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(PixelBufferCoordinate left, PixelBufferCoordinate right)
        {
            return left.CompareTo(right) >= 0;
        }
    }
}