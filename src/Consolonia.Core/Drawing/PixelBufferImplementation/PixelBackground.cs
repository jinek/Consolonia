using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Media;
using Newtonsoft.Json;

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    [DebuggerDisplay("[{Color}, {Mode}]")]
    public readonly struct PixelBackground : IEquatable<PixelBackground>
    {
        public PixelBackground()
        {
            Mode = PixelBackgroundMode.Transparent;
            Color = Colors.Transparent;
        }

        public PixelBackground(Color color)
        {
            Mode = color.A == 0 ? PixelBackgroundMode.Transparent : PixelBackgroundMode.Colored;
            Color = color;
        }

        public PixelBackground(PixelBackgroundMode mode, Color? color = null)
        {
            Color = color ?? Colors.Transparent;
            Mode = mode;
        }

        [JsonConverter(typeof(ColorConverter))]
        public Color Color { get; init; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public PixelBackgroundMode Mode { get; init; }

        public bool Equals(PixelBackground other)
        {
            if ((object)other is null) return false;
            return Color.Equals(other.Color) && Mode == other.Mode;
        }

        public PixelBackground Shade()
        {
            Color newColor = Color;
            PixelBackgroundMode newMode = Mode;
            switch (Mode)
            {
                case PixelBackgroundMode.Colored:
                    newColor = Color.Shade();
                    break;
                case PixelBackgroundMode.Transparent:
                    newMode = PixelBackgroundMode.Shaded;
                    break;
                case PixelBackgroundMode.Shaded:
                    newMode = PixelBackgroundMode.Colored;
                    newColor = Colors.Black;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new PixelBackground(newMode, newColor);
        }

        public override bool Equals([NotNullWhen(true)] object obj)
        {
            return obj is PixelBackground other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Color, Mode);
        }

        public static bool operator ==(PixelBackground left, PixelBackground right)
        {
            if ((object)left is null) return (object)right is null;
            return left.Equals(right);
        }

        public static bool operator !=(PixelBackground left, PixelBackground right)
        {
            if ((object)left == null) return (object)right != null;
            return !left.Equals(right);
        }
    }
}