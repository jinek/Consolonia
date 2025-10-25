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
    public class Grapheme
    {
        public string Text { get; set; }

        public int Cluster { get; set; }
    }

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
        public static IReadOnlyList<Grapheme> GetGraphemes(this string text, bool supportsComplexEmoji)
        {
            var glyphs = new List<Grapheme>();
            var emoji = new StringBuilder();
            StringRuneEnumerator runes = text.EnumerateRunes();
            var lastRune = new Rune();
            int regionalRuneCount = 0;
            int index = 0;
            int cluster = 0;
            while (runes.MoveNext())
            {
                if (runes.Current.Value == Codepoints.ZWJ ||
                    runes.Current.Value == Codepoints.ORC)
                {
                    if (supportsComplexEmoji)
                    {
                        // Append joiner to current emoji if building; otherwise, attach to last glyph (if any).
                        if (emoji.Length > 0)
                            emoji.Append(runes.Current);
                        else if (glyphs.Count > 0)
                            glyphs[^1].Text = glyphs[^1].Text + runes.Current;
                    }
                    else
                    {
                        // we terminate multi-chained 1 because terminal doesn't support it 
                        break;
                    }
                }
                else if (runes.Current.Value >= Emoji.SkinTones.Light && runes.Current.Value <= Emoji.SkinTones.Dark ||
                         runes.Current.Value == Codepoints.Keycap)
                {
                    // Append to current emoji if building; otherwise, attach to last glyph (if any).
                    if (emoji.Length > 0)
                        emoji.Append(runes.Current);
                    else if (glyphs.Count > 0)
                        glyphs[^1].Text = glyphs[^1].Text + runes.Current;
                }
                // regional indicator symbols
                else if (runes.Current.Value >= 0x1F1E6 && runes.Current.Value <= 0x1F1FF)

                {
                    regionalRuneCount++;
                    if (regionalRuneCount % 2 == 0 && emoji.Length > 0)
                    {
                        // complete the flag pair
                        emoji.Append(runes.Current);
                        glyphs.Add(new Grapheme() { Text = emoji.ToString(), Cluster = cluster });
                        cluster = index + runes.Current.Utf16SequenceLength;
                        emoji.Clear();
                    }
                    else
                    {
                        // start a new RI run (or recover if buffer was empty)
                        if (emoji.Length > 0)
                        {
                            glyphs.Add(new Grapheme() { Text = emoji.ToString(), Cluster = cluster });
                            cluster = index + runes.Current.Utf16SequenceLength;
                            emoji.Clear();
                        }

                        emoji.Append(runes.Current);
                    }
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
                        glyphs[^1].Text = glyphs[^1].Text + runes.Current;
                    }
                }
                else if (Emoji.IsEmoji(runes.Current.ToString()))
                {
                    if (supportsComplexEmoji &&
                        (lastRune.Value == Codepoints.ZWJ || lastRune.Value == Codepoints.ORC))
                    {
                        // the last char was a joiner or object replacement, so we continue building the emoji
                        emoji.Append(runes.Current);
                    }
                    else if (emoji.Length > 0)
                    {
                        // we have a new emoji starting, so we flush any existing emoji buffer
                        // ending the previous glyph and starting a new one
                        glyphs.Add(new Grapheme() { Text = emoji.ToString(), Cluster = cluster });
                        cluster = index;
                        emoji.Clear();
                        regionalRuneCount = 0;
                        emoji.Append(runes.Current);
                    }
                    else
                    {
                        regionalRuneCount = 0;
                        emoji.Append(runes.Current);
                    }
                }
                else
                {
                    if (emoji.Length > 0)
                    {
                        glyphs.Add(new Grapheme() { Text = emoji.ToString(), Cluster = cluster });
                        cluster = index + runes.Current.Utf16SequenceLength;
                        emoji.Clear();
                    }

                    glyphs.Add(new Grapheme() { Text = runes.Current.ToString(), Cluster = cluster });
                    cluster = index + runes.Current.Utf16SequenceLength;
                }

                lastRune = runes.Current;
                index += runes.Current.Utf16SequenceLength;
            }

            if (emoji.Length > 0) glyphs.Add(new Grapheme() { Text = emoji.ToString(), Cluster = cluster });
            return glyphs;
        }
    }
}