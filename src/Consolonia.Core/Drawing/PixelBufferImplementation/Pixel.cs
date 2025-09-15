using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Media;
using Consolonia.Controls;
using Newtonsoft.Json;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable NotResolvedInText
namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    [SuppressMessage("ReSharper", "NotResolvedInText",
        Justification = "Properties are accessible but are not seen by resharper", Scope = "type")]
    [SuppressMessage("ReSharper", "NotResolvedInText", MessageId = "Color")]
    [SuppressMessage("ReSharper", "NotResolvedInText", MessageId = "Symbol")]
    [DebuggerDisplay(
        "'{Foreground.Symbol.Text}', Foreground: {Foreground.Color}, Background: {Background.Color}, CaretStyle: {CaretStyle}")]
    public struct Pixel : IEquatable<Pixel>
    {
        private static readonly Lazy<IConsoleColorMode> ConsoleColorMode =
            new(() => AvaloniaLocator.Current.GetRequiredService<IConsoleColorMode>());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Pixel()
        {
            Foreground = PixelForeground.Default;
            Background = PixelBackground.Transparent;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Pixel(CaretStyle caretStyle)
        {
            Foreground = PixelForeground.Default;
            Background = PixelBackground.Transparent;
            CaretStyle = caretStyle;
        }

        /// <summary>
        ///     Make a pixel foreground with transparent background
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="foregroundColor"></param>
        /// <param name="style"></param>
        /// <param name="weight"></param>
        /// <param name="textDecorations"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Pixel(ISymbol symbol,
            Color foregroundColor,
            FontStyle style = FontStyle.Normal,
            FontWeight weight = FontWeight.Normal,
            TextDecorationLocation? textDecorations = null) : this(
            new PixelForeground(symbol, foregroundColor, weight, style, textDecorations),
            PixelBackground.Transparent)
        {
        }

        /// <summary>
        ///     Make a pixel with only background color, but no foreground
        /// </summary>
        /// <param name="background"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Pixel(PixelBackground background) :
            this(PixelForeground.Default, background)
        {
        }

        /// <summary>
        ///     Make a pixel with foreground and background
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Pixel(PixelForeground foreground,
            CaretStyle caretStyle = CaretStyle.None)
        {
            Foreground = foreground;
            Background = PixelBackground.Transparent;
            CaretStyle = caretStyle;
        }

        /// <summary>
        ///     Make a pixel with foreground and background
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Pixel(PixelForeground foreground,
            PixelBackground background,
            CaretStyle caretStyle = CaretStyle.None)
        {
            Foreground = foreground;
            Background = background;
            CaretStyle = caretStyle;
        }

        // Pixel empty is a non-pixel. It has no symbol, no color, no weight, no style, no text decoration, and no background.
        // it is used only when a multichar sequence overlaps a pixel making it a non-entity.
        public static Pixel Empty => new();

        // pixel space is a pixel with a space symbol, but could have color blended into. it is used to advance the cursor
        // and set the background color
        public static Pixel Space => new(new PixelForeground(SimpleSymbol.Space, Colors.Transparent),
            PixelBackground.Transparent);

#pragma warning disable CA1051 // Do not declare visible instance fields
        public PixelForeground Foreground;

        public PixelBackground Background;

        public CaretStyle CaretStyle = CaretStyle.BlinkingBar;
#pragma warning restore CA1051 // Do not declare visible instance fields

        [JsonIgnore] public ushort Width => Foreground.Symbol.Width;

        public bool Equals(Pixel other)
        {
            return EqualityComparer<PixelForeground>.Default.Equals(Foreground, other.Foreground) &&
                   EqualityComparer<PixelBackground>.Default.Equals(Background, other.Background) &&
                   CaretStyle == other.CaretStyle;
        }

        public void Shade()
        {
            Foreground.Shade();
            Background.Shade();
        }

        public void Brighten()
        {
            Foreground.Brighten();
            Background.Brighten();
        }

        public void Invert()
        {
            var foreGroundColor = Foreground.Color;
            Foreground.Color = Background.Color;
            Background.Color = foreGroundColor;
        }

        /// <summary>
        ///     Blend the pixelAbove with this pixel.
        /// </summary>
        /// <param name="pixelAbove"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void Blend(ref Pixel pixelAbove)
        {
            PixelForeground newForeground;

            var newBackground = new PixelBackground(MergeColors(Background.Color, pixelAbove.Background.Color));

            CaretStyle newCaretStyle;

            //todo: logic of IsNothingToDraw overlaps with following if which overlaps Foreground.Blend - do we do double checks?
            bool isNoForegroundOnTop = pixelAbove.Foreground.IsNothingToDraw();
            if (pixelAbove.Background.Color.A == 0x0 /*todo: can be approximate, extract to extension method*/)
            {
                newForeground = isNoForegroundOnTop ? Foreground : Foreground.Blend(pixelAbove.Foreground);
                newCaretStyle = CaretStyle.Blend(pixelAbove.CaretStyle);
            }
            else
            {
                if (!isNoForegroundOnTop)
                {
                    newForeground = pixelAbove.Foreground;
                }
                else
                {
                    // merge the PixelForeground color with the pixelAbove background color

                    if (pixelAbove.Background.Color.A == 0xFF)
                        // non-transparent layer above
                        newForeground = PixelForeground.Default;
                    else
                        newForeground = new PixelForeground(Foreground.Symbol,
                            MergeColors(Foreground.Color, pixelAbove.Background.Color),
                            Foreground.Weight,
                            Foreground.Style,
                            Foreground.TextDecoration);
                }

                newCaretStyle = pixelAbove.CaretStyle;
            }

            this.Foreground = newForeground;
            this.Background = newBackground;
            this.CaretStyle = newCaretStyle;
        }

        /// <summary>
        ///     merge colors with alpha blending
        /// </summary>
        /// <param name="target"></param>
        /// <param name="source"></param>
        /// <returns>source blended into target</returns>
        private static Color MergeColors(Color target, Color source)
        {
            return ConsoleColorMode.Value.Blend(target, source);
        }

        public override bool Equals([NotNullWhen(true)] object obj)
        {
            return obj is Pixel pixel && Equals(pixel);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Foreground, Background, CaretStyle);
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