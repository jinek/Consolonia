using System;
using Consolonia.Core.Drawing.PixelBufferImplementation;

namespace Consolonia.Core.Infrastructure
{
    public readonly struct ConsoleCursor(PixelBufferCoordinate coordinate, string type) : IComparable<ConsoleCursor>
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
    }
}