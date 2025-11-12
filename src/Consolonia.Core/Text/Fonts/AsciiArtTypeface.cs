using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using Avalonia.Platform;
using Consolonia.Controls;
using Consolonia.Core.Drawing;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Consolonia.Core.Helpers;


namespace Consolonia.Core.Text.Fonts
{
    /// <summary>
    /// Fonts which use the TLF/Caca format
    /// </summary>
    public class AsciiArtTypeface : IGlyphTypeface, ITextShaperImpl, IGlyphRunRender
    {
        // codepoint to glyph
        private Dictionary<uint, AsciiArtGlyph> _glyphs = new();

        // glyphindex to codepoint
        private List<uint> _codepoints = new();
        private bool _disposedValue;

        public AsciiArtTypeface(string familyName)
        {
            FamilyName = familyName;
            Weight = FontWeight.Normal;
            Style = FontStyle.Normal;
            Stretch = FontStretch.Normal;
            FontSimulations = FontSimulations.None;
        }

        public ushort AddGlyph(uint codepoint, AsciiArtGlyph glyph)
        {
            if (codepoint == ' ')
            {
                // generate a TAB glyph based on space advance
                var tabCodepoint = (uint)'\t';
                var advance = "\t".MeasureText();
                var tabGlyph = new AsciiArtGlyph(glyph.Typeface, tabCodepoint, glyph.Lines.Select(line =>
                {
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < advance; i++)
                        sb.Append(line);
                    return sb.ToString();
                }).ToArray());
                _glyphs[tabCodepoint] = tabGlyph;
                _codepoints.Add(tabCodepoint);
            }
            _glyphs[codepoint] = glyph;
            _codepoints.Add(codepoint);
            return (ushort)_codepoints.IndexOf(codepoint);
        }

        public string FamilyName { get; init; }

        public FontWeight Weight { get; init; }

        public FontStyle Style { get; init; }

        public FontStretch Stretch { get; init; }

        public int GlyphCount => _glyphs.Count;

        public FontMetrics Metrics { get; set; }

        public FontSimulations FontSimulations { get; init; }

        public char Hardblank { get; set; } = '$';

        public ushort GetGlyph(uint codepoint)
        {
            // unknown code points are (ushort)-1 => ushort.MaxValue
            return (ushort)_codepoints.IndexOf(codepoint);
        }

        public int GetGlyphAdvance(ushort glyph)
        {
            if (glyph == ushort.MaxValue)
                return 0;
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
            if (index < 0)
            {
                // THIN SPACE 0x2009 used as codepoint for unknown glyphs
                index = _codepoints.IndexOf(0x2009); 
                if (index < 0)
                {
                    index = AddGlyph(0x2009, new AsciiArtGlyph(this, 0x2009, Enumerable.Repeat(" ", Metrics.DesignEmHeight).ToArray()));
                }
            }
            glyph = (ushort)index;
            return true;
        }

        public bool TryGetGlyphMetrics(ushort glyph, out GlyphMetrics metrics)
        {
            metrics = new GlyphMetrics()
            {
                XBearing = 0,
                YBearing = 0,
                Height = Metrics.DesignEmHeight,
                Width = GetGlyphAdvance(glyph)
            };
            return true;
        }

        public bool TryGetTable(uint tag, out byte[] table)
        {
            table = null;
            return false;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~AsciiArtTypeface()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public ShapedBuffer ShapeText(ReadOnlyMemory<char> text, TextShaperOptions options)
        {
            var graphemes = Grapheme.Parse(text.Span.ToString(), false);

            var shapedBuffer = new ShapedBuffer(text, graphemes.Count, this, 1, 0 /*todo: must be 1 for right to left?*/);

            for (ushort i = 0; i < shapedBuffer.Length; i++)
            {
                Grapheme grapheme = graphemes[i];
                ushort glyphIndex;

                if (!TryGetGlyph((uint)grapheme.Glyph[0], out glyphIndex))
                {
                }
                int glyphAdvance = GetGlyphAdvance(glyphIndex);
                shapedBuffer[i] = new GlyphInfo(glyphIndex, i, glyphAdvance);
            }
            return shapedBuffer;
        }

        PixelRect IGlyphRunRender.DrawGlyphRun(DrawingContextImpl context, PixelPoint position, GlyphRunImpl glyphRun, Color foreground)
        {
            var startPosition = position;
            var pos = new PixelPoint(position.X, position.Y);
            foreach (var glyphInfo in glyphRun.GlyphInfos.Where(gi => gi.GlyphIndex != ushort.MaxValue))
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