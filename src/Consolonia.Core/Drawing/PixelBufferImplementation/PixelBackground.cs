using System;
using Avalonia.Media;

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    public readonly struct PixelBackground
    {
        public PixelBackground(PixelBackgroundMode mode, Color? color = null)
        {
            Color = color ?? Colors.Black;
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

    }
}