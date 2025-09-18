using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Avalonia.Media;
using Newtonsoft.Json;

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    [DebuggerDisplay("[{Color}]")]
    [JsonConverter(typeof(PixelBackgroundConverter))]
    public readonly struct PixelBackground(Color color) : IEquatable<PixelBackground>
    {
        public static readonly PixelBackground Transparent = new();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PixelBackground() : this(Colors.Transparent)
        {
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PixelBackground Shade()
        {
            return new PixelBackground(Color.Shade());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PixelBackground Brighten()
        {
            return new PixelBackground(Color.Brighten());
        }

#pragma warning disable CA1051 // Do not declare visible instance fields
        [JsonConverter(typeof(ColorConverter))]
        public readonly Color Color = color;
#pragma warning restore CA1051 // Do not declare visible instance fields

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