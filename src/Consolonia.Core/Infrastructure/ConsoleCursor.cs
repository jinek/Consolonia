using System;
using Consolonia.Core.Drawing.PixelBufferImplementation;

namespace Consolonia.Core.Infrastructure
{
    public readonly struct ConsoleCursor(PixelBufferCoordinate coordinate, string type)
        : IComparable<ConsoleCursor>, IEquatable<ConsoleCursor>
    {
        public PixelBufferCoordinate Coordinate { get; init; } = coordinate;
        public string Type { get; init; } = type;

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(Type);
        }

        public int CompareTo(ConsoleCursor other)
        {
            int coordinateComparison = Coordinate.CompareTo(other.Coordinate);
            return coordinateComparison != 0
                ? coordinateComparison
                : string.Compare(Type, other.Type, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            if (obj is ConsoleCursor other)
            {
                return Coordinate.Equals(other.Coordinate) && string.Equals(Type, other.Type, StringComparison.Ordinal);
            }

            return false;
        }

        public override int GetHashCode()
        {
            //by copilot
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Coordinate.GetHashCode();
                hash = hash * 23 + (Type?.GetHashCode(StringComparison.Ordinal) ?? 0);
                return hash;
            }
        }

        public static bool operator ==(ConsoleCursor left, ConsoleCursor right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ConsoleCursor left, ConsoleCursor right)
        {
            return !(left == right);
        }

        public static bool operator <(ConsoleCursor left, ConsoleCursor right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(ConsoleCursor left, ConsoleCursor right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(ConsoleCursor left, ConsoleCursor right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(ConsoleCursor left, ConsoleCursor right)
        {
            return left.CompareTo(right) >= 0;
        }

        public bool Equals(ConsoleCursor other)
        {
            return Coordinate.Equals(other.Coordinate) && Type == other.Type;
        }
    }
}

