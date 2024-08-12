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
        public GlyphRunImpl(IGlyphTypeface glyphTypeface, IReadOnlyList<GlyphInfo> glyphInfos, Point baselineOrigin)
        {
            FontRenderingEmSize = glyphTypeface.Metrics.DesignEmHeight;
            GlyphTypeface = glyphTypeface;
            BaselineOrigin = baselineOrigin;
            GlyphIndices = glyphInfos.Select(info => info.GlyphIndex).ToArray();
            Bounds = new Rect(baselineOrigin + new Point(0, glyphTypeface.Metrics.Ascent),
                new Size(glyphInfos.Sum(info => info.GlyphAdvance), FontRenderingEmSize));
        }

        public void Dispose()
        {
            
        }

        public IReadOnlyList<float> GetIntersections(float lowerLimit, float upperLimit)
        {
            throw new NotImplementedException();
        }

        public IGlyphTypeface GlyphTypeface { get; }

        public double FontRenderingEmSize { get; }
        public Point BaselineOrigin { get; }
        public Rect Bounds { get; }
        public ushort[] GlyphIndices { get; }
    }
}