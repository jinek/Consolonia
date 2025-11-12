using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            if (options.Typeface is ITextShaperImpl textShaper)
            {
                return textShaper.ShapeText(text, options);
            }

            throw new ArgumentNullException(nameof(options.Typeface.FamilyName), "Unsupported glyph typeface.");
        }
    }
}