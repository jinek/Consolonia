using System;
using System.Collections.Generic;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Reactive;
using Avalonia.Styling;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Consolonia.Core.Infrastructure;
using NeoSmart.Unicode;
using Wcwidth;

namespace Consolonia.Core.Helpers
{
    public static class Extensions
    {
        public static IDisposable SubscribeAction<TValue>(
            this IObservable<AvaloniaPropertyChangedEventArgs<TValue>> observable,
            Action<AvaloniaPropertyChangedEventArgs<TValue>> action)
        {
            return observable.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<TValue>>(action));
        }

        public static void AddConsoloniaDesignMode(this Application application)
        {
            if (Design.IsDesignMode)
            {
                // For previewing in Visual Studio designer without Design.PreviewWith tag we need to set default font and colors
                // get anything to render. This is not perfect, but nicer than getting a big error screen.
                IBrush foregroundBrush = Brushes.White;
                if (application.Styles.TryGetResource("ThemeForegroundBrush", null, out object brush))
                    foregroundBrush = (IBrush)brush;

                IBrush backgroundBrush = Brushes.Black;
                if (application.Styles.TryGetResource("ThemeBackgroundBrush", null, out brush))
                    backgroundBrush = (IBrush)brush;

                application.Styles.Add(new Style(x => x.Is<TemplatedControl>())
                {
                    Setters =
                    {
                        new Setter(TemplatedControl.FontSizeProperty, 16.0),
                        new Setter(TemplatedControl.FontFamilyProperty, new FontFamily("Cascadia Mono")),
                        new Setter(TemplatedControl.ForegroundProperty, foregroundBrush),
                        new Setter(TemplatedControl.BackgroundProperty, backgroundBrush)
                    }
                });

                // EXPERIMENTAL
                // If you do RenderTransform="scale(10.0,10.0) you can actually sort of see the UI get bigger
                // but this doesn't seem to work when using these style setters. <sigh>
                //this.Styles.Add(new Style(x => x.Is<Visual>())
                //{
                //    Setters =
                //    {
                //        //new Setter(Visual.RenderTransformOriginProperty, RelativePoint.TopLeft),
                //        //new Setter(Visual.RenderTransformProperty, new ScaleTransform(2.0, 2.0)),
                //        //new Setter(Visual.RenderTransformProperty, new ScaleTransform(2.0, 2.0)),
                //    }
                //});
            }
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