using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Media;
using Newtonsoft.Json;

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    [DebuggerDisplay("'{Symbol.Text}' [{Color}]")]
    public class PixelForeground : IEquatable<PixelForeground>
    {
        public PixelForeground()
        {
            Symbol = new SimpleSymbol();
            Color = Colors.Transparent;
            Weight = FontWeight.Normal;
            Style = FontStyle.Normal;
            TextDecoration = null;
        }

        public PixelForeground(ISymbol symbol, Color color,
            FontWeight weight = FontWeight.Normal, FontStyle style = FontStyle.Normal,
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

        [DefaultValue(FontWeight.Normal)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public FontWeight Weight { get; init; }

        [DefaultValue(FontStyle.Normal)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public FontStyle Style { get; init; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public TextDecorationLocation? TextDecoration { get; init; }

        public bool Equals(PixelForeground other)
        {
            if ((object)other is null) return false;

            return Symbol.Equals(other.Symbol) &&
                   Color.Equals(other.Color) &&
                   Weight == other.Weight &&
                   Style == other.Style &&
                   Equals(TextDecoration, other.TextDecoration);
        }

        public PixelForeground Shade()
        {
            Color newColor = Color.Shade();
            return new PixelForeground(Symbol, newColor, Weight, Style, TextDecoration);
        }

        public PixelForeground Blend(PixelForeground pixelAboveForeground)
        {
            //todo: check default(char) is there
            ISymbol symbolAbove = pixelAboveForeground.Symbol;
            ArgumentNullException.ThrowIfNull(symbolAbove);

            ISymbol newSymbol = Symbol.Blend(ref symbolAbove);

            return new PixelForeground(newSymbol, pixelAboveForeground.Color, pixelAboveForeground.Weight,
                pixelAboveForeground.Style, pixelAboveForeground.TextDecoration);
        }

        public override bool Equals([NotNullWhen(true)] object obj)
        {
            return obj is PixelForeground other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Symbol, Color, (int)Weight, (int)Style, TextDecoration);
        }

        public static bool operator ==(PixelForeground left, PixelForeground right)
        {
            if ((object)left is null) return (object)right is null;
            return left.Equals(right);
        }

        public static bool operator !=(PixelForeground left, PixelForeground right)
        {
            if ((object)left is null) return (object)right is not null;
            return !left.Equals(right);
        }
    }
}