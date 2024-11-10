using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Consolonia.Core.Helpers;

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    [DebuggerDisplay("'{Text}'")]
    public readonly struct SimpleSymbol : ISymbol, IEquatable<SimpleSymbol>
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
            return string.IsNullOrEmpty(Text);
        }

        public ISymbol Blend(ref ISymbol symbolAbove)
        {
            return symbolAbove.IsWhiteSpace() ? this : symbolAbove;
        }

        public bool Equals(SimpleSymbol other)
        {
            return Text.Equals(other.Text, StringComparison.Ordinal);
        }

        public override bool Equals([NotNullWhen(true)] object obj)
        {
            return obj is SimpleSymbol other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Text.GetHashCode(StringComparison.Ordinal);
        }

        public static bool operator ==(SimpleSymbol left, SimpleSymbol right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SimpleSymbol left, SimpleSymbol right)
        {
            return !left.Equals(right);
        }
    }
}