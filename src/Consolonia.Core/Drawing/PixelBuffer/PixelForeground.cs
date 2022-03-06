using System;

namespace Consolonia.Core.Drawing.PixelBuffer
{
    public struct PixelForeground
    {
        public PixelForeground(ISymbol symbol, ConsoleColor color = ConsoleColor.White)
        {
            Symbol = symbol;
            Color = color;
        }

        public readonly ISymbol Symbol; //now wokring with 16 bit unicode only for simplicity //todo: reference here
        public readonly ConsoleColor Color;

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