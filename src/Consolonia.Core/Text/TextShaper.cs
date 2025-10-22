using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly static Dictionary<ushort, string> GlyphByIndex = new();
        private readonly static Dictionary<string, ushort> IndexByGlyph = new();

        public ShapedBuffer ShapeText(ReadOnlyMemory<char> text, TextShaperOptions options)
        {
            var console = AvaloniaLocator.Current.GetRequiredService<IConsoleOutput>();

            IReadOnlyList<string> glyphs = text.Span.ToString().GetGlyphs(console.SupportsComplexEmoji);

            var shapedBuffer = new ShapedBuffer(text, glyphs.Count,
                options.Typeface, 1, 0 /*todo: must be 1 for right to left?*/);

            for (ushort i = 0; i < shapedBuffer.Length; i++)
            {
                var glyph = glyphs[i];
                ushort glyphIndex;
                lock (IndexByGlyph)
                {
                    if (!IndexByGlyph.TryGetValue(glyph, out glyphIndex))
                    {
                        glyphIndex = (ushort)GlyphByIndex.Count;
                        GlyphByIndex[glyphIndex] = glyph;
                        IndexByGlyph[glyph] = glyphIndex;
                        Debug.WriteLine(glyphIndex);
                    }
                }

                // NOTE: We are using the placeholder glyph since we are pushing
                // raw text to the console and not using a font system to render the text
                shapedBuffer[i] = new GlyphInfo(glyphIndex, i, glyph.MeasureText());
            }

            return shapedBuffer;
        }

        public static string GetGlyphByIndex(ushort glyphIndex)
        {
            lock (IndexByGlyph)
            {
                return GlyphByIndex[glyphIndex];
            }
        }
    }
}