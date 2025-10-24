using System;
using System.Collections.Generic;
using Avalonia.Media;
using Consolonia.Controls;
using Consolonia.Core.Drawing;

namespace Consolonia.Core.Text
{
    public sealed class GlyphTypeface : IGlyphTypeface
    {
        private static readonly object GlyphCacheSync = new();
        private static readonly Dictionary<ushort, string> GlyphByIndex = new();
        private static readonly Dictionary<ushort, ushort> WidthByIndex = new();
        private static readonly Dictionary<string, ushort> IndexByGlyph = new();

        public void Dispose()
        {
        }

        public bool TryGetGlyphMetrics(ushort glyphIndex, out GlyphMetrics metrics)
        {
            lock (GlyphCacheSync)
            {
                var glyph = GlyphByIndex[glyphIndex];
                metrics = new GlyphMetrics
                {
                    XBearing = 0,
                    YBearing = 0,
                    Height = 1,
                    Width = WidthByIndex[glyphIndex],
                };
                return true;
            }
        }

        public ushort GetGlyph(uint codepoint)
        {
            return GetGlyphIndex(Char.ConvertFromUtf32((int)codepoint));
        }

        public bool TryGetGlyph(uint codepoint, out ushort glyphIndex)
        {
            glyphIndex = GetGlyphIndex(Char.ConvertFromUtf32((int)codepoint));
            return true;
        }

        public ushort GetGlyphIndex(string glyph)
        {
            ushort glyphIndex;
            lock (GlyphCacheSync)
            {
                if (!IndexByGlyph.TryGetValue(glyph, out glyphIndex))
                {
                    if (IndexByGlyph.Count >= ushort.MaxValue)
                        throw new InvalidOperationException("Glyph cache overflow.");
                    glyphIndex = (ushort)GlyphByIndex.Count;
                    GlyphByIndex[glyphIndex] = glyph;
                    WidthByIndex[glyphIndex] = glyph.MeasureText();
                    IndexByGlyph[glyph] = glyphIndex;
                }
            }
            return glyphIndex;
        }

        public string GetGlyphText(ushort glyphIndex)
        {
            lock (GlyphCacheSync)
            {
                return GlyphByIndex[glyphIndex];
            }
        }

        public ushort[] GetGlyphs(ReadOnlySpan<uint> codepoints)
        {
            ushort[] glyphs = new ushort[codepoints.Length];
            for (int i = 0; i < codepoints.Length; i++)
            {
                glyphs[i] = GetGlyphIndex(Char.ConvertFromUtf32((int)codepoints[i]));
            }
            return glyphs;
        }

        public int GetGlyphAdvance(ushort glyphIndex)
        {
            lock (GlyphCacheSync)
            {
                return WidthByIndex[glyphIndex];
            }
        }

        public int[] GetGlyphAdvances(ReadOnlySpan<ushort> glyphs)
        {
            int[] advances = new int[glyphs.Length];
            lock (GlyphCacheSync)
            {
                for (int i = 0; i < glyphs.Length; i++)
                {
                    advances[i] = GetGlyphAdvance(glyphs[i]);
                }
            }
            return advances;
        }

        public bool TryGetTable(uint tag, out byte[] table)
        {
            throw new NotImplementedException();
        }

        public string FamilyName { get; } = FontManagerImpl.GetTheOnlyFontFamilyName();
        public FontWeight Weight { get; init; } = FontWeight.Normal;
        public FontStyle Style { get; init; } = FontStyle.Normal;
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