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
        public void GetGlyphsEmptyStringReturnsEmptyList()
        {
            string text = string.Empty;
            IReadOnlyList<string> glyphs = text.GetGlyphs(true);
            Assert.IsEmpty(glyphs);
        }

        [Test]
        public void GetGlyphsSingleCharacterReturnsSingleGlyph()
        {
            string text = "a";
            Assert.AreEqual(1, text.MeasureText());

            IReadOnlyList<string> glyphs = text.GetGlyphs(true);
            Assert.AreEqual(1, glyphs.Count);
            Assert.AreEqual("a", glyphs[0]);
        }

        [Test]
        public void GetGlyphsMultipleCharsReturnsMultipleGlyph()
        {
            string text = "hello";
            Assert.AreEqual(5, text.MeasureText());

            IReadOnlyList<string> glyphs = text.GetGlyphs(true);
            Assert.AreEqual(5, glyphs.Count);
            Assert.AreEqual("h", glyphs[0]);
            Assert.AreEqual("e", glyphs[1]);
            Assert.AreEqual("l", glyphs[2]);
            Assert.AreEqual("l", glyphs[3]);
            Assert.AreEqual("o", glyphs[4]);
        }

        [Test]
        public void GetGlyphsComplexCharsReturnsSingleGlyph()
        {
            string text = "𝔉𝔞𝔫𝔠𝔶";
            Assert.AreEqual(5, text.MeasureText());

            IReadOnlyList<string> glyphs = text.GetGlyphs(true);
            Assert.AreEqual(5, glyphs.Count);
            Assert.AreEqual("𝔉", glyphs[0]);
            Assert.AreEqual("𝔞", glyphs[1]);
            Assert.AreEqual("𝔫", glyphs[2]);
            Assert.AreEqual("𝔠", glyphs[3]);
            Assert.AreEqual("𝔶", glyphs[4]);
        }

        [Test]
        public void GetGlyphsSingleEmojiReturnsSingleGlyph()
        {
            string text = "👍";
            Assert.AreEqual(2, text.MeasureText());

            IReadOnlyList<string> glyphs = text.GetGlyphs(true);
            Assert.AreEqual(1, glyphs.Count);
            Assert.AreEqual("👍", glyphs[0]);
        }

        [Test]
        [TestCase("1\uFE0f\u20e3")]
        [TestCase("👍🏻")]
        [TestCase("👨‍👩‍👧‍👦")]
        public void GetGlyphsWithComplexEmoji(string text)
        {
            Assert.AreEqual(2, text.MeasureText());

            IReadOnlyList<string> glyphs = text.GetGlyphs(true);
            Assert.AreEqual(1, glyphs.Count);
            Assert.AreEqual(text, glyphs[0]);
        }

        [Test]
        public void GetGlyphsWithMultipleGlyphs()
        {
            string text = "a👍";
            Assert.AreEqual(3, text.MeasureText());

            IReadOnlyList<string> glyphs = text.GetGlyphs(true);
            Assert.AreEqual(2, glyphs.Count);
            Assert.AreEqual("a", glyphs[0]);
            Assert.AreEqual("👍", glyphs[1]);
        }

        [Test]
        public void GetGlyphsWithOutComplexEmojiSupport()
        {
            string text = "👨‍👩‍👧‍👦";
            Assert.AreEqual(2, text.MeasureText());

            IReadOnlyList<string> glyphs = text.GetGlyphs(false);
            Assert.AreEqual(1, glyphs.Count);
            Assert.AreEqual("👨", glyphs[0]);
        }

        [Test]
        [TestCase("x")]
        [TestCase("🠷")]
        [TestCase("⚙")]
        [TestCase("👨‍👩‍👧‍👦")]
        [TestCase("☰")]
        public void GetGlyphsEmojiWithTextPresentation(string text)
        {
            // Emoji followed by FE0E (text presentation selector)
            // The emoji should be rendered as text (single-wide)
            text = $"{text}\uFE0E";
            Assert.AreEqual(1, text.MeasureText());

            IReadOnlyList<string> glyphs = text.GetGlyphs(true);
            Assert.AreEqual(1, glyphs.Count);
            Assert.AreEqual(text, glyphs[0]);
        }

        [Test]
        [TestCase("x")]
        [TestCase("🠷")]
        [TestCase("⚙")]
        [TestCase("👨‍👩‍👧‍👦")]
        [TestCase("☰")]
        public void GetGlyphsEmojiWithEmojiPresentation(string text)
        {
            // Emoji followed by FE0F (emoji presentation selector)
            // The emoji should be rendered as emoji (double-wide)
            text = $"{text}\uFE0F";
            Assert.AreEqual(2, text.MeasureText());

            IReadOnlyList<string> glyphs = text.GetGlyphs(true);
            Assert.AreEqual(1, glyphs.Count);
            Assert.AreEqual(text, glyphs[0]);
        }

        [Test]
        [TestCase("⚙")]
        [TestCase("👨‍👩‍👧‍👦")]
        [TestCase("☰")]
        public void GetGlyphsAutoEmojiPresentation(string text)
        {
            // 🗙 (U+1F5D9) followed by FE0F (emoji presentation selector)
            Assert.AreEqual(2, text.MeasureText());

            IReadOnlyList<string> glyphs = text.GetGlyphs(true);
            Assert.AreEqual(1, glyphs.Count);
            Assert.AreEqual(text, glyphs[0]);

            var symbol = new Symbol(glyphs[0]);
            Assert.AreEqual($"{text}\ufe0f", symbol.Complex);
        }

        [Test]
        public void GetGlyphsCancelSignWithTextPresentation()
        {
            // 🗙 (U+1F5D9) followed by FE0E (text presentation selector)
            string text = "🗙\uFE0E";
            Assert.AreEqual(1, text.MeasureText());

            IReadOnlyList<string> glyphs = text.GetGlyphs(true);
            Assert.AreEqual(1, glyphs.Count);
            Assert.AreEqual("🗙\uFE0E", glyphs[0]);
        }

        [Test]
        public void GetGlyphsMultipleEmojisWithVariationSelectors()
        {
            // Mix of emojis with variation selectors
            string text = "☺\uFE0E☺\uFE0F";
            Assert.AreEqual(3, text.MeasureText()); // 1 (text) + 2 (emoji)

            IReadOnlyList<string> glyphs = text.GetGlyphs(true);
            Assert.AreEqual(2, glyphs.Count);
            Assert.AreEqual("☺\uFE0E", glyphs[0]);
            Assert.AreEqual("☺\uFE0F", glyphs[1]);
        }

        [Test]
        public void GetGlyphsMultipleEmojisWithoutVariationSelectors()
        {
            // Mix of emojis with variation selectors
            string text = "🏳️‍🌈🏳️‍🌈";
            Assert.AreEqual(4, text.MeasureText());

            IReadOnlyList<string> glyphs = text.GetGlyphs(true);
            Assert.AreEqual(2, glyphs.Count);
            Assert.AreEqual("🏳️‍🌈", glyphs[0]);
            Assert.AreEqual("🏳️‍🌈", glyphs[1]);
        }

        // Add regional indicator tests below
        [Test]
        public void GetGlyphsRegionalIndicatorSingleFlag()
        {
            // US flag: 🇺🇸 (U+1F1FA U+1F1F8)
            string text = "🇺🇸flag";
            Assert.AreEqual(6, text.MeasureText());

            IReadOnlyList<string> glyphs = text.GetGlyphs(true);
            Assert.AreEqual(5, glyphs.Count);
            Assert.AreEqual("🇺🇸", glyphs[0]);
            Assert.AreEqual("f", glyphs[1]);
            Assert.AreEqual("l", glyphs[2]);
            Assert.AreEqual("a", glyphs[3]);
            Assert.AreEqual("g", glyphs[4]);
        }

        [Test]
        public void GetGlyphsRegionalIndicatorMultipleFlags()
        {
            // US flag + UK flag: 🇺🇸🇬🇧
            string text = "🇺🇸🇬🇧";
            Assert.AreEqual(4, text.MeasureText());

            IReadOnlyList<string> glyphs = text.GetGlyphs(true);
            Assert.AreEqual(2, glyphs.Count);
            Assert.AreEqual("🇺🇸", glyphs[0]);
            Assert.AreEqual("🇬🇧", glyphs[1]);
        }

        [Test]
        public void GetGlyphsRegionalIndicatorWithText()
        {
            // Text with flag: "Hello 🇺🇸"
            string text = "Hello 🇺🇸";
            Assert.AreEqual(8, text.MeasureText()); // H-e-l-l-o-space + flag(2)

            IReadOnlyList<string> glyphs = text.GetGlyphs(true);
            Assert.AreEqual(7, glyphs.Count);
            Assert.AreEqual("H", glyphs[0]);
            Assert.AreEqual("e", glyphs[1]);
            Assert.AreEqual("l", glyphs[2]);
            Assert.AreEqual("l", glyphs[3]);
            Assert.AreEqual("o", glyphs[4]);
            Assert.AreEqual(" ", glyphs[5]);
            Assert.AreEqual("🇺🇸", glyphs[6]);
        }

        [Test]
        [TestCase("🇦🇺")] // Australia
        [TestCase("🇨🇦")] // Canada
        [TestCase("🇩🇪")] // Germany
        [TestCase("🇫🇷")] // France
        [TestCase("🇯🇵")] // Japan
        public void GetGlyphsRegionalIndicatorVariousFlags(string text)
        {
            Assert.AreEqual(2, text.MeasureText());

            IReadOnlyList<string> glyphs = text.GetGlyphs(true);
            Assert.AreEqual(1, glyphs.Count);
            Assert.AreEqual(text, glyphs[0]);
        }
    }
}