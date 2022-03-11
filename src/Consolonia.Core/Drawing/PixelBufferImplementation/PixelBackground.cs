using System;

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    public readonly struct PixelBackground
    {
        public PixelBackground(PixelBackgroundMode mode, ConsoleColor color = ConsoleColor.Black)
        {
            Color = color;
            Mode = mode;
        }

        public ConsoleColor Color { get; }
        public PixelBackgroundMode Mode { get; }

        public PixelBackground Shade()
        {
            ConsoleColor newColor = Color;
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
                    newColor = ConsoleColor.Black;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new PixelBackground(newMode, newColor);
        }
    }
}