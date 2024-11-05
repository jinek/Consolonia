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
                    // merge pixelAbove into this pixel using alpha channel.
                    Color mergedColors = MergeColors(Background.Color, pixelAbove.Background.Color);
                    return new Pixel(pixelAbove.Foreground,
                        new PixelBackground(mergedColors));

                case PixelBackgroundMode.Transparent:
                    // when a textdecoration of underline happens a DrawLine() is called over the top of the a pixel with non-zero symbol.
                    // this detects this situation and eats the draw line, turning it into a textdecoration
                    if (pixelAbove.Foreground.Symbol is DrawingBoxSymbol &&
                        Foreground.Symbol is SimpleSymbol simpleSymbol &&
                        ((ISymbol)simpleSymbol).GetCharacter() != (char)0)
                        // this is a line being draw through text. add TextDecoration for underline.
                        newForeground = new PixelForeground(Foreground.Symbol, Foreground.Weight, Foreground.Style,
                            TextDecorations.Underline, Foreground.Color);
                    else
                        // do normal blend.
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

        /// <summary>
        ///     merge colors with alpha blending
        /// </summary>
        /// <param name="target"></param>
        /// <param name="source"></param>
        /// <returns>source blended into target</returns>
        private static Color MergeColors(Color target, Color source)
        {
            float alphaB = source.A / 255.0f;
            float inverseAlphaB = 1.0f - alphaB;

            byte red = (byte)(target.R * inverseAlphaB + source.R * alphaB);
            byte green = (byte)(target.G * inverseAlphaB + source.G * alphaB);
            byte blue = (byte)(target.B * inverseAlphaB + source.B * alphaB);

            return new Color(0xFF, red, green, blue);
        }
    }
}