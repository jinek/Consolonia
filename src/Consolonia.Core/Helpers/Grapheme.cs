using System.Collections.Generic;
using System.Text;
using NeoSmart.Unicode;

namespace Consolonia.Core.Helpers
{
    /// <summary>
    /// Represents a single Glyph, the codepoints that produce it and the position in the text.
    /// </summary>
    public class Grapheme
    {
        // sequence of unicode codepoints that produce the glyph
        public string Glyph { get; set; }

        /// <summary>
        /// Index into original string of codepoints for start for Glyph
        /// </summary>
        public int Cluster { get; set; }

        /// <summary>
        ///     Process text into collection of grapheme where a grapheme is a single glyph
        /// </summary>
        /// <param name="text">text to get glyphs from</param>
        /// <param name="supportsComplexEmoji">If true, emojis like 👨‍👩‍👧‍👦 will be treated as a single glyph></param>
        /// <returns></returns>
        public static IReadOnlyList<Grapheme> Parse(string text, bool supportsComplexEmoji)
        {
            var glyphs = new List<Grapheme>();
            var buffer = new StringBuilder();
            int cluster = 0;
            int index = 0;
            int regionalIndicatorCount = 0;
            Rune? previousRune = null;
            foreach (var rune in text.EnumerateRunes())
            {
                var runeType = ClassifyRune(rune);

                switch (runeType)
                {
                    case RuneType.ZeroWidthJoiner:
                        if (!supportsComplexEmoji)
                        {
                            FlushBufferIfNeeded(buffer, glyphs, cluster);
                            return glyphs; // Terminal doesn't support complex emoji, stop parsing
                        }
                        HandleZeroWidthJoiner(rune, buffer, glyphs);
                        break;

                    case RuneType.Modifier:
                        AppendToCurrentOrLastGlyph(rune, buffer, glyphs);
                        break;

                    case RuneType.RegionalIndicator:
                        HandleRegionalIndicator(rune, ref regionalIndicatorCount, buffer, glyphs, ref cluster, index);
                        break;

                    case RuneType.VariationSelector:
                        AppendToCurrentOrLastGlyph(rune, buffer, glyphs);
                        break;

                    case RuneType.Emoji:
                        HandleEmoji(rune, previousRune, supportsComplexEmoji, buffer, glyphs, ref cluster, index, ref regionalIndicatorCount);
                        break;

                    case RuneType.Regular:
                        if (buffer.Length > 0)
                        {
                            glyphs.Add(new Grapheme { Glyph = buffer.ToString(), Cluster = cluster });
                            cluster = index;
                            buffer.Clear();
                        }
                        glyphs.Add(new Grapheme { Glyph = rune.ToString(), Cluster = cluster });
                        cluster = index + rune.Utf16SequenceLength;
                        break;
                }

                previousRune = rune;
                index += rune.Utf16SequenceLength;
            }

            FlushBufferIfNeeded(buffer, glyphs, cluster);
            return glyphs;
        }

        private enum RuneType
        {
            ZeroWidthJoiner,
            Modifier,
            RegionalIndicator,
            VariationSelector,
            Emoji,
            Regular
        }

        private static RuneType ClassifyRune(Rune rune)
        {
            if (rune.Value == Codepoints.ZWJ || rune.Value == Codepoints.ORC)
                return RuneType.ZeroWidthJoiner;

            if (IsModifier(rune))
                return RuneType.Modifier;

            if (IsRegionalIndicator(rune))
                return RuneType.RegionalIndicator;

            if (IsVariationSelector(rune))
                return RuneType.VariationSelector;

            if (Emoji.IsEmoji(rune.ToString()))
                return RuneType.Emoji;

            return RuneType.Regular;
        }

        private static bool IsModifier(Rune rune) =>
            (rune.Value >= Emoji.SkinTones.Light && rune.Value <= Emoji.SkinTones.Dark) ||
            rune.Value == Codepoints.Keycap;

        private static bool IsRegionalIndicator(Rune rune) =>
            rune.Value >= 0x1F1E6 && rune.Value <= 0x1F1FF;

        private static bool IsVariationSelector(Rune rune) =>
            rune.Value == Codepoints.VariationSelectors.EmojiSymbol ||
            rune.Value == Codepoints.VariationSelectors.TextSymbol;

        private static bool HandleZeroWidthJoiner(Rune rune, StringBuilder buffer, List<Grapheme> glyphs)
        {
            AppendToCurrentOrLastGlyph(rune, buffer, glyphs);
            return true;
        }

        private static void AppendToCurrentOrLastGlyph(Rune rune, StringBuilder buffer, List<Grapheme> glyphs)
        {
            if (buffer.Length > 0)
                buffer.Append(rune);
            else if (glyphs.Count > 0)
                glyphs[^1].Glyph += rune.ToString();
        }

        private static void HandleRegionalIndicator(Rune rune, ref int regionalIndicatorCount, StringBuilder buffer,
            List<Grapheme> glyphs, ref int cluster, int index)
        {
            regionalIndicatorCount++;

            if (regionalIndicatorCount % 2 == 0 && buffer.Length > 0)
            {
                // Complete the flag pair
                buffer.Append(rune);
                glyphs.Add(new Grapheme { Glyph = buffer.ToString(), Cluster = cluster });
                cluster = index + rune.Utf16SequenceLength;
                buffer.Clear();
            }
            else
            {
                // Start a new regional indicator sequence
                if (buffer.Length > 0)
                {
                    glyphs.Add(new Grapheme { Glyph = buffer.ToString(), Cluster = cluster });
                    cluster = index + rune.Utf16SequenceLength;
                    buffer.Clear();
                }
                buffer.Append(rune);
            }
        }

        private static void HandleEmoji(Rune rune, Rune? previousRune, bool supportsComplexEmoji,
            StringBuilder buffer, List<Grapheme> glyphs, ref int cluster, int index, ref int regionalIndicatorCount)
        {
            bool continueBuilding = supportsComplexEmoji && previousRune.HasValue &&
                                   (previousRune.Value.Value == Codepoints.ZWJ ||
                                    previousRune.Value.Value == Codepoints.ORC);

            if (continueBuilding)
            {
                buffer.Append(rune);
            }
            else if (buffer.Length > 0)
            {
                // Flush existing emoji and start new one
                glyphs.Add(new Grapheme { Glyph = buffer.ToString(), Cluster = cluster });
                cluster = index;
                buffer.Clear();
                regionalIndicatorCount = 0;
                buffer.Append(rune);
            }
            else
            {
                regionalIndicatorCount = 0;
                buffer.Append(rune);
            }
        }

        private static void FlushBufferIfNeeded(StringBuilder buffer, List<Grapheme> glyphs, int cluster)
        {
            if (buffer.Length > 0)
            {
                glyphs.Add(new Grapheme { Glyph = buffer.ToString(), Cluster = cluster });
                buffer.Clear();
            }
        }
    }
}