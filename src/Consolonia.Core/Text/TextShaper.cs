using System;
using System.Collections.Generic;
using Avalonia.Media.TextFormatting;
using Avalonia.Platform;

namespace Consolonia.Core.Text
{
    public class TextShaper : ITextShaperImpl
    {
        public ShapedBuffer ShapeText(ReadOnlyMemory<char> text, TextShaperOptions options)
        {
            if (options.Typeface is ITextShaperImpl textShaper) return textShaper.ShapeText(text, options);

            throw new KeyNotFoundException(
                "Unsupported console glyph typeface, we only work with ITextShaperImpl typefaces.");
        }
    }
}