using System;
using System.Linq;
using Avalonia.Media;
using Avalonia.Platform;

namespace Consolonia.Core.Text
{
    internal class GlyphTypefaceImpl : IGlyphTypeface
    {
        public void Dispose()
        {
        }

        public bool TryGetGlyphMetrics(ushort glyph, out GlyphMetrics metrics)
        {
            /*todo: handle special characters here?*/
            metrics = new GlyphMetrics
            {
                XBearing = 0,
                YBearing = 0,
                Height = 1,
                Width = 1
            };
            return true;
        }

        public ushort GetGlyph(uint codepoint)
        {
            checked
            {
                return (ushort)codepoint;
            }
        }

        public bool TryGetGlyph(uint codepoint, out ushort glyph)
        {
            glyph = (ushort)codepoint;
            return true;
        }

        public ushort[] GetGlyphs(ReadOnlySpan<uint> codepoints)
        {
            checked
            {
                return codepoints.ToArray().Select(u => (ushort)u).ToArray();
            }
        }

        public int GetGlyphAdvance(ushort glyph)
        {
            return 1;
        }

        public int[] GetGlyphAdvances(ReadOnlySpan<ushort> glyphs)
        {
            return Enumerable.Repeat(1, glyphs.Length).ToArray();
        }

        public bool TryGetTable(uint tag, out byte[] table)
        {
            throw new NotImplementedException();
        }

        public string FamilyName { get; } = FontManagerImpl.GetTheOnlyFontFamilyName();
        public FontWeight Weight => FontWeight.Normal;
        public FontStyle Style => FontStyle.Normal;
        public FontStretch Stretch => FontStretch.Normal;
        public int GlyphCount => sizeof(char);

        public FontMetrics Metrics { get; } = new()
        {
            /// <summary>
            ///     https://docs.microsoft.com/en-us/typography/opentype/spec/ttch01#funits-and-the-em-square
            /// </summary>
            DesignEmHeight = 1,
            Ascent = -1, //var height = (GlyphTypeface.Descent - GlyphTypeface.Ascent + GlyphTypeface.LineGap) * Scale;
            Descent = 0,
            LineGap = 0,
            UnderlinePosition = 0,
            UnderlineThickness = 0,
            StrikethroughPosition = 0,
            StrikethroughThickness = 0,
            IsFixedPitch = true
        };

        public FontSimulations FontSimulations => FontSimulations.None;
    }
}