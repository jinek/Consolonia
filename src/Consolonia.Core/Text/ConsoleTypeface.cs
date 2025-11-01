using System;
using System.Collections.Generic;
using Avalonia.Media;
using Consolonia.Controls;
using Consolonia.Core.Drawing;

namespace Consolonia.Core.Text
{
    /// <summary>
    ///     This represents a psuedo-typeface for console rendering.
    /// </summary>
    public sealed class ConsoleTypeface : IGlyphTypeface
    {
        private static readonly object GlyphCacheSync = new();
        private static readonly Dictionary<ushort, string> GlyphTextByIndex = new();
        private static readonly Dictionary<ushort, ushort> GlyphWidthByIndex = new();
        private static readonly Dictionary<string, ushort> GlyphIndexByText = new();

        public void Dispose()
        {
            lock(GlyphCacheSync)
            {
                GlyphTextByIndex.Clear();
                GlyphWidthByIndex.Clear();
                GlyphIndexByText.Clear();
            }
        }

        public bool TryGetGlyphMetrics(ushort glyph, out GlyphMetrics metrics)
        {
            lock (GlyphCacheSync)
            {
                if (GlyphWidthByIndex.TryGetValue(glyph, out ushort width))
                {
                    metrics = new GlyphMetrics
                    {
                        XBearing = 0,
                        YBearing = 0,
                        Height = 1,
                        Width = width
                    };
                    return true;
                }

                metrics = default;
                return false;
            }
        }

        public ushort GetGlyph(uint codepoint)
        {
            return GetGlyphIndex(char.ConvertFromUtf32((int)codepoint));
        }

        public bool TryGetGlyph(uint codepoint, out ushort glyph)
        {
            glyph = GetGlyphIndex(char.ConvertFromUtf32((int)codepoint));
            return true;
        }

        public ushort[] GetGlyphs(ReadOnlySpan<uint> codepoints)
        {
            ushort[] glyphs = new ushort[codepoints.Length];
            for (int i = 0; i < codepoints.Length; i++)
                glyphs[i] = GetGlyphIndex(char.ConvertFromUtf32((int)codepoints[i]));
            return glyphs;
        }

        public int GetGlyphAdvance(ushort glyph)
        {
            lock (GlyphCacheSync)
            {
                return GlyphWidthByIndex[glyph];
            }
        }

        public int[] GetGlyphAdvances(ReadOnlySpan<ushort> glyphs)
        {
            int[] advances = new int[glyphs.Length];
            lock (GlyphCacheSync)
            {
                for (int i = 0; i < glyphs.Length; i++) advances[i] = GetGlyphAdvance(glyphs[i]);
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

#pragma warning disable CA1822 // Mark members as static
        public ushort GetGlyphIndex(string glyphText)
        {
            ushort glyph;
            lock (GlyphCacheSync)
            {
                if (!GlyphIndexByText.TryGetValue(glyphText, out glyph))
                {
                    if (GlyphIndexByText.Count >= ushort.MaxValue)
                        throw new InvalidOperationException("Glyph cache overflow.");
                    glyph = (ushort)GlyphTextByIndex.Count;
                    GlyphTextByIndex[glyph] = glyphText;
                    GlyphWidthByIndex[glyph] = glyphText.MeasureText();
                    GlyphIndexByText[glyphText] = glyph;
                }
            }

            return glyph;
        }

        public string GetGlyphText(ushort glyph)
        {
            lock (GlyphCacheSync)
            {
                if (!GlyphTextByIndex.TryGetValue(glyph, out string text))
                    throw new ArgumentException($"Glyph index {glyph} not found in cache.", nameof(glyph));
                return text;
            }
        }
#pragma warning restore CA1822 // Mark members as static
    }
}