using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Media;
using Newtonsoft.Json;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    [DebuggerDisplay("'{Foreground.Symbol.Text}' [{Foreground.Color}, {Background.Color}]")]
    public readonly struct Pixel : IEquatable<Pixel>
    {
        public Pixel()
        {
            Foreground = new PixelForeground();
            Background = new PixelBackground();
            IsCaret = false;
        }

        public Pixel(bool isCaret)
        {
            Foreground = new PixelForeground();
            Background = new PixelBackground();
            IsCaret = isCaret;
        }

        /// <summary>
        ///     Make a pixel foreground with transparent background
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
            TextDecorationLocation? textDecorations = null) : this(
            new PixelForeground(symbol, foregroundColor, weight, style, textDecorations),
            new PixelBackground(PixelBackgroundMode.Transparent))
        {
        }

        /// <summary>
        ///     Make a pixel with only background color, but no foreground
        /// </summary>
        /// <param name="background"></param>
        public Pixel(PixelBackground background) :
            this(new PixelForeground(new SimpleSymbol(' '), Colors.Transparent),
                background)
        {
        }

        /// <summary>
        ///     Make a pixel with foreground and background
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

        // Pixel empty is a non-pixel. It has no symbol, no color, no weight, no style, no text decoration, and no background.
        // it is used only when a multichar sequence overlaps a pixel making it a non-entity.
        public static Pixel Empty => new();

        // pixel space is a pixel with a space symbol, but could have color blended into. it is used to advance the cursor
        // and set the background color
        public static Pixel Space => new(new PixelForeground(new SimpleSymbol(' '), Colors.Transparent),
            new PixelBackground(Colors.Transparent));

        public PixelForeground Foreground { get; init; }

        public PixelBackground Background { get; init; }

        public bool IsCaret { get; init; }

        [JsonIgnore] public ushort Width => Foreground.Symbol.Width;

        public bool Equals(Pixel other)
        {
            return Foreground.Equals(other.Foreground) &&
                   Background.Equals(other.Background) &&
                   IsCaret.Equals(other.IsCaret);
        }

        /// <summary>
        ///     Blend the pixelAbove with the this pixel.
        /// </summary>
        /// <param name="pixelAbove"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Pixel Blend(Pixel pixelAbove)
        {
            PixelForeground newForeground;
            PixelBackground newBackground;

            if (pixelAbove.IsCaret) return new Pixel(Foreground, Background, true);

            switch (pixelAbove.Background.Mode)
            {
                case PixelBackgroundMode.Colored:
                    // merge pixelAbove into this pixel using alpha channel.
                    Color mergedColors = MergeColors(Background.Color, pixelAbove.Background.Color);
                    newForeground = pixelAbove.Foreground;
                    newBackground = new PixelBackground(mergedColors);
                    return new Pixel(newForeground, newBackground, pixelAbove.IsCaret);

                case PixelBackgroundMode.Transparent:
                    // if the foreground is transparent, ignore pixelAbove foreground.
                    newForeground = pixelAbove.Foreground.Color != Colors.Transparent
                        ? Foreground.Blend(pixelAbove.Foreground)
                        : Foreground;

                    // background is transparent, ignore pixelAbove background.
                    newBackground = Background;
                    break;
                case PixelBackgroundMode.Shaded:
                    // shade the current pixel
                    (newForeground, newBackground) = Shade();
                    
                    // blend the pixelAbove foreground into the shaded pixel
                    newForeground = newForeground.Blend(pixelAbove.Foreground);
                    
                    // resulting in new pixel with shaded background and blended foreground
                    return new Pixel(newForeground, newBackground);

                default: throw new ArgumentOutOfRangeException(nameof(pixelAbove));
            }

            bool newIsCaret = IsCaret | pixelAbove.IsCaret;

            return new Pixel(newForeground, newBackground, newIsCaret);
        }

        public bool IsEmpty()
        {
            return Foreground.Symbol.Width == 0;
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

        public override bool Equals([NotNullWhen(true)] object obj)
        {
            return obj is Pixel && Equals((Pixel)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Foreground, Background, IsCaret);
        }

        public static bool operator ==(Pixel left, Pixel right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Pixel left, Pixel right)
        {
            return !left.Equals(right);
        }
    }
}