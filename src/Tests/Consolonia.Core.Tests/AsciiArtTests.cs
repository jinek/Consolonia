using Avalonia.Media;
using Consolonia.Core.Text.Fonts;
using NUnit.Framework;

namespace Consolonia.Core.Tests
{
    [TestFixture]
    public class AsciiArtGlyphTests
    {
        private AsciiArtTypeface _typeface;

        [SetUp]
        public void Setup()
        {
            _typeface = new AsciiArtTypeface("TestFont")
            {
                Hardblank = '$',
                Metrics = new FontMetrics
                {
                    DesignEmHeight = 5
                }
            };
        }

        [Test]
        public void ConstructorWithSimpleLineCreatesGlyph()
        {
            // Arrange
            uint codepoint = 'A';
            string[] lines = ["***", "* *", "***"];

            // Act
            var glyph = new AsciiArtGlyph(_typeface, codepoint, lines);

            // Assert
            Assert.AreEqual(_typeface, glyph.Typeface);
            Assert.AreEqual(codepoint, glyph.Codepoint);
            Assert.AreEqual(3, glyph.Lines.Length);
            Assert.AreEqual(3, glyph.Height);
            Assert.AreEqual(3, glyph.Width);
        }

        [Test]
        public void ConstructorWithEmptyLinesCreatesEmptyGlyph()
        {
            // Arrange
            uint codepoint = ' ';
            string[] lines = ["  ", "  ", "  "];

            // Act
            var glyph = new AsciiArtGlyph(_typeface, codepoint, lines);

            // Assert
            Assert.AreEqual(3, glyph.Height);
            Assert.AreEqual(2, glyph.Width);
            Assert.AreEqual(3, glyph.Lines.Length);
        }

        [Test]
        public void ConstructorWithHardblankReplacesWithSpace()
        {
            // Arrange
            uint codepoint = 'A';
            string[] lines = ["$$$", "$ $", "$$$"];

            // Act
            var glyph = new AsciiArtGlyph(_typeface, codepoint, lines);

            // Assert
            Assert.AreEqual(3, glyph.GraphemeLines.Length);
            foreach (var line in glyph.GraphemeLines)
            {
                foreach (var grapheme in line)
                {
                    Assert.IsFalse(grapheme.Glyph.Contains("$"));
                }
            }
        }

        [Test]
        public void ConstructorWithUnicodeEscapesDecodesCorrectly()
        {
            // Arrange
            uint codepoint = 'A';
            string[] lines = ["\\u0041", "\\u0042", "\\u0043"]; // A, B, C

            // Act
            var glyph = new AsciiArtGlyph(_typeface, codepoint, lines);

            // Assert
            Assert.AreEqual("A", glyph.GraphemeLines[0][0].Glyph);
            Assert.AreEqual("B", glyph.GraphemeLines[1][0].Glyph);
            Assert.AreEqual("C", glyph.GraphemeLines[2][0].Glyph);
        }

        [Test]
        public void ConstructorCalculatesCorrectWidth()
        {
            // Arrange
            uint codepoint = 'A';
            string[] lines = ["*", "**", "***"];

            // Act
            var glyph = new AsciiArtGlyph(_typeface, codepoint, lines);

            // Assert
            Assert.AreEqual(3, glyph.Width); // Width is max of all lines
        }

        [Test]
        public void ConstructorCalculatesStartsAndEndsCorrectly()
        {
            // Arrange
            uint codepoint = 'A';
            string[] lines = ["  *  ", " *** ", "*****"];

            // Act
            var glyph = new AsciiArtGlyph(_typeface, codepoint, lines);

            // Assert
            Assert.AreEqual(2, glyph.Starts[0]); // First non-space at index 2
            Assert.AreEqual(2, glyph.Ends[0]);   // Last non-space at index 2

            Assert.AreEqual(1, glyph.Starts[1]); // First non-space at index 1
            Assert.AreEqual(3, glyph.Ends[1]);   // Last non-space at index 3

            Assert.AreEqual(0, glyph.Starts[2]); // First non-space at index 0
            Assert.AreEqual(4, glyph.Ends[2]);   // Last non-space at index 4
        }

        [Test]
        public void ConstructorWithOnlySpacesStartsEqualsLineLength()
        {
            // Arrange
            uint codepoint = ' ';
            string[] lines = ["     "];

            // Act
            var glyph = new AsciiArtGlyph(_typeface, codepoint, lines);

            // Assert
            Assert.AreEqual(5, glyph.Starts[0]); // No non-space characters found
            Assert.AreEqual(0, glyph.Ends[0]);   // No non-space characters found
        }

        [Test]
        public void ConstructorWithGraphemesParsesCorrectly()
        {
            // Arrange
            uint codepoint = 'A';
            string[] lines = ["ABC", "DEF"];

            // Act
            var glyph = new AsciiArtGlyph(_typeface, codepoint, lines);

            // Assert
            Assert.AreEqual(2, glyph.GraphemeLines.Length);
            Assert.AreEqual(3, glyph.GraphemeLines[0].Length);
            Assert.AreEqual("A", glyph.GraphemeLines[0][0].Glyph);
            Assert.AreEqual("B", glyph.GraphemeLines[0][1].Glyph);
            Assert.AreEqual("C", glyph.GraphemeLines[0][2].Glyph);
        }

        [Test]
        public void ConstructorWithComplexUnicodeHandlesCorrectly()
        {
            // Arrange
            uint codepoint = 'A';
            string[] lines = ["\\u00A9", "\\u00AE"]; // © and ®

            // Act
            var glyph = new AsciiArtGlyph(_typeface, codepoint, lines);

            // Assert
            Assert.AreEqual("©", glyph.GraphemeLines[0][0].Glyph);
            Assert.AreEqual("®", glyph.GraphemeLines[1][0].Glyph);
        }

        [Test]
        public void ConstructorWithMultipleUnicodeEscapesDecodesAll()
        {
            // Arrange
            uint codepoint = 'A';
            string[] lines = ["\\u0041\\u0042\\u0043"]; // ABC

            // Act
            var glyph = new AsciiArtGlyph(_typeface, codepoint, lines);

            // Assert
            Assert.AreEqual(3, glyph.GraphemeLines[0].Length);
            Assert.AreEqual("A", glyph.GraphemeLines[0][0].Glyph);
            Assert.AreEqual("B", glyph.GraphemeLines[0][1].Glyph);
            Assert.AreEqual("C", glyph.GraphemeLines[0][2].Glyph);
        }

        [Test]
        public void ConstructorWithMixedContentAndSpacesCalculatesWidthCorrectly()
        {
            // Arrange
            uint codepoint = 'T';
            string[] lines = ["*****", "  *  ", "  *  ", "  *  "];

            // Act
            var glyph = new AsciiArtGlyph(_typeface, codepoint, lines);

            // Assert
            Assert.AreEqual(5, glyph.Width);
            Assert.AreEqual(4, glyph.Height);
        }

        [Test]
        public void HeightReturnsNumberOfLines()
        {
            // Arrange
            uint codepoint = 'A';
            string[] lines = ["*", "**", "***", "****"];

            // Act
            var glyph = new AsciiArtGlyph(_typeface, codepoint, lines);

            // Assert
            Assert.AreEqual(4, glyph.Height);
            Assert.AreEqual(lines.Length, glyph.Height);
        }

        [Test]
        public void ConstructorWithSingleCharacterLineCalculatesStartAndEndCorrectly()
        {
            // Arrange
            uint codepoint = 'I';
            string[] lines = ["*"];

            // Act
            var glyph = new AsciiArtGlyph(_typeface, codepoint, lines);

            // Assert
            Assert.AreEqual(0, glyph.Starts[0]);
            Assert.AreEqual(0, glyph.Ends[0]);
            Assert.AreEqual(1, glyph.Width);
        }

        [Test]
        public void ConstructorWithTrailingSpacesCalculatesEndsCorrectly()
        {
            // Arrange
            uint codepoint = 'L';
            string[] lines = ["*    ", "*    ", "***  "];

            // Act
            var glyph = new AsciiArtGlyph(_typeface, codepoint, lines);

            // Assert
            Assert.AreEqual(0, glyph.Ends[0]);
            Assert.AreEqual(0, glyph.Ends[1]);
            Assert.AreEqual(2, glyph.Ends[2]);
        }

        [Test]
        public void ConstructorWithLeadingSpacesCalculatesStartsCorrectly()
        {
            // Arrange
            uint codepoint = 'R';
            string[] lines = ["    *", "   **", "  ***"];

            // Act
            var glyph = new AsciiArtGlyph(_typeface, codepoint, lines);

            // Assert
            Assert.AreEqual(4, glyph.Starts[0]);
            Assert.AreEqual(3, glyph.Starts[1]);
            Assert.AreEqual(2, glyph.Starts[2]);
        }

        [Test]
        public void ConstructorWithVariableLineWidthsUsesMaxWidth()
        {
            // Arrange
            uint codepoint = 'X';
            string[] lines = ["*", "***", "*****", "***", "*"];

            // Act
            var glyph = new AsciiArtGlyph(_typeface, codepoint, lines);

            // Assert
            Assert.AreEqual(5, glyph.Width); // Maximum width from middle line
        }
    }
}