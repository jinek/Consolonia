using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Media;
using Newtonsoft.Json;

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    [DebuggerDisplay("[{Color}, {Mode}]")]
    public class PixelBackground : IEquatable<PixelBackground>
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
        public Color Color { get; set; }
        
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public PixelBackgroundMode Mode { get; set; }

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

        public bool Equals(PixelBackground other)
        {
            return Color.Equals(other.Color) && Mode == other.Mode;
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
            return left.Equals(right);
        }

        public static bool operator !=(PixelBackground left, PixelBackground right)
        {
            return !left.Equals(right);
        }
    }
}