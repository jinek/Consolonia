using System;

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    /// <summary>
    ///     https://en.wikipedia.org/wiki/Box-drawing_character
    /// </summary>
    public struct DrawingBoxSymbol : ISymbol
    {
        // all 0bXXXX_0000 are special values
        private const byte BoldSymbol = 0b0001_0000;
        private const byte EmptySymbol = 0b0;

        public DrawingBoxSymbol(byte upRightDownLeft)
        {
            _upRightDownLeft = upRightDownLeft;
        }

        private byte _upRightDownLeft;

        /// <summary>
        ///     https://en.wikipedia.org/wiki/Code_page_437
        /// </summary>
        char ISymbol.GetCharacter()
        {
            //DOS linedraw characters are not ordered in any programmatic manner, and calculating a particular character shape needs to use a look-up table. from https://en.wikipedia.org/wiki/Box-drawing_character

            byte leftPart = (byte)(_upRightDownLeft & 0b1111_0000);
            bool hasLeftPart = leftPart > 0;

            switch (_upRightDownLeft & 0b0000_1111)
            {
                case 0b0000_1000:
                case 0b0000_0010:
                case 0b0000_1010:
                    return hasLeftPart ? '║' : '│';
                case 0b0000_0100:
                case 0b0000_0001:
                case 0b0000_0101:
                    return hasLeftPart ? '═' : '─';

                case 0b0000_1111:
                    if (leftPart == 0b0000_0000)
                        return '┼';
                    bool vertical = (leftPart & 0b1010_0000) > 0;
                    bool horizontal = (leftPart & 0b0101_0000) > 0;
                    if (horizontal && vertical)
                        return '╬';
                    return horizontal ? '╪' : '╫';

                default:
                {
                    switch (_upRightDownLeft)
                    {
                        case EmptySymbol: return char.MinValue;
                        case BoldSymbol: return '█';

                        case 0b0000_1001: return '┘';
                        case 0b1000_1001: return '╜';
                        case 0b0001_1001: return '╛';
                        case 0b1001_1001: return '╝';

                        case 0b0000_0011: return '┐';
                        case 0b0010_0011: return '╖';
                        case 0b0001_0011: return '╕';
                        case 0b0011_0011: return '╗';

                        case 0b0000_0110: return '┌';
                        case 0b0100_0110: return '╒';
                        case 0b0010_0110: return '╓';
                        case 0b0110_0110: return '╔';

                        case 0b0000_1100: return '└';
                        case 0b0100_1100: return '╘';
                        case 0b1000_1100: return '╙';
                        case 0b1100_1100: return '╚';

                        case 0b0000_1110: return '├';
                        case 0b1000_1110:
                        case 0b0010_1110:
                        case 0b1010_1110: return '╟';
                        case 0b0100_1110: return '╞';
                        case 0b1100_1110:
                        case 0b0110_1110:
                        case 0b1110_1110: return '╠';

                        case 0b0000_1011: return '┤';
                        case 0b1000_1011:
                        case 0b0010_1011:
                        case 0b1010_1011: return '╢';
                        case 0b0001_1011: return '╡';
                        case 0b1001_1011:
                        case 0b0011_1011:
                        case 0b1011_1011: return '╣';

                        case 0b0000_1101: return '┴';
                        case 0b1000_1101: return '╨';
                        case 0b0100_1101:
                        case 0b0001_1101:
                        case 0b0101_1101: return '╧';
                        case 0b1100_1101:
                        case 0b1001_1101:
                        case 0b1101_1101: return '╩';

                        case 0b0000_0111: return '┬';
                        case 0b0010_0111: return '╥';
                        case 0b0100_0111:
                        case 0b0001_0111:
                        case 0b0101_0111: return '╤';
                        case 0b0110_0111:
                        case 0b0011_0111:
                        case 0b0111_0111: return '╦';

                        default: throw new InvalidOperationException();
                    }
                }
            }
        }

        public bool IsWhiteSpace()
        {
            return _upRightDownLeft == EmptySymbol;
        }

        public ISymbol Blend(ref ISymbol symbolAbove)
        {
            if (symbolAbove.IsWhiteSpace()) return this;

            if (symbolAbove is not DrawingBoxSymbol drawingBoxSymbol) return symbolAbove;
            if (drawingBoxSymbol._upRightDownLeft == BoldSymbol || _upRightDownLeft == BoldSymbol)
                _upRightDownLeft = BoldSymbol;
            else
                _upRightDownLeft |= drawingBoxSymbol._upRightDownLeft;

            return this;
        }

        public static byte UpRightDownLeftFromPattern(byte pattern, LineStyle lineStyle)
        {
            if (pattern == EmptySymbol) return EmptySymbol;
            switch (lineStyle)
            {
                case LineStyle.SingleLine:
                    return pattern;
                case LineStyle.Bold:
                    return BoldSymbol;
                case LineStyle.DoubleLine:
                    byte leftPart = (byte)(pattern << 4);
                    return (byte)(leftPart | pattern);
                default:
                    throw new ArgumentOutOfRangeException(nameof(lineStyle), lineStyle, null);
            }
        }
    }
}