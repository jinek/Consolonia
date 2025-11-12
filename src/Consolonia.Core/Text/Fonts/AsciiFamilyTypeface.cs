using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using Avalonia.Platform;
using Consolonia.Core.Drawing;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using HarfBuzzSharp;


namespace Consolonia.Core.Text.Fonts
{

    /// <summary>
    /// Typeface which is made up of multiple design em height typefaces
    /// </summary>
    public class AsciiFamilyTypeface : IGlyphTypeface, ITextShaperImpl, IGlyphRunRender
    {
        private Dictionary<int, IGlyphTypeface> _typefaces = new Dictionary<int, IGlyphTypeface>();
        private bool _disposedValue;

        public AsciiFamilyTypeface(string name)
        {
            FamilyName = name;
        }

        public void AddTypeface(IGlyphTypeface typeface)
        {
            _typefaces[typeface.Metrics.DesignEmHeight] = typeface;
        }

        public IGlyphTypeface GetTypeface(int designEmHeight)
        {
            if (_typefaces.ContainsKey(designEmHeight))
            {
                return _typefaces[designEmHeight];
            }
            //find closest
            int closest = _typefaces.Keys.OrderBy(k => Math.Abs(k - designEmHeight)).First();
            return _typefaces[closest];
        }

        public IGlyphTypeface PrimaryTypeface => _typefaces[_typefaces.Keys.Max()];

        public string FamilyName { get; init; }

        public FontWeight Weight => PrimaryTypeface.Weight;

        public FontStyle Style => PrimaryTypeface.Style;

        public FontStretch Stretch => PrimaryTypeface.Stretch;

        public int GlyphCount => PrimaryTypeface.GlyphCount;

        public FontMetrics Metrics => PrimaryTypeface.Metrics;

        public FontSimulations FontSimulations => FontSimulations.None;

        public ushort GetGlyph(uint codepoint)
        {
            return PrimaryTypeface.GetGlyph(codepoint);
        }

        public int GetGlyphAdvance(ushort glyph)
        {
            return PrimaryTypeface.GetGlyphAdvance(glyph);  
        }

        public int[] GetGlyphAdvances(ReadOnlySpan<ushort> glyphs)
        {
            return PrimaryTypeface.GetGlyphAdvances(glyphs);
        }

        public ushort[] GetGlyphs(ReadOnlySpan<uint> codepoints)
        {
            return PrimaryTypeface.GetGlyphs(codepoints);   
        }

        public bool TryGetGlyph(uint codepoint, out ushort glyph)
        {
            return PrimaryTypeface.TryGetGlyph(codepoint, out glyph);
        }

        public bool TryGetGlyphMetrics(ushort glyph, out GlyphMetrics metrics)
        {
            return PrimaryTypeface.TryGetGlyphMetrics(glyph, out metrics);  
        }

        public bool TryGetTable(uint tag, out byte[] table)
        {
            return PrimaryTypeface.TryGetTable(tag, out table); 
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
        // ~FamilyTypeface()
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

        PixelRect IGlyphRunRender.DrawGlyphRun(DrawingContextImpl context, PixelPoint position, GlyphRunImpl glyphRun, Color foreground)
        {
            var typeface = GetTypeface((int)glyphRun.FontRenderingEmSize);
            IGlyphRunRender typefaceDrawing = typeface as IGlyphRunRender;
            ArgumentNullException.ThrowIfNull(typefaceDrawing);
            return typefaceDrawing.DrawGlyphRun(context, position, glyphRun, foreground);
        }

        public ShapedBuffer ShapeText(ReadOnlyMemory<char> text, TextShaperOptions options)
        {
            var typeface = GetTypeface((int)options.FontRenderingEmSize);
            var textShaper = typeface as ITextShaperImpl;
            return textShaper.ShapeText(text, options);
        }
    }
}