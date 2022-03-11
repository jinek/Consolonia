using System;

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    public readonly struct PixelForeground
    {
        public PixelForeground(ISymbol symbol, ConsoleColor color = ConsoleColor.White)
        {
            Symbol = symbol;
            Color = color;
        }

        public ISymbol Symbol { get; } //now working with 16 bit unicode only for simplicity //todo: reference here
        public ConsoleColor Color { get; }

        public PixelForeground Shade()
        {
            ConsoleColor newColor = Color.Shade();
            return new PixelForeground(Symbol, newColor);
        }

        public PixelForeground Blend(PixelForeground pixelAboveForeground)
        {
            //todo: check default(char) is there
            ISymbol symbolAbove = pixelAboveForeground.Symbol;

            if (symbolAbove.IsWhiteSpace()) return this;

            ConsoleColor newColor = pixelAboveForeground.Color;
            ISymbol newSymbol = Symbol.Blend(ref symbolAbove);
            return new PixelForeground(newSymbol, newColor);
        }
    }
}