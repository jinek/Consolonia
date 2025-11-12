using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Media;
using Consolonia.Core.Drawing;
using Consolonia.Core.Drawing.PixelBufferImplementation;


namespace Consolonia.Core.Text.Fonts
{
    /// <summary>
    /// Fonts which use the TLF/Caca format
    /// </summary>
    public class AsciiArtTypeface : IGlyphTypeface, IGlyphRunRender
    {
        // codepoint to glyph
        private Dictionary<uint, AsciiArtGlyph> _glyphs = new();

        // glyphindex to codepoint
        private List<uint> _codepoints = new();

        public AsciiArtTypeface(string familyName)
        {
            FamilyName = familyName;
            Weight = FontWeight.Normal;
            Style = FontStyle.Normal;
            Stretch = FontStretch.Normal;
            FontSimulations = FontSimulations.None;
        }

        public void AddGlyph(uint codepoint, AsciiArtGlyph glyph)
        {
            _glyphs[codepoint] = glyph;
            _codepoints.Add(codepoint);
        }

        public string FamilyName { get; init; }

        public FontWeight Weight { get; init; }

        public FontStyle Style { get; init; }

        public FontStretch Stretch { get; init; }

        public int GlyphCount => _glyphs.Count;

        public FontMetrics Metrics { get; set; }

        public FontSimulations FontSimulations { get; init; }

        public char Hardblank { get; set; } = '$';

        public void Dispose()
        {
        }

        public ushort GetGlyph(uint codepoint)
        {
            return (ushort)_codepoints.IndexOf(codepoint);
        }

        public int GetGlyphAdvance(ushort glyph)
        {
            var codepoint = _codepoints[glyph];
            var asciiGlyph = _glyphs[codepoint];
            return asciiGlyph.Width;
        }

        public int[] GetGlyphAdvances(ReadOnlySpan<ushort> glyphs)
        {
            var advances = new int[glyphs.Length];
            for (int i = 0; i < glyphs.Length; i++)
            {
                advances[i] = GetGlyphAdvance(glyphs[i]);
            }
            return advances;
        }

        public ushort[] GetGlyphs(ReadOnlySpan<uint> codepoints)
        {
            var glyphs = new ushort[codepoints.Length];
            for (int i = 0; i < codepoints.Length; i++)
            {
                glyphs[i] = GetGlyph(codepoints[i]);
            }
            return glyphs;
        }

        public bool TryGetGlyph(uint codepoint, out ushort glyph)
        {
            var index = _codepoints.IndexOf(codepoint);
            if (index >= 0)
            {
                glyph = (ushort)index;
                return true;
            }
            glyph = 0;
            return false;
        }

        public bool TryGetGlyphMetrics(ushort glyph, out GlyphMetrics metrics)
        {
            throw new NotImplementedException();
        }

        public bool TryGetTable(uint tag, out byte[] table)
        {
            table = null;
            return false;
        }

        PixelRect IGlyphRunRender.DrawGlyphRun(DrawingContextImpl context, PixelPoint position, GlyphRunImpl glyphRun, Color foreground)
        {
            var startPosition = position;
            var pos = new PixelPoint(position.X, position.Y);
            foreach (var glyphInfo in glyphRun.GlyphInfos)
            {
                AsciiArtGlyph asciiGlyph = _glyphs[_codepoints[glyphInfo.GlyphIndex]];
                foreach (var graphemeLine in asciiGlyph.GraphemeLines)
                {
                    int iChar = pos.X;
                    foreach (var grapheme in graphemeLine)
                    {
                        var symbol = new Symbol(grapheme.Glyph);
                        context.DrawPixel(new Pixel(new PixelForeground(symbol, foreground, Weight, Style)), pos.WithX(iChar));
                        iChar += symbol.Width;
                    }
                    // advance to next line
                    pos = pos.WithY(pos.Y + 1);
                }
                // advance glyph width, reset position height
                pos = pos.WithX(pos.X + asciiGlyph.Width).WithY(position.Y);
            }
            return new PixelRect(startPosition, new PixelSize(pos.X - startPosition.X, this.Metrics.DesignEmHeight));
        }
    }
}