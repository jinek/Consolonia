using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Avalonia.Media;
using Avalonia.Controls;

#if DEBUG
using System.Diagnostics;
#endif

namespace Consolonia.Core.Text.Fonts
{
#pragma warning disable CA1310 // Specify StringComparison for correctness

    /// <summary>
    /// FIGlet font parser and renderer
    /// Handles TLF (Caca) fonts with tlf2a header
    /// </summary>
    public static class AsciiArtTypefaceLoader
    {
        private readonly static uint[] Codepoints = [
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
            // Parse header: tlf2a{hardblank} height baseline maxLength oldLayout commentLines
            var header = lines[0];

            bool isFiglet = header.StartsWith("flf2a", StringComparison.Ordinal);
            bool isCaca = header.StartsWith("tlf2a", StringComparison.Ordinal);
            if (!isFiglet && !isCaca)
                throw new InvalidDataException($"Invalid font header - must start with 'flf2a' or 'tlf2a', got: '{header.Substring(0, Math.Min(10, header.Length))}'");

            if (header.Length < 7)
                throw new InvalidDataException("Invalid font header - too short");

            // 6th character is the hardblank
            typeface.Hardblank = header[5];

            var parts = header[6..].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 1)
                throw new InvalidDataException("Invalid FIGlet header - missing height parameter");

            // ReSharper disable UnusedVariable
            var height = int.Parse(parts[0]); 
            var baseline = parts.Length > 1 ? int.Parse(parts[1]) : 0;
            var width = parts.Length > 2 ? int.Parse(parts[2]) : 0;
            var oldLayout = parts.Length > 3 ? int.Parse(parts[3]) : 0;
            OldLayoutMode oldLayoutMode = (OldLayoutMode)oldLayout;
            var commentLines = parts.Length > 4 ? int.Parse(parts[4]) : 0;
            var printDirection = parts.Length > 5 ? int.Parse(parts[5]) : 0;
            var layout = parts.Length > 6 ? int.Parse(parts[6]) : -1;
            var codeTagCount = parts.Length > 7 ? int.Parse(parts[7]) : 0;
            if (isFiglet)
            {
                if (layout >= 0)
                {
                    typeface.LayoutMode = (LayoutMode)layout;
                }
                else
                {
                    typeface.LayoutMode = LayoutMode.Kern;
                    if (oldLayoutMode.HasFlag(OldLayoutMode.HorizontalKerning))
                        typeface.LayoutMode |= LayoutMode.Kern;
                    if (oldLayoutMode.HasFlag(OldLayoutMode.HorizontalFitting))
                        typeface.LayoutMode |= LayoutMode.Smush | LayoutMode.Equal;
                }
            }
            else
            {
                if (layout >= 0)
                {
                    typeface.LayoutMode = (LayoutMode)layout;
                }
                else
                {
                    typeface.LayoutMode = LayoutMode.Smush;
                    if (oldLayoutMode.HasFlag(OldLayoutMode.HorizontalKerning))
                        typeface.LayoutMode |= LayoutMode.Kern;
                    if (oldLayoutMode.HasFlag(OldLayoutMode.HorizontalFitting))
                        typeface.LayoutMode |= LayoutMode.Smush | LayoutMode.Equal;
                }
            }

#if DEBUG // diagnostic output of parsing. Only in DEBUG builds.
            Debug.WriteLine($"Font '{name}': height={height}, baseline={baseline}, width={width}, oldLayout={oldLayoutMode} (legacy) layoutMode={typeface.LayoutMode}");
            // Debug output for modern layout mode
            if (oldLayoutMode >= 0)
            {
                Debug.Write("  OldLayoutMode:");
                foreach (var flag in Enum.GetValues<OldLayoutMode>())
                {
                    if (flag != OldLayoutMode.None && oldLayoutMode.HasFlag(flag))
                    {
                        Debug.Write($" {flag}");
                    }
                }
                Debug.WriteLine("");
            }

            // Debug output for modern layout mode
            if (typeface.LayoutMode > 0)
            {
                Debug.Write("  LayoutMode:");
                foreach (var flag in Enum.GetValues<LayoutMode>())
                {
                    if (flag != LayoutMode.None && typeface.LayoutMode.HasFlag(flag))
                    {
                        Debug.Write($" {flag}");
                    }
                }
                Debug.WriteLine("");
            }
#endif
            // ReSharper enable UnusedVariable
            int currentLine = 1 + commentLines;


            // Load standard ASCII characters (32-126)+ extended characters
            foreach (var codepoint in Codepoints)
            {
                var charLines = new string[height];
                for (int i = 0; i < height; i++)
                {
                    if (currentLine >= lines.Length)
                        throw new InvalidDataException($"Unexpected end of file while loading glyph for codepoint {codepoint}");

                    var line = lines[currentLine++].TrimEnd();
                    charLines[i] = ProcessLine(line.Trim(line[^1]));
                }
                typeface.AddGlyph(codepoint, new AsciiArtGlyph(typeface, codepoint, charLines));
                if (currentLine >= lines.Length)
                    break;
            }

            // Load extended characters if available
            uint extendedCodepoint = 0;

            while (currentLine < lines.Length)
            {
                var codeLine = lines[currentLine].TrimEnd();
                if (string.IsNullOrEmpty(codeLine))
                {
                    currentLine++;
                    continue;
                }
                var codeTokens = codeLine.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (codeTokens.Length == 0)
                {
                    currentLine++;
                    continue;
                }

                var codeToken = codeTokens[0];
                if (codeToken.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                {
                    extendedCodepoint = Convert.ToUInt32(codeToken, 16);
                    currentLine++;
                    continue;
                }
                else if (codeToken.Length > 0 && Char.IsNumber(codeToken[0]))
                {
                    extendedCodepoint = Convert.ToUInt32(codeToken);
                    currentLine++;
                }
                
                var charLines = new string[height];
                for (int i = 0; i < height; i++)
                {
                    if (currentLine >= lines.Length)
                        break;

                    var line = lines[currentLine++].TrimEnd();
                    charLines[i] = ProcessLine(line.Trim(line[^1]));
                }
                typeface.AddGlyph(extendedCodepoint, new AsciiArtGlyph(typeface, extendedCodepoint, charLines));
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

        public static string ProcessLine(string input)
        {
            return AnsiRegex.Replace(input, string.Empty);
        }
    }
}