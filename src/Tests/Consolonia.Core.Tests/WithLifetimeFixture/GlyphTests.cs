using System.Collections.Generic;
using Consolonia.Controls;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Consolonia.Core.Helpers;
using NUnit.Framework;

namespace Consolonia.Core.Tests.WithLifetimeFixture
{
    [TestFixture]
    public class GlyphTests
    {
        [Test]
        public void GetgraphemesEmptyStringReturnsEmptyList()
        {
            string text = string.Empty;
            var graphemes = Grapheme.Parse(text, true);
            Assert.IsEmpty(graphemes);
        }

        [Test]
        public void GetgraphemesSingleCharacterReturnsSingleGlyph()
        {
            string text = "a";
            Assert.AreEqual(1, text.MeasureText());

            var graphemes = Grapheme.Parse(text, true);
            Assert.AreEqual(1, graphemes.Count);
            Assert.AreEqual("a", graphemes[0].Glyph);
            Assert.AreEqual(0, graphemes[0].Cluster);
        }

        [Test]
        public void GetgraphemesMultipleCharsReturnsMultipleGlyph()
        {
            string text = "hello";
            Assert.AreEqual(5, text.MeasureText());

            var graphemes = Grapheme.Parse(text, true);
            Assert.AreEqual(5, graphemes.Count);
            Assert.AreEqual("h", graphemes[0].Glyph);
            Assert.AreEqual(0, graphemes[0].Cluster);
            Assert.AreEqual("e", graphemes[1].Glyph);
            Assert.AreEqual(1, graphemes[1].Cluster);
            Assert.AreEqual("l", graphemes[2].Glyph);
            Assert.AreEqual(2, graphemes[2].Cluster);
            Assert.AreEqual("l", graphemes[3].Glyph);
            Assert.AreEqual(3, graphemes[3].Cluster);
            Assert.AreEqual("o", graphemes[4].Glyph);
            Assert.AreEqual(4, graphemes[4].Cluster);
        }

        [Test]
        public void GetgraphemesComplexCharsReturnsSingleGlyph()
        {
            string text = "𝔉𝔞𝔫𝔠𝔶";
            Assert.AreEqual(5, text.MeasureText());

            var graphemes = Grapheme.Parse(text, true);
            Assert.AreEqual(5, graphemes.Count);
            Assert.AreEqual("𝔉", graphemes[0].Glyph);
            Assert.AreEqual(0, graphemes[0].Cluster);
            Assert.AreEqual("𝔞", graphemes[1].Glyph);
            Assert.AreEqual(2, graphemes[1].Cluster);
            Assert.AreEqual("𝔫", graphemes[2].Glyph);
            Assert.AreEqual(4, graphemes[2].Cluster);
            Assert.AreEqual("𝔠", graphemes[3].Glyph);
            Assert.AreEqual(6, graphemes[3].Cluster);
            Assert.AreEqual("𝔶", graphemes[4].Glyph);
            Assert.AreEqual(8, graphemes[4].Cluster);
        }

        [Test]
        public void GetgraphemesSingleEmojiReturnsSingleGlyph()
        {
            string text = "👍";
            Assert.AreEqual(2, text.MeasureText());

            var graphemes = Grapheme.Parse(text, true);
            Assert.AreEqual(1, graphemes.Count);
            Assert.AreEqual("👍", graphemes[0].Glyph);
            Assert.AreEqual(0, graphemes[0].Cluster);
        }

        [Test]
        [TestCase("1\uFE0f\u20e3")]
        [TestCase("👍🏻")]
        [TestCase("👨‍👩‍👧‍👦")]
        public void GetgraphemesWithComplexEmoji(string text)
        {
            Assert.AreEqual(2, text.MeasureText());

            var graphemes = Grapheme.Parse(text, true);
            Assert.AreEqual(1, graphemes.Count);
            Assert.AreEqual(text, graphemes[0].Glyph);
            Assert.AreEqual(0, graphemes[0].Cluster);
        }

        [Test]
        public void GetgraphemesWithMultiplegraphemes()
        {
            string text = "a👍";
            Assert.AreEqual(3, text.MeasureText());

            var graphemes = Grapheme.Parse(text, true);
            Assert.AreEqual(2, graphemes.Count);
            Assert.AreEqual("a", graphemes[0].Glyph);
            Assert.AreEqual(0, graphemes[0].Cluster);
            Assert.AreEqual("👍", graphemes[1].Glyph);
            Assert.AreEqual(1, graphemes[1].Cluster);
        }

        [Test]
        public void GetgraphemesWithOutComplexEmojiSupport()
        {
            string text = "👨‍👩‍👧‍👦";
            Assert.AreEqual(2, text.MeasureText());

            var graphemes = Grapheme.Parse(text, false);
            Assert.AreEqual(1, graphemes.Count);
            Assert.AreEqual("👨", graphemes[0].Glyph);
            Assert.AreEqual(0, graphemes[0].Cluster);
        }

        [Test]
        [TestCase("x")]
        [TestCase("🠷")]
        [TestCase("⚙")]
        [TestCase("👨‍👩‍👧‍👦")]
        [TestCase("☰")]
        public void GetgraphemesEmojiWithTextPresentation(string text)
        {
            // Emoji followed by FE0E (text presentation selector)
            // The emoji should be rendered as text (single-wide)
            text = $"{text}\uFE0E";
            Assert.AreEqual(1, text.MeasureText());

            var graphemes = Grapheme.Parse(text, true);
            Assert.AreEqual(1, graphemes.Count);
            Assert.AreEqual(text, graphemes[0].Glyph);
            Assert.AreEqual(0, graphemes[0].Cluster);
        }

        [Test]
        [TestCase("x")]
        [TestCase("🠷")]
        [TestCase("⚙")]
        [TestCase("👨‍👩‍👧‍👦")]
        [TestCase("☰")]
        public void GetgraphemesEmojiWithEmojiPresentation(string text)
        {
            // Emoji followed by FE0F (emoji presentation selector)
            // The emoji should be rendered as emoji (double-wide)
            text = $"{text}\uFE0F";
            Assert.AreEqual(2, text.MeasureText());

            var graphemes = Grapheme.Parse(text, true);
            Assert.AreEqual(1, graphemes.Count);
            Assert.AreEqual(text, graphemes[0].Glyph);
            Assert.AreEqual(0, graphemes[0].Cluster);
        }

        [Test]
        [TestCase("⚙")]
        [TestCase("👨‍👩‍👧‍👦")]
        [TestCase("☰")]
        public void GetgraphemesAutoEmojiPresentation(string text)
        {
            // 🗙 (U+1F5D9) followed by FE0F (emoji presentation selector)
            Assert.AreEqual(2, text.MeasureText());

            var graphemes = Grapheme.Parse(text, true);
            Assert.AreEqual(1, graphemes.Count);
            Assert.AreEqual(text, graphemes[0].Glyph);
            Assert.AreEqual(0, graphemes[0].Cluster);

            var symbol = new Symbol(graphemes[0].Glyph);
            Assert.AreEqual($"{text}\ufe0f", symbol.Complex);
        }

        [Test]
        public void GetgraphemesCancelSignWithTextPresentation()
        {
            // 🗙 (U+1F5D9) followed by FE0E (text presentation selector)
            string text = "🗙\uFE0E";
            Assert.AreEqual(1, text.MeasureText());

            var graphemes = Grapheme.Parse(text, true);
            Assert.AreEqual(1, graphemes.Count);
            Assert.AreEqual("🗙\uFE0E", graphemes[0].Glyph);
            Assert.AreEqual(0, graphemes[0].Cluster);
        }

        [Test]
        public void GetgraphemesMultipleEmojisWithVariationSelectors()
        {
            // Mix of emojis with variation selectors
            string text = "☺\uFE0E☺\uFE0F";
            Assert.AreEqual(3, text.MeasureText()); // 1 (text) + 2 (emoji)

            var graphemes = Grapheme.Parse(text, true);
            Assert.AreEqual(2, graphemes.Count);
            Assert.AreEqual("☺\uFE0E", graphemes[0].Glyph);
            Assert.AreEqual(0, graphemes[0].Cluster);
            Assert.AreEqual("☺\uFE0F", graphemes[1].Glyph);
            Assert.AreEqual(2, graphemes[1].Cluster);
        }

        [Test]
        public void GetgraphemesMultipleEmojisWithoutVariationSelectors()
        {
            // Mix of emojis with variation selectors
            string text = "🏳️‍🌈🏳️‍🌈";
            Assert.AreEqual(4, text.MeasureText());

            var graphemes = Grapheme.Parse(text, true);
            Assert.AreEqual(2, graphemes.Count);
            Assert.AreEqual("🏳️‍🌈", graphemes[0].Glyph);
            Assert.AreEqual(0, graphemes[0].Cluster);
            Assert.AreEqual("🏳️‍🌈", graphemes[1].Glyph);
            Assert.AreEqual(6, graphemes[1].Cluster);
        }

        // Add regional indicator tests below
        [Test]
        public void GetgraphemesRegionalIndicatorSingleFlag()
        {
            // US flag: 🇺🇸 (U+1F1FA U+1F1F8)
            string text = "🇺🇸flag";
            Assert.AreEqual(6, text.MeasureText());

            var graphemes = Grapheme.Parse(text, true);
            Assert.AreEqual(5, graphemes.Count);
            Assert.AreEqual("🇺🇸", graphemes[0].Glyph);
            Assert.AreEqual(0, graphemes[0].Cluster);
            Assert.AreEqual("f", graphemes[1].Glyph);
            Assert.AreEqual(4, graphemes[1].Cluster);
            Assert.AreEqual("l", graphemes[2].Glyph);
            Assert.AreEqual(5, graphemes[2].Cluster);
            Assert.AreEqual("a", graphemes[3].Glyph);
            Assert.AreEqual(6, graphemes[3].Cluster);
            Assert.AreEqual("g", graphemes[4].Glyph);
            Assert.AreEqual(7, graphemes[4].Cluster);
        }

        [Test]
        public void GetgraphemesRegionalIndicatorMultipleFlags()
        {
            // US flag + UK flag: 🇺🇸🇬🇧
            string text = "🇺🇸🇬🇧";
            Assert.AreEqual(4, text.MeasureText());

            var graphemes = Grapheme.Parse(text, true);
            Assert.AreEqual(2, graphemes.Count);
            Assert.AreEqual("🇺🇸", graphemes[0].Glyph);
            Assert.AreEqual(0, graphemes[0].Cluster);
            Assert.AreEqual("🇬🇧", graphemes[1].Glyph);
            Assert.AreEqual(4, graphemes[1].Cluster);
        }

        [Test]
        public void GetgraphemesRegionalIndicatorWithText()
        {
            // Text with flag: "Hello 🇺🇸"
            string text = "Hello 🇺🇸!";
            Assert.AreEqual(9, text.MeasureText()); // H-e-l-l-o-space + flag(2)

            var graphemes = Grapheme.Parse(text, true);
            Assert.AreEqual(8, graphemes.Count);
            Assert.AreEqual("H", graphemes[0].Glyph);
            Assert.AreEqual(0, graphemes[0].Cluster);
            Assert.AreEqual("e", graphemes[1].Glyph);
            Assert.AreEqual(1, graphemes[1].Cluster);
            Assert.AreEqual("l", graphemes[2].Glyph);
            Assert.AreEqual(2, graphemes[2].Cluster);
            Assert.AreEqual("l", graphemes[3].Glyph);
            Assert.AreEqual(3, graphemes[3].Cluster);
            Assert.AreEqual("o", graphemes[4].Glyph);
            Assert.AreEqual(4, graphemes[4].Cluster);
            Assert.AreEqual(" ", graphemes[5].Glyph);
            Assert.AreEqual(5, graphemes[5].Cluster);
            Assert.AreEqual("🇺🇸", graphemes[6].Glyph);
            Assert.AreEqual(6, graphemes[6].Cluster);
            Assert.AreEqual("!", graphemes[7].Glyph);
            Assert.AreEqual(10, graphemes[7].Cluster);
        }

        [Test]
        [TestCase("🇦🇺")] // Australia
        [TestCase("🇨🇦")] // Canada
        [TestCase("🇩🇪")] // Germany
        [TestCase("🇫🇷")] // France
        [TestCase("🇯🇵")] // Japan
        public void GetgraphemesRegionalIndicatorVariousFlags(string text)
        {
            Assert.AreEqual(2, text.MeasureText());

            var graphemes = Grapheme.Parse(text, true);
            Assert.AreEqual(1, graphemes.Count);
            Assert.AreEqual(text, graphemes[0].Glyph);
            Assert.AreEqual(0, graphemes[0].Cluster);
        }
    }
}