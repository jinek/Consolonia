using System;

namespace Consolonia.Core.Drawing.PixelBuffer
{
    public readonly struct Pixel
    {
        public readonly PixelForeground Foreground;
        public readonly PixelBackground Background;

        public Pixel(char character, ConsoleColor foregroundColor) : this(new SimpleSymbol(character),
            foregroundColor)
        {
        }

        public Pixel(byte drawingBoxSymbol, ConsoleColor foregroundColor) : this(
            new DrawingBoxSymbol(drawingBoxSymbol), foregroundColor)
        {
        }

        public Pixel(ISymbol symbol, ConsoleColor foregroundColor) : this(
            new PixelForeground(symbol, foregroundColor),
            new PixelBackground(PixelBackgroundMode.Transparent))
        {
        }

        public Pixel(ConsoleColor backgroundColor) : this(
            new PixelBackground(PixelBackgroundMode.Colored, backgroundColor))
        {
        }

        public Pixel(PixelBackgroundMode mode) : this(new PixelBackground(mode))
        {
        }

        public Pixel(PixelBackground background) : this(new PixelForeground(new SimpleSymbol()),
            background)
        {
        }

        public Pixel(PixelForeground foreground, PixelBackground background)
        {
            Foreground = foreground;
            Background = background;
        }

        public Pixel Blend(Pixel pixelAbove)
        {
            PixelForeground newForeground;
            PixelBackground newBackground;
            switch (pixelAbove.Background.Mode)
            {
                case PixelBackgroundMode.Colored:
                    return pixelAbove;
                case PixelBackgroundMode.Transparent:
                    newForeground = Foreground.Blend(pixelAbove.Foreground);
                    newBackground = Background;
                    break;
                case PixelBackgroundMode.Shaded:
                    (newForeground, newBackground) = Shade();
                    newForeground = newForeground.Blend(pixelAbove.Foreground);
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(pixelAbove));
            }

            return new Pixel(newForeground, newBackground);
        }

        private (PixelForeground, PixelBackground) Shade()
        {
            return (Foreground.Shade(), Background.Shade());
        }
    }
}