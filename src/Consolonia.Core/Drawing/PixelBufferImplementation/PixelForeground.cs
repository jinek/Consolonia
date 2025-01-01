using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Media;
using Newtonsoft.Json;

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    [DebuggerDisplay("'{Symbol.Text}' [{Color}]")]
    public readonly struct PixelForeground : IEquatable<PixelForeground>
    {
        public PixelForeground()
        {
            Symbol = new SimpleSymbol(" ");
            Color = Colors.Transparent;
            Weight = null;
            Style = null;
            TextDecoration = null;
        }

        public PixelForeground(ISymbol symbol, Color color,
            FontWeight? weight = null, FontStyle? style = null,
            TextDecorationLocation? textDecoration = null)
        {
            ArgumentNullException.ThrowIfNull(symbol);
            Symbol = symbol;
            Color = color;
            Weight = weight;
            Style = style;
            TextDecoration = textDecoration;
        }

        public ISymbol Symbol { get; init; }

        [JsonConverter(typeof(ColorConverter))]
        public Color Color { get; init; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FontWeight? Weight { get; init; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FontStyle? Style { get; init; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public TextDecorationLocation? TextDecoration { get; init; }

        public bool Equals(PixelForeground other)
        {
            return Symbol.Equals(other.Symbol) &&
                   Color.Equals(other.Color) &&
                   Weight.Equals(other.Weight) &&
                   Style.Equals(other.Style) &&
                   TextDecoration == other.TextDecoration;
        }

        public PixelForeground Shade()
        {
            return new PixelForeground(Symbol, Color.Shade(), Weight, Style, TextDecoration);
        }

        public PixelForeground Brighten()
        {
            return new PixelForeground(Symbol, Color.Brighten(), Weight, Style, TextDecoration);
        }

        public PixelForeground Blend(PixelForeground pixelAboveForeground)
        {
            //todo: check default(char) is there
            ISymbol symbolAbove = pixelAboveForeground.Symbol;
            ArgumentNullException.ThrowIfNull(symbolAbove);

            if (pixelAboveForeground.Color == Colors.Transparent)
                // if pixelAbove is transparent then the foreground below should be unchanged.
                return this;

            return new PixelForeground(Symbol.Blend(ref symbolAbove),
                pixelAboveForeground.Color,
                pixelAboveForeground.Weight ?? Weight,
                pixelAboveForeground.Style ?? Style,
                pixelAboveForeground.TextDecoration ?? TextDecoration);
        }

        public override bool Equals([NotNullWhen(true)] object obj)
        {
            return obj is PixelForeground other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Symbol, Color, (int)(Weight ?? FontWeight.Normal), (int)(Style ?? FontStyle.Normal),
                TextDecoration);
        }

        public static bool operator ==(PixelForeground left, PixelForeground right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PixelForeground left, PixelForeground right)
        {
            return !left.Equals(right);
        }

        public bool NothingToDraw()
        {
            return Color.A == 0x0 || Symbol.NothingToDraw();
        }
    }
}