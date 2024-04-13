using System;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using Avalonia.Platform;
using Avalonia.Utilities;

namespace Consolonia.Core.Text
{
    internal class TextShaperImpl : ITextShaperImpl
    {

        public ShapedBuffer ShapeText(ReadOnlyMemory<char> text, TextShaperOptions options)
        {
            //todo: check defaults (but can draw with font size = 0 in theory)
            var glyphIndices = new ReadOnlySlice<ushort>(text.Select(c => (ushort)c).ToArray());
            var glyphAdvances =
                new ReadOnlySlice<double>(new ReadOnlyMemory<double>(text.Select(_ => 1d).ToArray()));
            var glyphClusters =
                new ReadOnlySlice<ushort>(new ReadOnlyMemory<ushort>(text.Select((_, i) => (ushort)i).ToArray()));

            return new ShapedBuffer(
                new GlyphTypeface(typeface),
                fontRenderingEmSize,
                glyphIndices,
                glyphAdvances,
                new ReadOnlySlice<Vector>(null),
                text,
                glyphClusters /*todo: must be 1 for right to left*/);
        }
    }
}