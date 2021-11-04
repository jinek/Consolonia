using System.Globalization;
using System.Linq;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Utilities;

namespace Consolonia.Core.Text
{
    internal class TextShaperImpl : ITextShaperImpl
    {
        public GlyphRun ShapeText(ReadOnlySlice<char> text, Typeface typeface, double fontRenderingEmSize,
            CultureInfo culture)
        {
            //todo: check defaults (can draw with font size = 0
            return new GlyphRun(new GlyphTypeface(typeface), fontRenderingEmSize,
                new ReadOnlySlice<ushort>(text.Select(c => (ushort)c).ToArray()), characters: text);
        }
    }
}