using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using Consolonia.Controls;
using Newtonsoft.Json;

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    [DebuggerDisplay("'{Text}'")]
    [JsonConverter(typeof(SymbolConverter))]
    public readonly struct SimpleSymbol : ISymbol, IEquatable<SimpleSymbol>
    {
        public static readonly SimpleSymbol Empty = new();
        public static readonly SimpleSymbol Space = new(" ");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SimpleSymbol()
        {
            // we use String.Empty to represent an empty symbol
            Text = string.Empty;
            Width = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SimpleSymbol(char character)
            : this(character.ToString())
        {
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SimpleSymbol(string glyph)
        {
            Text = glyph;
            Width = Text.MeasureText();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        public bool NothingToDraw()
        {
            return string.IsNullOrEmpty(Text);
        }

        public ISymbol Blend(ref ISymbol symbolAbove)
        {
            return symbolAbove.NothingToDraw() ? this : symbolAbove;
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