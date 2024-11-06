using System;
using System.Diagnostics;
using Avalonia.Media;

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    [DebuggerDisplay("'{Symbol.Rune}' [{Color}]")]
    public readonly struct PixelForeground
    {
        public PixelForeground(ISymbol symbol, FontWeight weight = FontWeight.Normal,
            FontStyle style = FontStyle.Normal, TextDecorationCollection textDecorations = null, Color? color = null)
        {
            ArgumentNullException.ThrowIfNull(symbol);
            Symbol = symbol;
            Color = color ?? Colors.White;
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
            return new PixelForeground(Symbol, Weight, Style, TextDecorations, newColor);
        }

        public PixelForeground Blend(PixelForeground pixelAboveForeground)
        {
            //todo: check default(char) is there
            ISymbol symbolAbove = pixelAboveForeground.Symbol;
            ArgumentNullException.ThrowIfNull(symbolAbove);

            ISymbol newSymbol = Symbol.Blend(ref symbolAbove);

            return new PixelForeground(newSymbol, pixelAboveForeground.Weight, pixelAboveForeground.Style,
                pixelAboveForeground.TextDecorations, pixelAboveForeground.Color);
        }
    }
}