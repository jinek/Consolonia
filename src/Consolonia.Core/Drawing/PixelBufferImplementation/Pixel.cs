using System;
using Avalonia.Media;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

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

        public Pixel(char character, Color foregroundColor, FontStyle style = FontStyle.Normal,
            FontWeight weight = FontWeight.Normal) :
            this(new SimpleSymbol(character), foregroundColor, style, weight)
        {
        }

        public Pixel(byte drawingBoxSymbol, Color foregroundColor) : this(
            new DrawingBoxSymbol(drawingBoxSymbol), foregroundColor)
        {
        }

        public Pixel(ISymbol symbol, Color foregroundColor, FontStyle style = FontStyle.Normal,
            FontWeight weight = FontWeight.Normal, TextDecorationCollection textDecorations = null) : this(
            new PixelForeground(symbol, weight, style, textDecorations, foregroundColor),
            new PixelBackground(PixelBackgroundMode.Transparent))
        {
        }

        public Pixel(Color backgroundColor) : this(
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

        public Pixel(PixelForeground foreground, PixelBackground background, bool isCaret = false)
        {
            Foreground = foreground;
            Background = background;
            IsCaret = isCaret;
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

            bool newIsCaret = IsCaret | pixelAbove.IsCaret;

            return new Pixel(newForeground, newBackground, newIsCaret);
        }

        private (PixelForeground, PixelBackground) Shade()
        {
            return (Foreground.Shade(), Background.Shade());
        }
    }
}