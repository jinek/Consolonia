using System.Text;
using System.IO.Compression;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using Avalonia.Media;

namespace Consolonia.Core.Text.Fonts
{

#pragma warning disable CA1310 // Specify StringComparison for correctness
    /// <summary>
    /// FIGlet font parser and renderer
    /// Handles FLF (FIGlet) fonts with flf2a header
    /// </summary>
    public static class FigletTypefaceLoader
    {
        private static uint[] _codepoints = [
            32, 33, 34, 35, 36, 37, 38, 39, 40, 41,
            42, 43, 44, 45, 46, 47, 48, 49, 50, 51,
            52, 53, 54, 55, 56, 57, 58, 59, 60, 61,
            62, 63, 64, 65, 66, 67, 68, 69, 70, 71,
            72, 73, 74, 75, 76, 77, 78, 79, 80, 81,
            82, 83, 84, 85, 86, 87, 88, 89, 90, 91,
            92, 93, 94, 95, 96, 97, 98, 99, 100, 101,
            102, 103, 104, 105, 106, 107, 108, 109, 110, 111,
            112, 113, 114, 115, 116, 117, 118, 119, 120, 121,
            122, 123, 124, 125, 126,
            196, 214, 220, 228, 246, 252, 223
        ];

        public static AsciiArtTypeface Load(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                var fontName = Path.GetFileNameWithoutExtension(path);
                return Load(stream, fontName);
            }
        }

        public static AsciiArtTypeface Load(Stream stream, string name)
        {
            var typeface = new AsciiArtTypeface(name);

            string[] lines = AsciiArtUtilities.ReadFontStream(stream);

            if (lines.Length == 0)
                throw new InvalidDataException("Empty font file");

            // Parse header: flf2a{hardblank} height baseline maxLength oldLayout commentLines [printDirection [fullLayout [codeTagCount]]]
            var header = lines[0];

            if (!header.StartsWith("flf2a"))
                throw new InvalidDataException($"Invalid FIGlet header - must start with 'flf2a', got: '{header.Substring(0, Math.Min(10, header.Length))}'");

            if (header.Length < 7)
                throw new InvalidDataException("Invalid FIGlet header - too short");

            typeface.Hardblank = header[5];

            var parametersStart = 6;
            while (parametersStart < header.Length && header[parametersStart] == ' ')
                parametersStart++;

            var paramsString = header.Substring(parametersStart);
            var parts = paramsString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 1)
                throw new InvalidDataException("Invalid FIGlet header - missing height parameter");

            var height = int.Parse(parts[0]);
            var baseline = parts.Length > 1 ? int.Parse(parts[1]) : 0;
            var maxLength = parts.Length > 2 ? int.Parse(parts[2]) : 0;
            var oldLayout = parts.Length > 3 ? int.Parse(parts[3]) : 0;
            var commentLines = parts.Length > 4 ? int.Parse(parts[4]) : 0;
            var printDirection = parts.Length > 5 ? int.Parse(parts[5]) : 0;
            var fullLayout = parts.Length > 6 ? int.Parse(parts[6]) : 0;
            var codeTagCount = parts.Length > 7 ? int.Parse(parts[7]) : 0;

            int currentLine = 1 + commentLines;

            // Detect endmark character from first character line
            char endmarkChar = '@';
            if (currentLine < lines.Length && lines[currentLine].Length > 0)
            {
                endmarkChar = lines[currentLine][lines[currentLine].Length - 1];
            }

            // Load standard ASCII characters (32-126)+ extended characters
            foreach (var codepoint in _codepoints)
            {
                var charLines = new string[height];
                for (int i = 0; i < height; i++)
                {
                    if (currentLine >= lines.Length)
                        break;

                    var line = lines[currentLine++];
                    charLines[i] = RemoveEndmarks(line, endmarkChar, i == height - 1);
                }
                typeface.AddGlyph(codepoint, new AsciiArtGlyph(codepoint, typeface.Hardblank, charLines));
            }

            // Load extended characters if available
            uint charCode = 224;
            while (currentLine < lines.Length)
            {
                var codeLine = lines[currentLine].TrimEnd();
                if (string.IsNullOrEmpty(codeLine))
                {
                    currentLine++;
                    continue;
                }
                if (!codeLine.EndsWith("@"))
                {
                    var codeTokens = codeLine.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (codeTokens.Length == 0)
                    {
                        currentLine++;
                        continue;
                    }

                    var codeToken = codeTokens[0];
                    if (codeToken.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                    {
                        charCode = Convert.ToUInt32(codeToken, 16);
                        currentLine++;
                        continue;
                    }
                    else if (codeToken.Length > 0 && Char.IsNumber(codeToken[0]))
                    {
                        charCode = Convert.ToUInt32(codeToken);
                        currentLine++;
                        continue;
                    }
                }

                var charLines = new string[height];
                for (int i = 0; i < height && currentLine < lines.Length; i++)
                {
                    var line = lines[currentLine++];
                    charLines[i] = RemoveEndmarks(line, endmarkChar, i == height - 1);
                }
                
                if (charCode > 0)
                    typeface.AddGlyph(charCode, new AsciiArtGlyph(charCode, typeface.Hardblank, charLines));
            }
            typeface.Metrics = new FontMetrics
            {
                DesignEmHeight = (short)height,
                IsFixedPitch = false,
                StrikethroughPosition = (short)(height / 2),
                StrikethroughThickness = 1,
                UnderlinePosition = (short)(height - 2),
                UnderlineThickness = 1,
                Ascent = 0, // height-baseline,
                Descent = height,
                LineGap = 0,
            };
            return typeface;
        }

        private static string RemoveEndmarks(string line, char endmarkChar, bool isLastLine)
        {
            if (string.IsNullOrEmpty(line))
                return line;

            line = line.TrimEnd();

            if (line.Length == 0)
                return line;

            if (isLastLine)
            {
                // Last line has doubled endmark
                if (line.Length >= 2 && line[line.Length - 1] == line[line.Length - 2])
                {
                    return line.Substring(0, line.Length - 2);
                }
                return line.Substring(0, line.Length - 1);
            }
            else
            {
                // Non-last lines have single endmark
                return line.Substring(0, line.Length - 1);
            }
        }
    }
}