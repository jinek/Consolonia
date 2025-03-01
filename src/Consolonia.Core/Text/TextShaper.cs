using System;
using Avalonia;
using Avalonia.Media.TextFormatting;
using Avalonia.Platform;
using Consolonia.Controls;
using Consolonia.Core.Helpers;
using Consolonia.Core.Infrastructure;

namespace Consolonia.Core.Text
{
    public class TextShaper : ITextShaperImpl
    {
        public ShapedBuffer ShapeText(ReadOnlyMemory<char> text, TextShaperOptions options)
        {
            var console = AvaloniaLocator.Current.GetRequiredService<IConsoleOutput>();

            var glyphs = text.Span.ToString().GetGlyphs(console.SupportsComplexEmoji);

            var shapedBuffer = new ShapedBuffer(text, glyphs.Count,
                options.Typeface, 1, 0 /*todo: must be 1 for right to left?*/);

            for (int i = 0; i < shapedBuffer.Length; i++)
                // NOTE: We are using the placeholder glyph since we are pushing
                // raw text to the console and not using a font system to render the text
                shapedBuffer[i] = new GlyphInfo(GlyphTypeface.Glyph, i, glyphs[i].MeasureText());

            return shapedBuffer;
        }
    }
}