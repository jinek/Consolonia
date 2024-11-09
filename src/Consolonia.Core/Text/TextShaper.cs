using System;
using Avalonia.Media.TextFormatting;
using Avalonia.Platform;
using Consolonia.Core.Helpers;

namespace Consolonia.Core.Text
{
    public class TextShaper : ITextShaperImpl
    {
        public ShapedBuffer ShapeText(ReadOnlyMemory<char> text, TextShaperOptions options)
        {
            var glyphs = text.Span.ToString().GetGlyphs();

            var shapedBuffer = new ShapedBuffer(text, glyphs.Count,
                options.Typeface, 1, 0 /*todo: must be 1 for right to left?*/);

            for (int i = 0; i < shapedBuffer.Length; i++)
                shapedBuffer[i] = new GlyphInfo('X', i, glyphs[i].MeasureText());

            return shapedBuffer;
        }
    }
}