using System;

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    public readonly struct Pixel
    {
        public PixelForeground Foreground { get; }
        public PixelBackground Background { get; }
        public bool IsCaret { get; }

        public Pixel(bool isCaret) : this(PixelBackgroundMode.Transparent)
        {
            IsCaret = isCaret;
        }
        
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

        public Pixel(PixelForeground foreground, PixelBackground background, bool isCaret=false)
        {
            Foreground = foreground;
            Background = background;
            IsCaret = isCaret;
        }

        public Pixel Blend(Pixel pixelAbove)
        {
            PixelForeground newForeground;
            PixelBackground newBackground;
            bool newIsCaret;
            
            switch (pixelAbove.Background.Mode)
            {
                case PixelBackgroundMode.Colored:
                    return pixelAbove;
                case PixelBackgroundMode.Transparent:
                    newForeground = Foreground.Blend(pixelAbove.Foreground);
                    newBackground = Background;
                    newIsCaret = IsCaret | pixelAbove.IsCaret;
                    break;
                case PixelBackgroundMode.Shaded:
                    (newForeground, newBackground) = Shade();
                    newForeground = newForeground.Blend(pixelAbove.Foreground);
                    newIsCaret = IsCaret | pixelAbove.IsCaret;
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(pixelAbove));
            }

            return new Pixel(newForeground, newBackground, newIsCaret);
        }

        private (PixelForeground, PixelBackground) Shade()
        {
            return (Foreground.Shade(), Background.Shade());
        }
    }
}