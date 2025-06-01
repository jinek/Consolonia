using Consolonia.Core.Drawing.PixelBufferImplementation;

namespace Consolonia.Core.Infrastructure
{
    public readonly struct ConsoleCursor(PixelBufferCoordinate coordinate, string type)
    {
        public PixelBufferCoordinate Coordinate { get; init; } = coordinate;
        public string Type { get; init; } = type;

        public bool IsEmpty() => string.IsNullOrEmpty(Type);
    }
}