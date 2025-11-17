using System;
using System.Linq;
using System.Text.RegularExpressions;
using Consolonia.Controls;
using Consolonia.Core.Helpers;

namespace Consolonia.Core.Text.Fonts
{
    public class AsciiArtGlyph
    {
        private static readonly Regex UnicodeEscapeRegex = new(@"\\u([0-9A-Fa-f]{4})", RegexOptions.Compiled);

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
                string line = EncodeUnicode(Lines[iLine].Replace(typeface.Hardblank, ' '));
                GraphemeLines[iLine] = Grapheme.Parse(line, false).ToArray();
                byte width = (byte)line.MeasureText();
                if (width > Width)
                    Width = width;
                Starts[iLine] = line.Length;
                for (int iStart = 0; iStart < line.Length; iStart++)
                    if (line[iStart] != ' ')
                    {
                        Starts[iLine] = iStart;
                        break;
                    }

                Ends[iLine] = 0;
                for (int iEnd = line.Length - 1; iEnd >= 0; iEnd--)
                    if (line[iEnd] != ' ')
                    {
                        Ends[iLine] = iEnd;
                        break;
                    }
            }
#if DEBUG_FONT_GLYPH
            Debug.WriteLine("==============================");
            Debug.WriteLine($"NEW GLYPH: U+{codepoint:X4} ({char.ConvertFromUtf32((int)codepoint)})");
            foreach (var graphemeLine in GraphemeLines)
            {
                foreach(var grapheme in graphemeLine)
                {
                    Debug.Write(grapheme.Glyph);
                }
                Debug.WriteLine("");
            }
#endif
        }

        public AsciiArtTypeface Typeface { get; set; }

        public byte Width { get; init; }

        public byte Height => (byte)Lines.Length;

        public uint Codepoint { get; init; }

        private static string EncodeUnicode(string text)
        {
            return UnicodeEscapeRegex.Replace(text, match =>
            {
                int codePoint = Convert.ToInt32(match.Groups[1].Value, 16);
                return new string((char)codePoint, 1);
            });
        }

#pragma warning disable CA1819 // Properties should not return arrays
        public Grapheme[][] GraphemeLines { get; init; }

        public string[] Lines { get; init; }

        public int[] Ends { get; set; }

        public int[] Starts { get; set; }
#pragma warning restore CA1819 // Properties should not return arrays
    }
}