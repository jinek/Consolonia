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
            new PixelBackground())
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
        ///     Blend the pixelAbove with this pixel.
        /// </summary>
        /// <param name="pixelAbove"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Pixel Blend(Pixel pixelAbove)
        {
            PixelForeground newForeground;

            var newBackground = new PixelBackground(MergeColors(Background.Color, pixelAbove.Background.Color));

            bool newIsCaret;

            if (pixelAbove.Background.Color.A == 0x0 /*todo: can be approximate, extract to extension method*/)
            {
                newForeground = pixelAbove.Foreground.Color == Colors.Transparent
                    ? Foreground
                    : Foreground.Blend(pixelAbove.Foreground);
                newIsCaret = pixelAbove.IsCaret | IsCaret;
            }
            else
            {
                if (!pixelAbove.IsEmpty())
                {
                    newForeground = pixelAbove.Foreground;
                }
                else
                {
                    newForeground = new PixelForeground(Foreground.Symbol,
                        MergeColors(Foreground.Color, pixelAbove.Background.Color), Foreground.Weight,
                        Foreground.Style,
                        Foreground.TextDecoration);
                }

                newIsCaret = pixelAbove.IsCaret;
            }

            return new Pixel(newForeground, newBackground, newIsCaret);
        }

        public bool IsEmpty()
        {
            return Foreground.Symbol.Width == 0;
        }

        /*private (PixelForeground, PixelBackground) Shade()
        {
            return (Foreground.Shade(), Background.Shade());
        }*/

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
            return obj is Pixel pixel && Equals(pixel);
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