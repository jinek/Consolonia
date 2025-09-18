using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using Consolonia.Controls;
using Newtonsoft.Json;

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    [DebuggerDisplay("'{Text} {Pattern,b}'")]
    [JsonConverter(typeof(SymbolConverter))]
    public readonly struct Symbol : IEquatable<Symbol>
    {
        private const string BoldText = "█";

        // this is a cache of all characters as strings, primarily for box-drawing characters
        private static readonly string[] SymbolCache = new string[char.MaxValue];

        public static readonly Symbol Empty = new();
        public static readonly Symbol Space = new(" ");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Symbol()
        {
            // we use String.Empty to represent an empty symbol
            Text = string.Empty;
            Width = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Symbol(char ch)
        {
            Text = SymbolCache[ch];
            if (Text == null)
            {
                Text = ch.ToString();
                SymbolCache[ch] = Text;
            }

            Width = (byte)Text.MeasureText();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Symbol(byte upRightDownLeftPattern)
        {
            Width = 1;
            Pattern = upRightDownLeftPattern;
            char boxChar = BoxPattern.GetBoxChar(upRightDownLeftPattern);
            if (boxChar == BoxPattern.BoldChar)
            {
                // get well-known string instance
                Text = BoldText;
            }
            else if (boxChar == BoxPattern.EmptyChar)
            {
                Text = string.Empty;
            }
            else if (boxChar >= BoxPattern.Min && boxChar <= BoxPattern.Max)
            {
                // get cached string instance
                Text = SymbolCache[boxChar];
                if (Text == null)
                {
                    Text = boxChar.ToString();
                    SymbolCache[boxChar] = Text;
                }
            }
            else
                // uhoh
            {
                throw new ArgumentOutOfRangeException(nameof(upRightDownLeftPattern));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Symbol(string glyph)
        {
            if (glyph.Length == 1)
            {
                Text = SymbolCache[glyph[0]];
                if (Text == null)
                {
                    Text = glyph[0].ToString();
                    SymbolCache[glyph[0]] = Text;
                }
            }
            else
            {
                Text = glyph;
            }

            Width = (byte)Text.MeasureText();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Symbol(Rune rune)
            : this(rune.ToString())
        {
        }

        public bool Equals(Symbol other)
        {
            return Text.Equals(other.Text, StringComparison.Ordinal) &&
                   Pattern == other.Pattern;
        }

        public string Text { get; }

        // box pattern for box merging.
        public byte Pattern { get; }

        [JsonIgnore] public byte Width { get; init; }

        public bool NothingToDraw()
        {
            return string.IsNullOrEmpty(Text);
        }

        public Symbol Blend(ref Symbol symbolAbove)
        {
            if (symbolAbove.NothingToDraw())
                return this;

            if (IsBoxSymbol() && symbolAbove.IsBoxSymbol())
                return new Symbol(BoxPattern.Merge(Pattern, symbolAbove.Pattern));
            return symbolAbove;
        }

        public bool IsBoxSymbol()
        {
            return Pattern > 0;
        }

        public override bool Equals([NotNullWhen(true)] object obj)
        {
            return obj is Symbol other && Text.Equals(other.Text, StringComparison.Ordinal) &&
                   Pattern == other.Pattern;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Text, Pattern);
        }

        public static bool operator ==(Symbol left, Symbol right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Symbol left, Symbol right)
        {
            return !left.Equals(right);
        }
    }
}