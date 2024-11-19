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
    public class SimpleSymbol : ISymbol, IEquatable<SimpleSymbol>
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

        public string Text { get;  } 

        [JsonIgnore]
        public ushort Width { get; init; }

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
            if ((object)other is null)
                return false;

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
            if ((object)left is null)
            {
                return (object)right is null;
            }
            return left.Equals(right);
        }

        public static bool operator !=(SimpleSymbol left, SimpleSymbol right)
        {
            if ((object)left is null)
            {
                return (object)right is not null;
            }

            return !left.Equals(right);
        }
    }
}