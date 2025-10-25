using System.Collections.Generic;
using System.Text;
using NeoSmart.Unicode;

namespace Consolonia.Core.Helpers
{
    public class Grapheme
    {
        public string Text { get; set; }

        public int Cluster { get; set; }

        /// <summary>
        ///     Process text into collection of glyphs where a glyph is either text or a combination of chars which make up an
        ///     emoji.
        /// </summary>
        /// <param name="text">text to get glyphs from</param>
        /// <param name="supportsComplexEmoji">If true, emojis like 👨‍👩‍👧‍👦 will be treated as a single glyph></param>
        /// <returns></returns>
        public static IReadOnlyList<Grapheme> Parse(string text, bool supportsComplexEmoji)
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