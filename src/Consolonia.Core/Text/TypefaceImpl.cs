using System;
using System.Linq;
using Avalonia.Platform;

namespace Consolonia.Core.Text
{
    internal class TypefaceImpl : IGlyphTypefaceImpl
    {
        public void Dispose()
        {
        }

        public ushort GetGlyph(uint codepoint)
        {
            checked
            {
                return (ushort)codepoint;
            }
        }

        public ushort[] GetGlyphs(ReadOnlySpan<uint> codepoints)
        {
            checked
            {
                return codepoints.ToArray().Select(u => (ushort)u).ToArray();
            }
        }

        public int GetGlyphAdvance(ushort glyph)
        {
            return 1;
        }

        public int[] GetGlyphAdvances(ReadOnlySpan<ushort> glyphs)
        {
            return Enumerable.Repeat(1, glyphs.Length).ToArray();
        }

        /// <summary>
        ///     https://docs.microsoft.com/en-us/typography/opentype/spec/ttch01#funits-and-the-em-square
        /// </summary>
        public short DesignEmHeight => 1;

        public int Ascent =>
            -1; //var height = (GlyphTypeface.Descent - GlyphTypeface.Ascent + GlyphTypeface.LineGap) * Scale;

        public int Descent => 0;
        public int LineGap => 0;
        public int UnderlinePosition => 0;
        public int UnderlineThickness => 0;
        public int StrikethroughPosition => 0;
        public int StrikethroughThickness => 0;
        public bool IsFixedPitch => true;
    }
}