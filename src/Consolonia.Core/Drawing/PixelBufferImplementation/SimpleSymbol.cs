using System;
using System.Diagnostics;

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    [DebuggerDisplay("'{Text}'")]
    public readonly struct SimpleSymbol : ISymbol
    {
        public SimpleSymbol()
        {
            Text = string.Empty;
            Width = 1;
        }

        public SimpleSymbol(char character)
        {
            Text = character.ToString();
            Width = 1;
        }

        public SimpleSymbol(string text, ushort width)
        {
            Text = text;
            Width = width;
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
    }
}