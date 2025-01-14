using System;
using System.Collections.Generic;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Media;
using Avalonia.Reactive;
using Avalonia.Styling;
using Consolonia.Core.Controls;
using Consolonia.Core.Infrastructure;
using NeoSmart.Unicode;
using Wcwidth;

namespace Consolonia.Core.Helpers
{
    public static class UtilityExtensions
    {
        public static IDisposable SubscribeAction<TValue>(
            this IObservable<AvaloniaPropertyChangedEventArgs<TValue>> observable,
            Action<AvaloniaPropertyChangedEventArgs<TValue>> action)
        {
            return observable.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<TValue>>(action));
        }


        /// <summary>
        ///     Process text into collection of glyphs where a glyph is either text or a combination of chars which make up an
        ///     emoji.
        /// </summary>
        /// <param name="text">text to get glyphs from</param>
        /// <param name="supportsComplexEmoji">If true, emojis like 👨‍👩‍👧‍👦 will be treated as a single glyph></param>
        /// <returns></returns>
        public static IReadOnlyList<string> GetGlyphs(this string text, bool supportsComplexEmoji)
        {
            var glyphs = new List<string>();
            var emoji = new StringBuilder();
            StringRuneEnumerator runes = text.EnumerateRunes();
            var lastRune = new Rune();

            while (runes.MoveNext())
                if (supportsComplexEmoji)
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
                else
                {
                    if (runes.Current.Value != Emoji.ZeroWidthJoiner &&
                        runes.Current.Value != Emoji.ObjectReplacementCharacter &&
                        runes.Current.Value != Codepoints.VariationSelectors.EmojiSymbol &&
                        runes.Current.Value != Codepoints.VariationSelectors.TextSymbol)
                        glyphs.Add(runes.Current.ToString());
                }

            if (emoji.Length > 0) glyphs.Add(emoji.ToString());
            return glyphs;
        }

        /// <summary>
        ///     Measure text for actual width
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static ushort MeasureText(this string text)
        {
            var console = AvaloniaLocator.Current.GetService<IConsoleOutput>();
            bool supportsComplexEmoji = console == null || console.SupportsComplexEmoji;
            ushort width = 0;
            ushort lastWidth = 0;
            foreach (Rune rune in text.EnumerateRunes())
            {
                ushort runeWidth = (ushort)UnicodeCalculator.GetWidth(rune);
                if (supportsComplexEmoji &&
                    (rune.Value == Emoji.ZeroWidthJoiner || rune.Value == Emoji.ObjectReplacementCharacter))
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