using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using Consolonia.Controls;
using Newtonsoft.Json;
using Wcwidth;

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    [DebuggerDisplay("'{GetText()} {Pattern,b}'")]
    [JsonConverter(typeof(SymbolConverter))]
    public readonly struct Symbol : IEquatable<Symbol>
    {
        public static readonly Symbol Empty = new();
        public static readonly Symbol Space = new(' ');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Symbol()
        {
            // we use String.Empty to represent an empty symbol
            Character = Char.MinValue;
            Complex = null;
            Width = 0;
            Pattern = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Symbol(char ch)
        {
            Character = ch;
            Complex = null;
            Width = (byte)UnicodeCalculator.GetWidth(ch);
            Pattern = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Symbol(byte boxPattern)
        {
            Character = BoxPattern.GetBoxChar(boxPattern);
            Complex = null;
            Width = 1;
            Pattern = boxPattern;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Symbol(string glyph)
        {
            if (glyph.Length == 1)
            {
                Character = glyph[0];
                Complex = null;
                Width = (byte)UnicodeCalculator.GetWidth(Character);
                Pattern = 0;
            }
            else
            {
                // this is a multi-char glyph, we don't cache it. 
                Character = Char.MinValue;
                Pattern = 0;
                Complex = glyph;
                Width = (byte)Complex.MeasureText();
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Symbol(Rune rune)
            : this(rune.ToString())
        {
        }


        public bool Equals(Symbol other)
        {
            return Character == other.Character &&
                   String.Equals(Complex, other.Complex, StringComparison.Ordinal) &&
                   Width == other.Width &&
                   Pattern == other.Pattern;
        }


#pragma warning disable CA1051 // Do not declare visible instance fields
        /// <summary>
        /// The character for this symbol. If Complex is set this is Char.MinValue. 
        /// </summary>
        public readonly char Character;

        /// <summary>
        /// If cell has complex text (more than one char) this contains the full unicode sequence to draw this symbol.
        /// </summary>
        public readonly string Complex;

        // box pattern for box merging.
        public readonly byte Pattern;

        [JsonIgnore]
        public readonly byte Width;
#pragma warning restore CA1051 // Do not declare visible instance fields


        /// <summary>
        /// Get the symbol as text
        /// </summary>
        /// <returns>symbol as string</returns>
        /// NOTE: This is only for debug purposes, do not use in rendering code as it allocates a string for the character.
        public string GetText()
            => (Complex != null && Complex.Length > 1) ? Complex : Character.ToString();

        public bool NothingToDraw()
        {
            return Character == Char.MinValue && 
                String.IsNullOrEmpty(Complex);
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
                    Character == other.Character &&
                    String.Equals(Complex, other.Complex, StringComparison.Ordinal) &&
                    Width == other.Width &&
                    Pattern == other.Pattern;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Character, Complex, Width, Pattern);
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