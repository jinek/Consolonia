using System;
using System.Collections.Generic;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Reactive;
using Avalonia.Styling;
using NeoSmart.Unicode;

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
            {
                // if last rune was a joiner, then we append to the emoji
                if (Emoji.IsEmoji(runes.Current.ToString()))
                {
                    if (supportsComplexEmoji &&
                        lastRune.Value == Codepoints.ZWJ ||
                        lastRune.Value == Codepoints.ORC)
                    {
                        // the last char was a joiner or object replacement, so we continue building the emoji
                        emoji.Append(runes.Current);
                    }
                    else
                    {
                        // we have a new emoji starting, so we flush any existing emoji buffer
                        // ending the previous glyph and starting a new one
                        if (emoji.Length > 0)
                        {
                            glyphs.Add(emoji.ToString());
                            emoji.Clear();
                        }

                        emoji.Append(runes.Current);
                    }
                }
                // Emoji modifier (skin tone) or keycap extender should continue current glyph
                else if (runes.Current.Value >= 0x1F3FB && runes.Current.Value <= 0x1F3FF || // Fitzpatrick
                         runes.Current.Value == 0x20E3) // COMBINING ENCLOSING KEYCAP
                {
                    if (emoji.Length > 0)
                        emoji.Append(runes.Current);
                    else if (glyphs.Count > 0)
                        glyphs[^1] = glyphs[^1] + runes.Current;
                    // else: stray — ignore
                }
                else if (runes.Current.Value == Codepoints.ZWJ ||
                          runes.Current.Value == Codepoints.ORC)
                {
                    if (supportsComplexEmoji)
                    {
                        // Append joiner to current emoji if building; otherwise, attach to last glyph (if any).
                        if (emoji.Length > 0)
                            emoji.Append(runes.Current);
                        else if (glyphs.Count > 0) 
                            glyphs[^1] = glyphs[^1] + runes.Current;
                    }
                    // else: stray joiner — ignore
                }
                else if (runes.Current.Value == Codepoints.VariationSelectors.EmojiSymbol ||
                         runes.Current.Value == Codepoints.VariationSelectors.TextSymbol)
                {
                    // Variation selectors should be appended to the current glyph being built
                    // If we have a glyph in progress (emoji buffer), append to it
                    if (emoji.Length > 0)
                    {
                        emoji.Append(runes.Current);
                    }
                    // Otherwise, if we have any glyphs, we need to append the variation selector to the last glyph
                    else if (glyphs.Count > 0)
                    {
                        string lastGlyph = glyphs[glyphs.Count - 1];
                        glyphs[glyphs.Count - 1] = lastGlyph + runes.Current;
                    }
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

            if (emoji.Length > 0) glyphs.Add(emoji.ToString());
            return glyphs;
        }
    }
}