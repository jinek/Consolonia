using System;
using System.Diagnostics;
using System.Text;
using NeoSmart.Unicode;
using Wcwidth;

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    [DebuggerDisplay("'{Text}'")]
    public readonly struct SimpleSymbol : ISymbol
    {
        public SimpleSymbol()
        {
            // we use String.Empty to represent an empty symbol. It still takes up space, but it's invisible
            Text = string.Empty;
            Width = 1;
        }

        public SimpleSymbol(char character)
            :this(character.ToString())
        {
        }

        public SimpleSymbol(string glyph)
        {
            Text = glyph;
            Width = MeasureGlyph(Text);
        }

        public SimpleSymbol(Rune rune)
        {
            Text = rune.ToString();
            Width = MeasureGlyph(Text);
        }

        public string Text { get; } = string.Empty;

        public ushort Width { get; }

        public bool IsWhiteSpace()
        {
            return string.IsNullOrWhiteSpace(Text);
        }

        public ISymbol Blend(ref ISymbol symbolAbove)
        {
            return !String.IsNullOrEmpty(symbolAbove.Text) ? symbolAbove : this;
        }


        private static ushort MeasureGlyph(string glyph)
        {
            ushort width = 0;
            ushort lastWidth = 0;
            foreach (var rune in glyph.EnumerateRunes())
            {
                var runeWidth = (ushort)UnicodeCalculator.GetWidth(rune);
                if (rune.Value == Emoji.ZeroWidthJoiner || rune.Value == Emoji.ObjectReplacementCharacter)
                    width -= lastWidth;
                else
                    width += runeWidth;

                if (runeWidth > 0)
                    lastWidth = runeWidth;
            }

            return width;
        }
    }
}