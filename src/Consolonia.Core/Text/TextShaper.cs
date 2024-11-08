using System;
using System.Linq;
using Avalonia.Media.TextFormatting;
using Avalonia.Media.TextFormatting.Unicode;
using Avalonia.Platform;

namespace Consolonia.Core.Text
{

    public class TextShaper : ITextShaperImpl
    {
        public ShapedBuffer ShapeText(ReadOnlyMemory<char> text, TextShaperOptions options)
        {
            var glyphInfos = text.Span.ToString().EnumerateRunes()
                            .Select((rune, index) =>
                                new GlyphInfo((ushort)'X', index, 1)).ToArray();

            var shapedBuffer = new ShapedBuffer(text, glyphInfos.Length,
                options.Typeface, 1, 0 /*todo: must be 1 for right to left?*/);

            for (int i = 0; i < shapedBuffer.Length; i++)
                shapedBuffer[i] = glyphInfos[i];
            return shapedBuffer;
        }
    }
}