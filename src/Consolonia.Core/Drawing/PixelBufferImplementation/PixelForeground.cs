using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Media;

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    [DebuggerDisplay("'{Symbol.Text}' [{Color}]")]
    public readonly struct PixelForeground : IEquatable<PixelForeground>
    {
        public PixelForeground(ISymbol symbol, Color color,
            FontWeight weight = FontWeight.Normal, FontStyle style = FontStyle.Normal,
            TextDecorationCollection textDecorations = null)
        {
            ArgumentNullException.ThrowIfNull(symbol);
            Symbol = symbol;
            Color = color;
            Weight = weight;
            Style = style;
            TextDecorations = textDecorations;
        }

        public ISymbol Symbol { get; } //now working with 16 bit unicode only for simplicity //todo: reference here

        public Color Color { get; }

        public FontWeight Weight { get; }

        public FontStyle Style { get; }

        public TextDecorationCollection TextDecorations { get; }

        public PixelForeground Shade()
        {
            Color newColor = Color.Shade();
            return new PixelForeground(Symbol, newColor, Weight, Style, TextDecorations);
        }

        public PixelForeground Blend(PixelForeground pixelAboveForeground)
        {
            //todo: check default(char) is there
            ISymbol symbolAbove = pixelAboveForeground.Symbol;
            ArgumentNullException.ThrowIfNull(symbolAbove);

            ISymbol newSymbol = Symbol.Blend(ref symbolAbove);

            return new PixelForeground(newSymbol, pixelAboveForeground.Color, pixelAboveForeground.Weight,
                pixelAboveForeground.Style, pixelAboveForeground.TextDecorations);
        }

        public bool Equals(PixelForeground other)
            => Symbol.Equals(other.Symbol) &&
                Color.Equals(other.Color) &&
                Weight == other.Weight &&
                Style == other.Style &&
                Equals(TextDecorations, other.TextDecorations);

        public override bool Equals([NotNullWhen(true)] object obj)
            => obj is PixelForeground other && this.Equals(other);

        public override int GetHashCode()
          => HashCode.Combine(Symbol, Color, (int)Weight, (int)Style, TextDecorations);

        public static bool operator ==(PixelForeground left, PixelForeground right)
            => left.Equals(right);

        public static bool operator !=(PixelForeground left, PixelForeground right)
            => !left.Equals(right);
    }
}