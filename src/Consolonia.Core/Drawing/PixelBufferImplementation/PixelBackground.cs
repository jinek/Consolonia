using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Media;
using Newtonsoft.Json;

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    [DebuggerDisplay("[{Color}]")]
    public readonly struct PixelBackground(Color color) : IEquatable<PixelBackground>
    {
        public PixelBackground() : this(Colors.Transparent)
        {
        }


        [JsonConverter(typeof(ColorConverter))]
        public Color Color { get; init; } = color;

        public bool Equals(PixelBackground other)
        {
            return Color.Equals(other.Color);
        }

        public override bool Equals([NotNullWhen(true)] object obj)
        {
            return obj is PixelBackground other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Color);
        }

        public static bool operator ==(PixelBackground left, PixelBackground right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PixelBackground left, PixelBackground right)
        {
            return !left.Equals(right);
        }
    }
}