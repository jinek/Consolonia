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
            GlyphInfos = glyphInfos;
            Bounds = new Rect(new Point(0, 0),
                new Size(glyphInfos.Count, FontRenderingEmSize));
        }

        public IReadOnlyList<GlyphInfo> GlyphInfos { get; }

        public void Dispose()
        {
        }

        public IReadOnlyList<float> GetIntersections(float lowerLimit, float upperLimit)
        {
            // empty intersections defaults to entire span.
            return new List<float>();
        }

        public IGlyphTypeface GlyphTypeface { get; }

        public double FontRenderingEmSize { get; }
        public Point BaselineOrigin { get; }
        public Rect Bounds { get; }
    }
}