using System;
using System.Diagnostics;
using Avalonia.Media;

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    [DebuggerDisplay("'{Symbol.Text}' [{Color}]")]
    public readonly struct PixelForeground
    {
        public PixelForeground(ISymbol symbol, Color color,
            FontWeight weight = FontWeight.Normal, FontStyle style = FontStyle.Normal, TextDecorationCollection textDecorations = null)
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
    }
}