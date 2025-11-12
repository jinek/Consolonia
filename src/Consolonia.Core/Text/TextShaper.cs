using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Media.TextFormatting;
using Avalonia.Platform;
using Consolonia.Core.Helpers;
using Consolonia.Core.Infrastructure;
using Consolonia.Core.Text.Fonts;

namespace Consolonia.Core.Text
{
    public class TextShaper : ITextShaperImpl
    {
        public ShapedBuffer ShapeText(ReadOnlyMemory<char> text, TextShaperOptions options)
        {
            var console = AvaloniaLocator.Current.GetRequiredService<IConsoleOutput>();

            IReadOnlyList<Grapheme> graphemes = Grapheme.Parse(text.Span.ToString(), console.SupportsComplexEmoji);

            var glyphTypeface = options.Typeface;
            if (glyphTypeface is AsciiFamilyTypeface asciiFamilyTypeface)
            {
                glyphTypeface = asciiFamilyTypeface.GetTypeface((int)options.FontRenderingEmSize);
            }

            var shapedBuffer = new ShapedBuffer(text, graphemes.Count,
                glyphTypeface, 1, 0 /*todo: must be 1 for right to left?*/);


            for (ushort i = 0; i < shapedBuffer.Length; i++)
            {
                Grapheme grapheme = graphemes[i];
                ushort glyphIndex;
                int glyphAdvance;
                switch (grapheme.Glyph)
                {
                    case "\r":
                    case "\n":
                        glyphTypeface.TryGetGlyph(0x200c, out glyphIndex);
                        glyphAdvance = 0;
                        break;
                    default:
                        if (glyphTypeface is ConsoleTypeface consoleTypeface)
                        {
                            glyphIndex = consoleTypeface.GetGlyphIndex(grapheme.Glyph);
                            glyphAdvance = consoleTypeface.GetGlyphAdvance(glyphIndex);
                        }
                        else
                        {
                            glyphTypeface.TryGetGlyph((uint)grapheme.Glyph.EnumerateRunes().First().Value, out glyphIndex);
                            glyphAdvance = glyphTypeface.GetGlyphAdvance(glyphIndex);
                        }
                        break;
                }

                shapedBuffer[i] = new GlyphInfo(glyphIndex, grapheme.Cluster, glyphAdvance);
            }

            return shapedBuffer;
        }
    }
}