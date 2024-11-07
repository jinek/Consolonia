using System;
using System.Linq;
using Avalonia.Media.TextFormatting;
using Avalonia.Platform;
using NullLib.ConsoleEx;

namespace Consolonia.Core.Text
{
    public class TextShaper : ITextShaperImpl
    {
        public ShapedBuffer ShapeText(ReadOnlyMemory<char> text, TextShaperOptions options)
        {
            Text = text.Span.ToString();

            var glyphInfos = Convert(Text);

            var shapedBuffer = new ShapedBuffer(text, glyphInfos.Length,
                options.Typeface, 1, 0 /*todo: must be 1 for right to left?*/);

            for (int i = 0; i < shapedBuffer.Length; i++) shapedBuffer[i] = glyphInfos[i];
            return shapedBuffer;
        }

        public string Text { get; set; }

        public static GlyphInfo[] Convert(string text)
        {
            return text.EnumerateRunes()
                            .Select((rune, index) =>
                                new GlyphInfo((ushort)rune.Value, index, ConsoleText.IsWideChar((char)rune.Value) ? 2 : 1)).ToArray();
        }


    }
}