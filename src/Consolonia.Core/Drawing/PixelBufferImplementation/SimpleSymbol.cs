using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Consolonia.Core.Helpers;
using Newtonsoft.Json;

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    [DebuggerDisplay("'{Text}'")]
    [JsonConverter(typeof(SymbolConverter))]
    public readonly struct SimpleSymbol : ISymbol, IEquatable<SimpleSymbol>
    {
        public SimpleSymbol()
        {
            // we use String.Empty to represent an empty symbol
            Text = string.Empty;
            Width = 0;
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

        public bool Equals(SimpleSymbol other)
        {
            return Text.Equals(other.Text, StringComparison.Ordinal);
        }

        public string Text { get; }

        [JsonIgnore] public ushort Width { get; init; }

        public bool IsWhiteSpace()
        {
            return string.IsNullOrWhiteSpace(Text);
        }

        public ISymbol Blend(ref ISymbol symbolAbove)
        {
            return symbolAbove.IsWhiteSpace() ? this : symbolAbove;
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