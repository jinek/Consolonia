using System;
using System.Collections.Generic;
using System.Text;
using Avalonia;
using Avalonia.Reactive;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Consolonia.Core.Infrastructure;
using NeoSmart.Unicode;
using Wcwidth;

namespace Consolonia.Core.Helpers
{
    public static class Extensions
    {
        public static void SubscribeAction<TValue>(
            this IObservable<AvaloniaPropertyChangedEventArgs<TValue>> observable,
            Action<AvaloniaPropertyChangedEventArgs<TValue>> action)
        {
            observable.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<TValue>>(action));
        }

        public static void Print(this IConsole console, PixelBufferCoordinate point, Pixel pixel)
        {
            console.Print(point, 
                pixel.Background.Color, 
                pixel.Foreground.Color, 
                pixel.Foreground.Style, 
                pixel.Foreground.Weight, 
                pixel.Foreground.TextDecorations, 
                pixel.Foreground.Symbol.Text);
        }

        /// <summary>
        ///  Process text into collection of glyphs where a glyph is either text or a combination of chars which make up an emoji.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static IList<string> GetGlyphs(this string text)
        {
            List<string> glyphs = new List<string>();
            StringBuilder emoji = new StringBuilder();
            var runes = text.EnumerateRunes();
            Rune lastRune = new Rune();

            while (runes.MoveNext())
            {
                if (lastRune.Value == Codepoints.ZWJ ||
                    lastRune.Value == Codepoints.ORC ||
                    Emoji.IsEmoji(runes.Current.ToString()))
                {
                    emoji.Append(runes.Current);
                }
                else if (runes.Current.Value == Emoji.ZeroWidthJoiner ||
                        runes.Current.Value == Emoji.ObjectReplacementCharacter ||
                        runes.Current.Value == Codepoints.VariationSelectors.EmojiSymbol ||
                        runes.Current.Value == Codepoints.VariationSelectors.TextSymbol)
                {
                    emoji.Append(runes.Current);
                }
                else
                {
                    if (emoji.Length > 0)
                    {
                        glyphs.Add(emoji.ToString());
                        emoji.Clear();
                    }
                    glyphs.Add(runes.Current.ToString());
                }
                lastRune = runes.Current;
            }
            if (emoji.Length > 0)
            {
                glyphs.Add(emoji.ToString());
            }
            return glyphs;
        }

        /// <summary>
        /// Measure text for actual width 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static ushort MeasureText(this string text)
        {
            ushort width = 0;
            ushort lastWidth = 0;
            foreach (var rune in text.EnumerateRunes())
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