using System;
using System.Diagnostics;
using Avalonia.Media;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    [DebuggerDisplay("'{Foreground.Symbol.Text}' [{Foreground.Color}, {Background.Color}]")]
    public readonly struct Pixel
    {
        public PixelForeground Foreground { get; }

        public PixelBackground Background { get; }

        public bool IsCaret { get; }

        /// <summary>
        /// Make a pixel foreground with transparent background
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="foregroundColor"></param>
        /// <param name="style"></param>
        /// <param name="weight"></param>
        /// <param name="textDecorations"></param>
        public Pixel(ISymbol symbol, 
            Color foregroundColor, 
            FontStyle style = FontStyle.Normal,
            FontWeight weight = FontWeight.Normal, 
            TextDecorationCollection textDecorations = null) : this(
            new PixelForeground(symbol, foregroundColor, weight, style, textDecorations),
            new PixelBackground(PixelBackgroundMode.Transparent))
        {
        }

        /// <summary>
        /// Make a pixel with only background color, but no foreground
        /// </summary>
        /// <param name="background"></param>
        public Pixel(PixelBackground background) : 
            this(new PixelForeground(new SimpleSymbol(), Colors.Transparent),
            background)
        {
        }

        /// <summary>
        /// Make a pixel with foreground and background
        /// </summary>
        /// <param name="foreground"></param>
        /// <param name="background"></param>
        /// <param name="isCaret"></param>
        public Pixel(PixelForeground foreground, 
            PixelBackground background, 
            bool isCaret = false)
        {
            Foreground = foreground;
            Background = background;
            IsCaret = isCaret;
        }

        /// <summary>
        /// Blend the pixelAbove with the this pixel.
        /// </summary>
        /// <param name="pixelAbove"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Pixel Blend(Pixel pixelAbove)
        {
            PixelForeground newForeground;
            PixelBackground newBackground;

            switch (pixelAbove.Background.Mode)
            {
                case PixelBackgroundMode.Colored:
                    // merge pixelAbove into this pixel using alpha channel.
                    Color mergedColors = MergeColors(Background.Color, pixelAbove.Background.Color);
                    newForeground = pixelAbove.Foreground;
                    newBackground = new PixelBackground(mergedColors);
                    break;

                case PixelBackgroundMode.Transparent:
                    // if the foreground is transparent, ignore pixelAbove foreground.
                    newForeground = pixelAbove.Foreground.Color != Colors.Transparent
                        ? Foreground.Blend(pixelAbove.Foreground)
                        : Foreground;

                    // background is transparent, ignore pixelAbove background.
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