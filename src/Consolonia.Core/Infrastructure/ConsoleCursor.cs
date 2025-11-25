using System;
using Consolonia.Controls;
using Consolonia.Core.Drawing.PixelBufferImplementation;

namespace Consolonia.Core.Infrastructure
{
    public readonly struct ConsoleCursor
        : IComparable<ConsoleCursor>, IEquatable<ConsoleCursor>
    {
        public PixelBufferCoordinate Coordinate { get; init; }
        public string Type { get; init; }
        public byte Width { get; init; }

        public ConsoleCursor(PixelBufferCoordinate coordinate, string type)
        {
            Coordinate = coordinate;
            Type = type;
            Width = (byte)type.MeasureText();
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(Type);
        }

        public int CompareTo(ConsoleCursor other)
        {
            if (string.IsNullOrEmpty(Type) && string.IsNullOrEmpty(other.Type))
                // empty/absent cursor is same anywhere on the screen
                return 0;
            
            int coordinateComparison = Coordinate.CompareTo(other.Coordinate);
            return coordinateComparison != 0
                ? coordinateComparison
                : string.Compare(Type, other.Type, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            if (obj is ConsoleCursor other)
                return Equals(other);

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
            return CompareTo(other) == 0;
        }
    }
}