using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    /// <summary>
    ///     https://en.wikipedia.org/wiki/Box-drawing_character
    /// </summary>
    [DebuggerDisplay("DrawingBox {Text}")]
    [JsonConverter(typeof(SymbolConverter))]
    public readonly struct DrawingBoxSymbol : ISymbol, IEquatable<DrawingBoxSymbol>
    {
        // all 0bXXXX_0000 are special values
        private const byte BoldSymbol = 0b0001_0000;
        private const byte EmptySymbol = 0b0;

        public DrawingBoxSymbol(byte upRightDownLeft)
        {
            UpRightDownLeft = upRightDownLeft;
            Text = GetBoxSymbol(UpRightDownLeft).ToString();
        }

        public byte UpRightDownLeft { get; init; }

        public bool Equals(DrawingBoxSymbol other)
        {
            if ((object)other is null) return false;
            return UpRightDownLeft == other!.UpRightDownLeft;
        }

        [JsonIgnore] public string Text { get; init; }

        [JsonIgnore] public ushort Width { get; } = 1;

        public bool IsWhiteSpace()
        {
            return UpRightDownLeft == EmptySymbol;
        }

        public ISymbol Blend(ref ISymbol symbolAbove)
        {
            if (symbolAbove.IsWhiteSpace()) return this;

            if (symbolAbove is not DrawingBoxSymbol drawingBoxSymbol)
                return symbolAbove;

            if (drawingBoxSymbol.UpRightDownLeft == BoldSymbol || UpRightDownLeft == BoldSymbol)
                return new DrawingBoxSymbol(BoldSymbol);

            return new DrawingBoxSymbol((byte)(UpRightDownLeft | drawingBoxSymbol.UpRightDownLeft));
        }

        /// <summary>
        ///     https://en.wikipedia.org/wiki/Code_page_437
        /// </summary>
        private static char GetBoxSymbol(byte upRightDownLeft)
        {
            //DOS linedraw characters are not ordered in any programmatic manner, and calculating a particular character shape needs to use a look-up table. from https://en.wikipedia.org/wiki/Box-drawing_character

            byte leftPart = (byte)(upRightDownLeft & 0b1111_0000);
            bool hasLeftPart = leftPart > 0;

            switch (upRightDownLeft & 0b0000_1111)
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
                    return upRightDownLeft switch
                    {
                        EmptySymbol => char.MinValue,
                        BoldSymbol => '█',
                        0b0000_1001 => '┘',
                        0b1000_1001 => '╜',
                        0b0001_1001 => '╛',
                        0b1001_1001 => '╝',
                        0b0000_0011 => '┐',
                        0b0010_0011 => '╖',
                        0b0001_0011 => '╕',
                        0b0011_0011 => '╗',
                        0b0000_0110 => '┌',
                        0b0100_0110 => '╒',
                        0b0010_0110 => '╓',
                        0b0110_0110 => '╔',
                        0b0000_1100 => '└',
                        0b0100_1100 => '╘',
                        0b1000_1100 => '╙',
                        0b1100_1100 => '╚',
                        0b0000_1110 => '├',
                        0b1000_1110 or 0b0010_1110 or 0b1010_1110 => '╟',
                        0b0100_1110 => '╞',
                        0b1100_1110 or 0b0110_1110 or 0b1110_1110 => '╠',
                        0b0000_1011 => '┤',
                        0b1000_1011 or 0b0010_1011 or 0b1010_1011 => '╢',
                        0b0001_1011 => '╡',
                        0b1001_1011 or 0b0011_1011 or 0b1011_1011 => '╣',
                        0b0000_1101 => '┴',
                        0b1000_1101 => '╨',
                        0b0100_1101 or 0b0001_1101 or 0b0101_1101 => '╧',
                        0b1100_1101 or 0b1001_1101 or 0b1101_1101 => '╩',
                        0b0000_0111 => '┬',
                        0b0010_0111 => '╥',
                        0b0100_0111 or 0b0001_0111 or 0b0101_0111 => '╤',
                        0b0110_0111 or 0b0011_0111 or 0b0111_0111 => '╦',
                        _ => throw new InvalidOperationException()
                    };
                }
            }
        }

        public static DrawingBoxSymbol UpRightDownLeftFromPattern(byte pattern, LineStyle lineStyle)
        {
            if (pattern == EmptySymbol) return new DrawingBoxSymbol(EmptySymbol);
            switch (lineStyle)
            {
                case LineStyle.SingleLine:
                    return new DrawingBoxSymbol(pattern);
                case LineStyle.Bold:
                    return new DrawingBoxSymbol(BoldSymbol);
                case LineStyle.DoubleLine:
                    byte leftPart = (byte)(pattern << 4);
                    return new DrawingBoxSymbol((byte)(leftPart | pattern));
                default:
                    throw new ArgumentOutOfRangeException(nameof(lineStyle), lineStyle, null);
            }
        }

        public override bool Equals([NotNullWhen(true)] object obj)
        {
            return obj is DrawingBoxSymbol other && Equals(other);
        }

        public override int GetHashCode()
        {
            return UpRightDownLeft.GetHashCode();
        }

        public static bool operator ==(DrawingBoxSymbol left, DrawingBoxSymbol right)
        {
            if ((object)left is null) return (object)right is null;
            return left.Equals(right);
        }

        public static bool operator !=(DrawingBoxSymbol left, DrawingBoxSymbol right)
        {
            if ((object)left is null) return (object)right is not null;
            return !left!.Equals(right);
        }
    }
}