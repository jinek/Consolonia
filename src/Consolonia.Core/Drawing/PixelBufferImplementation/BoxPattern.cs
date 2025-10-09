using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    /// <summary>
    ///     https://en.wikipedia.org/wiki/Box-drawing_character
    /// </summary>
    public static class BoxPattern
    {
        public const char Min = '─'; //0x2500
        public const char Max = '╵'; //0x257F

        // all 0bXXXX_0000 are special values
        public const char BoldChar = '█';
        public const byte BoldPattern = 0b0001_0000;
        public const char EmptyChar = char.MinValue;
        public const byte EmptyPattern = 0b0;

        /// <summary>
        ///     https://en.wikipedia.org/wiki/Code_page_437
        /// </summary>
        public static char GetBoxChar(byte upRightDownLeft)
        {
            //DOS line draw characters are not ordered in any programmatic manner, and calculating a particular character shape needs to use a look-up table. from https://en.wikipedia.org/wiki/Box-drawing_character

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
                            EmptyPattern => char.MinValue,
                            BoldPattern => '█',
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
                            _ => throw new InvalidOperationException(GetMaskText(upRightDownLeft))
                        };
                    }
            }
        }

        /// <summary>
        ///     Merge 2 patterns together. Aka, Left + Top => TopLeft
        /// </summary>
        /// <param name="boxPattern"></param>
        /// <param name="overlayPattern"></param>
        /// <returns></returns>
        public static byte Merge(byte boxPattern, byte overlayPattern)
        {
            if (overlayPattern == EmptyPattern)
                return boxPattern;

            if (overlayPattern == BoldPattern || boxPattern == BoldPattern)
                return BoldPattern;

            return (byte)(boxPattern | overlayPattern);
        }

        /// <summary>
        /// This will output human readable strings for box pattern 
        /// </summary>
        /// <remarks>This is only called by debugger visualizer</remarks>
        /// <param name="mask">mask for '╟'</param>
        /// <returns>"DU,DD,SR" meaning Double Up, Double Down, Single Right</returns>
        public static string GetMaskText(byte mask)
        {
            if (mask == EmptyPattern)
                return String.Empty;
            var segments = GetSubMaskSegments("D", (byte)(mask >> 4))
                .Concat(GetSubMaskSegments("S", (byte)(mask & 0b0000_1111)));
            return String.Join(',', segments);
        }

        private static IEnumerable<string> GetSubMaskSegments(string prefix, byte mask)
        {
            List<string> segments = new List<string>();

            if ((mask & 0b1000) == 0b1000)
                segments.Add($"{prefix}U");
            if ((mask & 0b0100) == 0b0100)
                segments.Add($"{prefix}R");
            if ((mask & 0b0010) == 0b0010)
                segments.Add($"{prefix}D");
            if ((mask & 0b0001) == 0b0001)
                segments.Add($"{prefix}L");

            return segments;
        }
    }
}