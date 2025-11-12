using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Avalonia.Media;

namespace Consolonia.Core.Text.Fonts
{
#pragma warning disable CA1310 // Specify StringComparison for correctness

    /// <summary>
    /// FIGlet font parser and renderer
    /// Handles TLF (Caca) fonts with tlf2a header
    /// </summary>
    public static class CacaTypefaceLoader
    {
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
            // Parse header: tlf2a{hardblank} height baseline maxLength oldLayout commentLines
            var header = lines[0];

            if (!header.StartsWith("tlf2a", StringComparison.Ordinal))
                throw new InvalidDataException($"Invalid TLF header - must start with 'tlf2a', got: '{header.Substring(0, Math.Min(10, header.Length))}'");

            if (header.Length < 7)
                throw new InvalidDataException("Invalid TLF header - too short");

            typeface.Hardblank = header[5];

            var parametersStart = 6;
            while (parametersStart < header.Length && header[parametersStart] == ' ')
                parametersStart++;

            var paramsString = header.Substring(parametersStart);
            var parts = paramsString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 1)
                throw new InvalidDataException("Invalid TLF header - missing height parameter");

            var height = int.Parse(parts[0]);
            int commentLines = parts.Length > 4 ? int.Parse(parts[4]) : 0;

            // Skip comment lines
            int currentLine = 1 + commentLines;

            // Parse glyphs - they are positional starting from ASCII 32 (space)
            char asciiCode = (char)32;

            var glyphLines = new List<string>();
            while (currentLine < lines.Length)
            {
                glyphLines.Clear();
                var line = lines[currentLine].TrimEnd();
                if (line.StartsWith("0x") && Int32.TryParse(line[2..6], NumberStyles.HexNumber, provider: null, result: out var codePoint))
                {
                    // explicit codepoint in format like this:
                    // 0x3131 ㄱ HANGUL LETTER KIYEOK
                    asciiCode = (char)codePoint;
                    currentLine++;
                    continue;
                }

                if (asciiCode != '@' && (!line.EndsWith('@') && !line.EndsWith(asciiCode)))
                {
                    // sometimes they skip, which you can tell because last char is asciiCode
                    asciiCode = line[^1];
                }

                // Read height lines for this glyph 
                for (int iLine = 0; iLine < height && currentLine < lines.Length; iLine++, currentLine++)
                {
                    line = lines[currentLine].TrimEnd();
                    glyphLines.Add(StripAnsi(line.TrimEnd(line[^1])));
                }

                // Store the glyph if we got any lines
                typeface.AddGlyph(asciiCode, new AsciiArtGlyph(asciiCode, typeface.Hardblank, glyphLines.ToArray()));
                asciiCode++;
            }
            typeface.Metrics = new FontMetrics
            {
                DesignEmHeight = (short)height,
                IsFixedPitch = false,
                StrikethroughPosition = (short)(height / 2),
                StrikethroughThickness = 1,
                UnderlinePosition = (short)(height - 2),
                UnderlineThickness = 1,
                Ascent = 0,
                Descent = height,
                LineGap = 0,
            };

            return typeface;
        }

        private static readonly Regex AnsiRegex = new Regex(@"\x1B\[[0-9;]*[A-Za-z]", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public static string StripAnsi(string input)
        {
            return AnsiRegex.Replace(input, string.Empty);
        }
    }
}