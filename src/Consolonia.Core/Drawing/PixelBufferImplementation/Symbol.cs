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
        // this is a cache of all characters as symbols, pattern will always be zero.
        private static readonly Symbol[] SymbolCache = new Symbol[char.MaxValue + 1];

        public static readonly Symbol Empty = new();
        public static readonly Symbol Space = new(' ');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Symbol()
        {
            // we use String.Empty to represent an empty symbol
            Text = string.Empty;
            Width = 0;
            Pattern = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Symbol(char ch)
        {
            // struct copy from cache
            this = GetCharSymbolFromCache(ch);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Symbol(byte boxPattern)
        {
            // struct copy from cache
            this = GetCharSymbolFromCache(BoxPattern.GetBoxChar(boxPattern));
            Pattern = boxPattern;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Symbol(string glyph)
        {
            if (glyph.Length == 1)
            {
                // this gives us a single instance of the string for the character even if it is being flood filled into pixelbuffer.

                // struct copy from cache
                this = GetCharSymbolFromCache(glyph[0]);
            }
            else
            {
                // this is a multi-char glyph, we don't cache it. 
                Text = glyph;
                Width = (byte)Text.MeasureText();
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Symbol(Rune rune)
            : this(rune.ToString())
        {
        }


        // this private ctor is used to create new symbol for cache (other ctor call into the cache)
        private Symbol(string text, byte width)
        {
            Text = text;
            Width = width;
            Pattern = 0;
        }

        public bool Equals(Symbol other)
        {
            return Text.Equals(other.Text, StringComparison.Ordinal) &&
                   Width == other.Width &&
                   Pattern == other.Pattern;
        }

        public string Text { get; }

        // box pattern for box merging.
        public byte Pattern { get; }

        [JsonIgnore] public byte Width { get; init; }

        public bool NothingToDraw()
        {
            return Text.Length == 0;
        }

        public Symbol Blend(ref Symbol symbolAbove)
        {
            if (symbolAbove.NothingToDraw())
                return this;

            // if both are box symbols we need to merge them.
            if (IsBoxSymbol() && symbolAbove.IsBoxSymbol())
                return new Symbol(BoxPattern.Merge(Pattern, symbolAbove.Pattern));

            // top symbol always overwrites bottom symbol. We know it's not empty because we checked that above.
            return symbolAbove;
        }

        public bool IsBoxSymbol()
        {
            return Pattern > 0;
        }

        public override bool Equals([NotNullWhen(true)] object obj)
        {
            return obj is Symbol other && 
                    Text.Equals(other.Text, StringComparison.Ordinal) &&
                    Width.Equals(other.Width) &&
                    Pattern == other.Pattern;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Text, Width, Pattern);
        }

        public static bool operator ==(Symbol left, Symbol right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Symbol left, Symbol right)
        {
            return !left.Equals(right);
        }

        private static Symbol GetCharSymbolFromCache(char ch)
        {
            var symbol = SymbolCache[ch];

            // uninitialized symbol in cache has null text.
            if (symbol.Text == null)
            {
                var text = ch.ToString();
                symbol = new Symbol(text, (byte)text.MeasureText());
                SymbolCache[ch] = symbol;
            }
            return symbol;

        }
    }
}