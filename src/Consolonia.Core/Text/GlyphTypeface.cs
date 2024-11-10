using System;
using System.Linq;
using Avalonia.Media;
using Consolonia.Core.Drawing;

namespace Consolonia.Core.Text
{
    public sealed class GlyphTypeface : IGlyphTypeface
    {
        // NOTE: We are using this placeholder glyph since we are pushing
        // raw text to the console and not using a font system to render the text
        internal const ushort Glyph = 21; // ASCII NAK

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
            return Glyph;
        }

        public bool TryGetGlyph(uint codepoint, out ushort glyph)
        {
            glyph = Glyph;
            return true;
        }

        public ushort[] GetGlyphs(ReadOnlySpan<uint> codepoints)
        {
            return Enumerable.Repeat(Glyph, codepoints.Length).ToArray();
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
        public FontWeight Weight { get; set; } = FontWeight.Normal;
        public FontStyle Style { get; set; } = FontStyle.Normal;
        public FontStretch Stretch => FontStretch.Normal;
        public int GlyphCount => char.MaxValue;

        public FontMetrics Metrics { get; } = new()
        {
            // https://docs.microsoft.com/en-us/typography/opentype/spec/ttch01#funits-and-the-em-square
            DesignEmHeight = 1,
            Ascent = -1, //var height = (GlyphTypeface.Descent - GlyphTypeface.Ascent + GlyphTypeface.LineGap) * Scale; //todo: need to consult Avalonia guys
            Descent = 0,
            LineGap = 0,
            UnderlinePosition = -1,
            UnderlineThickness = DrawingContextImpl.UnderlineThickness,
            StrikethroughPosition = -1,
            StrikethroughThickness = DrawingContextImpl.StrikethroughThickness,
            IsFixedPitch = true
        };

        public FontSimulations FontSimulations => FontSimulations.None;
    }
}