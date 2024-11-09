using Avalonia.Media;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Consolonia.Core.Helpers;
using Consolonia.Core.Text;
using NUnit.Framework;

namespace Consolonia.Core.Tests
{
    [TestFixture]
    public class GlyphTests
    {
        [Test]
        public void GetGlyphsEmptyStringReturnsEmptyList()
        {
            var text = string.Empty;
            var glyphs = text.GetGlyphs(true);
            Assert.IsEmpty(glyphs);
        }

        [Test]
        public void GetGlyphsSingleCharacterReturnsSingleGlyph()
        {
            var text = "a";
            Assert.AreEqual(1, text.MeasureText());

            var glyphs = text.GetGlyphs(true);
            Assert.AreEqual(1, glyphs.Count);
            Assert.AreEqual("a", glyphs[0]);
        }

        [Test]
        public void GetGlyphsMultipleCharsReturnsMultipleGlyph()
        {
            var text = "hello";
            Assert.AreEqual(5, text.MeasureText());

            var glyphs = text.GetGlyphs(true);
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
            var text = "𝔉𝔞𝔫𝔠𝔶";
            Assert.AreEqual(5, text.MeasureText());

            var glyphs = text.GetGlyphs(true);
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
            var text = "👍";
            Assert.AreEqual(2, text.MeasureText());

            var glyphs = text.GetGlyphs(true);
            Assert.AreEqual(1, glyphs.Count);
            Assert.AreEqual("👍", glyphs[0]);
        }

        [Test]
        public void GetGlyphsWithComplexEmoji()
        {
            var text = "👨‍👩‍👧‍👦";
            Assert.AreEqual(2, text.MeasureText());

            var glyphs = text.GetGlyphs(true);
            Assert.AreEqual(1, glyphs.Count);
            Assert.AreEqual("👨‍👩‍👧‍👦", glyphs[0]);
        }

        [Test]
        public void GetGlyphsWithMultipleGlyphs()
        {
            var text = "a👍";
            Assert.AreEqual(3, text.MeasureText());

            var glyphs = text.GetGlyphs(true);
            Assert.AreEqual(2, glyphs.Count);
            Assert.AreEqual("a", glyphs[0]);
            Assert.AreEqual("👍", glyphs[1]);
        }

        [Test]
        public void GetGlyphsWithOutComplexEmojiSupport()
        {
            var text = "👨‍👩‍👧‍👦";
            Assert.AreEqual(2, text.MeasureText());

            var glyphs = text.GetGlyphs(false);
            Assert.AreEqual(4, glyphs.Count);
            Assert.AreEqual("👨", glyphs[0]);
            Assert.AreEqual("👩", glyphs[1]);
            Assert.AreEqual("👧", glyphs[2]);
            Assert.AreEqual("👦", glyphs[3]);
        }
    }
}