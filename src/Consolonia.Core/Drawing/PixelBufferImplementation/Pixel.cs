using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Media;
using Consolonia.Core.Text;

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

        public Pixel(char character, Color foregroundColor, FontStyle style = FontStyle.Normal, FontWeight weight = FontWeight.Normal) :
            this(new SimpleSymbol(character), foregroundColor, style, weight)
        {
        }

        public Pixel(byte drawingBoxSymbol, Color foregroundColor) : this(
            new DrawingBoxSymbol(drawingBoxSymbol), foregroundColor, FontStyle.Normal, FontWeight.Normal)
        {
        }

        public Pixel(ISymbol symbol, Color foregroundColor, FontStyle style = FontStyle.Normal, FontWeight weight = FontWeight.Normal) : this(
            new PixelForeground(symbol, weight, style, foregroundColor),
            new PixelBackground(PixelBackgroundMode.Transparent))
        {
        }

        public Pixel(Color backgroundColor, IGlyphTypeface typeface = null) : this(
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
                    // when a textdecoration of underline happens a DrawLine() is called over the top of the a pixel with non-zero symbol.
                    // this detects this situation and eats the draw line, instead changing the underling text to be fontStyle=Oblique
                    if (pixelAbove.Foreground.Symbol is DrawingBoxSymbol box &&
                        this.Foreground.Symbol is SimpleSymbol simpleSymbol &&
                        simpleSymbol is ISymbol symbol &&
                        symbol.GetCharacter() != (Char)0)
                    {
                        // this is a line being draw through text. use fontstyle.Oblique to signal this.
                        newForeground = new PixelForeground(this.Foreground.Symbol, this.Foreground.Weight, FontStyle.Oblique, this.Foreground.Color);
                    }
                    else
                    {
                        // do normal blend.
                        newForeground = Foreground.Blend(pixelAbove.Foreground);
                    }
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