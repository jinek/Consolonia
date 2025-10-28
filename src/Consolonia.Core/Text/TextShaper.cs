using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;
using Avalonia;
using Avalonia.Media.TextFormatting;
using Avalonia.Platform;
using Consolonia.Core.Helpers;
using Consolonia.Core.Infrastructure;

namespace Consolonia.Core.Text
{
    public class TextShaper : ITextShaperImpl
    {

        public ShapedBuffer ShapeText(ReadOnlyMemory<char> text, TextShaperOptions options)
        {
            var console = AvaloniaLocator.Current.GetRequiredService<IConsoleOutput>();

            IReadOnlyList<Grapheme> graphemes = Grapheme.Parse(text.Span.ToString(), console.SupportsComplexEmoji);

            var shapedBuffer = new ShapedBuffer(text, graphemes.Count,
                options.Typeface, 1, 0 /*todo: must be 1 for right to left?*/);

            var glyphTypeface = options.Typeface as ConsoleTypeface;
            for (ushort i = 0; i < shapedBuffer.Length; i++)
            {
                var grapheme = graphemes[i];
                ushort glyphIndex;
                int glyphAdvance = 0;
                switch (grapheme.Glyph)
                {
                    case "\r":
                    case "\n":
                        glyphIndex = glyphTypeface.GetGlyphIndex("\u200c");
                        glyphAdvance = 0;
                        break;
                    default:
                        glyphIndex = glyphTypeface.GetGlyphIndex(grapheme.Glyph);
                        glyphAdvance =  glyphTypeface.GetGlyphAdvance(glyphIndex);
                        break;
                }

                shapedBuffer[i] = new GlyphInfo(glyphIndex, grapheme.Cluster, glyphAdvance);
            }
            return shapedBuffer;
        }
    }
}