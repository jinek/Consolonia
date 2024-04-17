using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using Avalonia.Platform;

namespace Consolonia.Core.Text
{
    internal class GlyphRunImpl : IGlyphRunImpl
    {
        private readonly IReadOnlyList<GlyphInfo> _glyphInfos;
        private readonly string _text;

        public GlyphRunImpl(string text)
        {
            throw new NotImplementedException("Remove this?");
            _text = text;
            Bounds = new Rect(0, 0, text.Length, 1);//todo: no all symbols take 1 width
        }

        public GlyphRunImpl(IGlyphTypeface glyphTypeface, IReadOnlyList<GlyphInfo> glyphInfos, Point baselineOrigin)
        {
            _glyphInfos = glyphInfos;
            GlyphTypeface = glyphTypeface;
            BaselineOrigin = baselineOrigin;
            GlyphIndices = glyphInfos.Select(info => info.GlyphIndex).ToArray();
        }

        public void Dispose()
        {
            
        }

        public IReadOnlyList<float> GetIntersections(float lowerLimit, float upperLimit)
        {
            throw new NotImplementedException();
        }

        public IGlyphTypeface GlyphTypeface { get; }

        public double FontRenderingEmSize => 1;
        public Point BaselineOrigin { get; }
        public Rect Bounds { get; }
        public ushort[] GlyphIndices { get; }
    }
}