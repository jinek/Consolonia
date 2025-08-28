using System.Collections.Generic;
using Consolonia.Controls;
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
        public void GetGlyphsWithComplexEmoji()
        {
            string text = "👨‍👩‍👧‍👦";
            Assert.AreEqual(2, text.MeasureText());

            IReadOnlyList<string> glyphs = text.GetGlyphs(true);
            Assert.AreEqual(1, glyphs.Count);
            Assert.AreEqual("👨‍👩‍👧‍👦", glyphs[0]);
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
            Assert.AreEqual(4, glyphs.Count);
            Assert.AreEqual("👨", glyphs[0]);
            Assert.AreEqual("👩", glyphs[1]);
            Assert.AreEqual("👧", glyphs[2]);
            Assert.AreEqual("👦", glyphs[3]);
        }
    }
}