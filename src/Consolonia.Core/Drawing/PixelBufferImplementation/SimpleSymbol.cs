using System.Diagnostics;
using System.Text;
using Consolonia.Core.Helpers;

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
            : this(character.ToString())
        {
        }

        public SimpleSymbol(string glyph)
        {
            Text = glyph;
            Width = Text.MeasureText();
        }

        public SimpleSymbol(Rune rune)
        {
            Text = rune.ToString();
            Width = Text.MeasureText();
        }

        public string Text { get; } = string.Empty;

        public ushort Width { get; }

        public bool IsWhiteSpace()
        {
            return string.IsNullOrWhiteSpace(Text);
        }

        public ISymbol Blend(ref ISymbol symbolAbove)
        {
            return !string.IsNullOrEmpty(symbolAbove.Text) ? symbolAbove : this;
        }
    }
}