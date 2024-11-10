using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Media;

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    [DebuggerDisplay("[{Color}, {Mode}]")]
    public readonly struct PixelBackground : IEquatable<PixelBackground>
    {
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

        public Color Color { get; }
        public PixelBackgroundMode Mode { get; }

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
            => Color.Equals(other.Color) && Mode == other.Mode;

        public override bool Equals([NotNullWhen(true)] object obj)
            => obj is PixelBackground other && this.Equals(other);

        public override int GetHashCode()
            => HashCode.Combine(Color, Mode);

        public static bool operator ==(PixelBackground left, PixelBackground right)
            => left.Equals(right);

        public static bool operator !=(PixelBackground left, PixelBackground right)
            => !left.Equals(right);
    }
}