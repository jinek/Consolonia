using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Consolonia.Controls;
using Consolonia.Core.Helpers;


namespace Consolonia.Core.Text.Fonts
{
    public class AsciiArtGlyph
    {
        public AsciiArtGlyph(AsciiArtTypeface typeface, uint codepoint, string[] lines)
        {
            Typeface = typeface;
            Codepoint = codepoint;
            Lines = lines;
            Ends = new int[Lines.Length];
            Starts = new int[Lines.Length];
            GraphemeLines = new Grapheme[lines.Length][];

            for (int iLine = 0; iLine < Lines.Length; iLine++)
            {
                var line = EncodeUnicode(Lines[iLine].Replace(typeface.Hardblank, ' '));
                GraphemeLines[iLine] = Grapheme.Parse(line, false).ToArray();
                var width = (byte)line.MeasureText();
                if (width > Width)
                    Width = width;
                
                for (var iStart = 0; iStart < line.Length; iStart++)
                {
                    if (line[iStart] != ' ')
                    {
                        Starts[iLine] = iStart;
                        break;
                    }
                }
                for (var iEnd = line.Length - 1; iEnd >= 0; iEnd--)
                {
                    if (line[iEnd] != ' ')
                    {
                        Ends[iLine] = iEnd;
                        break;
                    }
                }
            }
        }

        private static readonly Regex UnicodeEscapeRegex = new(@"\\u([0-9A-Fa-f]{4})", RegexOptions.Compiled);

        private static string EncodeUnicode(string text)
        {
            return UnicodeEscapeRegex.Replace(text, match =>
            {
                int codePoint = Convert.ToInt32(match.Groups[1].Value, 16);
                return new string((char)codePoint, 1);
            });
        }

        public AsciiArtTypeface Typeface { get; set; }

        public byte Width { get; init; }

        public byte Height => (byte)Lines.Length;

        public uint Codepoint { get; init; }

#pragma warning disable CA1819 // Properties should not return arrays
        public Grapheme[][] GraphemeLines { get; init; }

        public string[] Lines { get; init; }

        public int[] Ends { get; set; }

        public int[] Starts { get; set; }
#pragma warning restore CA1819 // Properties should not return arrays
    }
}