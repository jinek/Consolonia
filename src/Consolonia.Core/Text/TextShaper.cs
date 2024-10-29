using System;
using System.Linq;
using System.Text;
using Avalonia.Media.TextFormatting;
using Avalonia.Platform;
using NullLib.ConsoleEx;

namespace Consolonia.Core.Text
{
    public class TextShaper : ITextShaperImpl
    {
        public ShapedBuffer ShapeText(ReadOnlyMemory<char> text, TextShaperOptions options)
        {
            var glyphInfos = Convert(text.Span.ToString());
            
            var shapedBuffer = new ShapedBuffer(text, glyphInfos.Length,
                options.Typeface ?? new GlyphTypeface(), 1, 0 /*todo: must be 1 for right to left?*/);
            
            for (int i = 0; i < shapedBuffer.Length; i++)
            {
                shapedBuffer[i] = glyphInfos[i];
            }
            return shapedBuffer;
        }

        public static GlyphInfo[] Convert(string str)
        {
            if (!str.IsNormalized(NormalizationForm.FormKC))
                str = str.Normalize(NormalizationForm.FormKC);

            // ReSharper disable once InvertIf
            if (str.Any(
                    c => ConsoleText.IsWideChar(c) &&
                         char.IsLetterOrDigit(c) /*todo: https://github.com/SlimeNull/NullLib.ConsoleEx/issues/2*/))
            {
                StringBuilder stringBuilder = new();
                foreach (char c in str)
                    stringBuilder.Append(ConsoleText.IsWideChar(c) && char.IsLetterOrDigit(c)
                        ? '?' //todo: support wide characters
                        : c);

                str = stringBuilder.ToString();
            }

            return str.Select((c, index) => new GlyphInfo(c, index, 1)).ToArray();
        }
    }
}