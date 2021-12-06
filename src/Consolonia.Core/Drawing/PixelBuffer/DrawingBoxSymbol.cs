using System;

namespace Consolonia.Core.Drawing.PixelBuffer
{
    /// <summary>
    /// https://en.wikipedia.org/wiki/Box-drawing_character
    /// </summary>
    public struct DrawingBoxSymbol : ISymbol
    {
        public DrawingBoxSymbol(byte upRightDownLeft)
        {
            UpRightDownLeft = upRightDownLeft;
        }

        public byte UpRightDownLeft;

        /// <summary>
        /// https://en.wikipedia.org/wiki/Code_page_437
        /// </summary>
        char ISymbol.GetCharacter()
        {
            //DOS linedraw characters are not ordered in any programmatic manner, and calculating a particular character shape needs to use a look-up table. from https://en.wikipedia.org/wiki/Box-drawing_character
            switch (UpRightDownLeft)
            {
                case 0b0000: return char.MinValue;
                case 0b1000:
                case 0b0010:
                case 0b1010:
                    return '│';
                case 0b0100:
                case 0b0001:
                case 0b0101:
                    return '─';
                case 0b1111:
                    return '┼';
                case 0b1001: return '┘';
                case 0b0011: return '┐';
                case 0b0110: return '┌';
                case 0b1100: return '└';
                case 0b1110: return '├';
                case 0b1011: return '┤';
                case 0b1101: return '┴';
                case 0b0111: return '┬';
                default: throw new IndexOutOfRangeException();
            }
        }

        public bool IsWhiteSpace()
        {
            return UpRightDownLeft == 0b0000;
        }

        public ISymbol Blend(ref ISymbol symbolAbove)
        {
            if (symbolAbove is not DrawingBoxSymbol drawingBoxSymbol) return symbolAbove;

            UpRightDownLeft |= drawingBoxSymbol.UpRightDownLeft;
            return this;
        }
    }
}