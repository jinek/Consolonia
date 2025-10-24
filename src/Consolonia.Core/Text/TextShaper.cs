using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Avalonia;
using Avalonia.Media;
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

            IReadOnlyList<string> glyphs = text.Span.ToString().GetGlyphs(console.SupportsComplexEmoji);

            var shapedBuffer = new ShapedBuffer(text, glyphs.Count,
                options.Typeface, 1, 0 /*todo: must be 1 for right to left?*/);

            var glyphTypeface = options.Typeface as GlyphTypeface;
            for (ushort i = 0; i < shapedBuffer.Length; i++)
            {
                var glyph = glyphs[i];
                var glyphIndex = glyphTypeface.GetGlyphIndex(glyph);
                var glyphWidth = glyphTypeface.GetGlyphAdvance(glyphIndex);
                // NOTE: We are using the placeholder glyph since we are pushing
                // raw text to the console and not using a font system to render the text
                shapedBuffer[i] = new GlyphInfo(glyphIndex, i, glyphWidth);
            }
            //Debug.WriteLine(text);
            //Debug.WriteLine($"Indexed as:{String.Join(',', shapedBuffer.Select(gi=> gi.GlyphIndex))}");
            //Debug.WriteLine($"Advance as:{String.Join(',', shapedBuffer.Select(gi=> gi.GlyphAdvance))}");
            return shapedBuffer;
        }
    }
}