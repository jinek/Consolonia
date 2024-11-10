using System.Linq;
using System.Text;
using Avalonia.Media;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using NUnit.Framework;

namespace Consolonia.Core.Tests
{
    [TestFixture]
    public class PixelForegroundTests
    {
        [Test]
        public void ConstructorWithSymbol()
        {
            var symbol = new SimpleSymbol('a');
            var pixelForeground = new PixelForeground(symbol, Colors.Red);
            Assert.That(pixelForeground.Color, Is.EqualTo(Colors.Red));
            Assert.That(pixelForeground.Symbol.Text, Is.EqualTo("a"));
            Assert.That(pixelForeground.Weight, Is.EqualTo(FontWeight.Normal));
            Assert.That(pixelForeground.Style, Is.EqualTo(FontStyle.Normal));
            Assert.That(pixelForeground.TextDecorations, Is.Null);
        }

        [Test]
        public void ConstructorWithSymbolAndWeight()
        {
            var symbol = new SimpleSymbol('a');
            var pixelForeground = new PixelForeground(symbol, Colors.Red, FontWeight.Bold);
            Assert.That(pixelForeground.Color, Is.EqualTo(Colors.Red));
            Assert.That(pixelForeground.Symbol.Text, Is.EqualTo("a"));
            Assert.That(pixelForeground.Weight, Is.EqualTo(FontWeight.Bold));
            Assert.That(pixelForeground.Style, Is.EqualTo(FontStyle.Normal));
            Assert.That(pixelForeground.TextDecorations, Is.Null);
        }

        [Test]
        public void ConstructorWithSymbolAndStyle()
        {
            var symbol = new SimpleSymbol('a');
            var pixelForeground = new PixelForeground(symbol, Colors.Red, style: FontStyle.Italic);
            Assert.That(pixelForeground.Color, Is.EqualTo(Colors.Red));
            Assert.That(pixelForeground.Symbol.Text, Is.EqualTo("a"));
            Assert.That(pixelForeground.Weight, Is.EqualTo(FontWeight.Normal));
            Assert.That(pixelForeground.Style, Is.EqualTo(FontStyle.Italic));
            Assert.That(pixelForeground.TextDecorations, Is.Null);
        }

        [Test]
        public void ConstructorWithSymbolAndTextDecorations()
        {
            var symbol = new SimpleSymbol('a');
            TextDecorationCollection textDecorations = TextDecorations.Underline;
            var pixelForeground = new PixelForeground(symbol, Colors.Red, textDecorations: textDecorations);
            Assert.That(pixelForeground.Color, Is.EqualTo(Colors.Red));
            Assert.That(pixelForeground.Symbol.Text, Is.EqualTo("a"));
            Assert.That(pixelForeground.Weight, Is.EqualTo(FontWeight.Normal));
            Assert.That(pixelForeground.Style, Is.EqualTo(FontStyle.Normal));
            Assert.That(pixelForeground.TextDecorations, Is.EqualTo(TextDecorations.Underline));
        }

        [Test]
        public void ConstructorWithWideCharacter()
        {
            Rune rune = "🎵".EnumerateRunes().First();
            var symbol = new SimpleSymbol(rune);
            var pixelForeground = new PixelForeground(symbol, Colors.Red);
            Assert.That(pixelForeground.Color, Is.EqualTo(Colors.Red));
            Assert.That(pixelForeground.Symbol.Text, Is.EqualTo("🎵"));
            Assert.That(pixelForeground.Weight, Is.EqualTo(FontWeight.Normal));
            Assert.That(pixelForeground.Style, Is.EqualTo(FontStyle.Normal));
            Assert.That(pixelForeground.TextDecorations, Is.Null);
        }

        [Test]
        public void Equality()
        {
            var pixelForeground = new PixelForeground(new SimpleSymbol('a'), Colors.Red);
            var pixelForeground2 = new PixelForeground(new SimpleSymbol('a'), Colors.Red);
            Assert.That(pixelForeground.Equals((object)pixelForeground2));
            Assert.That(pixelForeground.Equals(pixelForeground2));
            Assert.That(pixelForeground == pixelForeground2, Is.True);
        }

        [Test]
        public void Inequality()
        {
            var pixelForeground = new PixelForeground(new SimpleSymbol('a'), Colors.Red);
            foreach (var variation in new PixelForeground[]
                                    {
                                        new(new SimpleSymbol('b'), Colors.Red),
                                        new(new SimpleSymbol('a'), Colors.Blue),
                                        new(new SimpleSymbol('a'), Colors.Red, FontWeight.Bold),
                                        new(new SimpleSymbol('a'), Colors.Red, style: FontStyle.Italic),
                                        new(new SimpleSymbol('a'), Colors.Red, textDecorations: TextDecorations.Underline),
                                    })
            {
                Assert.That(!pixelForeground.Equals((object)variation));
                Assert.That(!pixelForeground.Equals(variation));
                Assert.That(pixelForeground != variation, Is.True);
            }
        }

        [Test]
        public void Blend()
        {
            var symbol = new SimpleSymbol('a');
            var pixelForeground = new PixelForeground(symbol, Colors.Red);
            var symbolAbove = new SimpleSymbol('b');
            var pixelForegroundAbove = new PixelForeground(symbolAbove, Colors.Blue);
            PixelForeground newPixelForeground = pixelForeground.Blend(pixelForegroundAbove);
            Assert.That(newPixelForeground.Color, Is.EqualTo(Colors.Blue));
            Assert.That(newPixelForeground.Symbol.Text, Is.EqualTo("b"));
        }

        [Test]
        public void BlendComplex()
        {
            var symbol = new SimpleSymbol('a');
            var pixelForeground = new PixelForeground(symbol, Colors.Red);
            var symbolAbove = new SimpleSymbol('b');
            var pixelForegroundAbove = new PixelForeground(symbolAbove, Colors.Blue, FontWeight.Bold, FontStyle.Italic,
                TextDecorations.Underline);
            PixelForeground newPixelForeground = pixelForeground.Blend(pixelForegroundAbove);
            Assert.That(newPixelForeground.Color, Is.EqualTo(Colors.Blue));
            Assert.That(newPixelForeground.Symbol.Text, Is.EqualTo("b"));
            Assert.That(newPixelForeground.Weight, Is.EqualTo(FontWeight.Bold));
            Assert.That(newPixelForeground.Style, Is.EqualTo(FontStyle.Italic));
            Assert.That(newPixelForeground.TextDecorations, Is.EqualTo(TextDecorations.Underline));
        }

        [Test]
        public void BlendEmoji()
        {
            var symbol = new SimpleSymbol("🎵");
            var pixelForeground = new PixelForeground(symbol, Colors.Red);
            var symbolAbove = new SimpleSymbol("🎶");
            var pixelForegroundAbove = new PixelForeground(symbolAbove, Colors.Blue);
            PixelForeground newPixelForeground = pixelForeground.Blend(pixelForegroundAbove);
            Assert.That(newPixelForeground.Color, Is.EqualTo(Colors.Blue));
            Assert.That(newPixelForeground.Symbol.Text, Is.EqualTo("🎶"));
        }

        [Test]
        public void HashCode()
        {
            var pixelForeground = new PixelForeground(new SimpleSymbol('a'), Colors.Red);
            var pixelForeground2 = new PixelForeground(new SimpleSymbol('a'), Colors.Red);
            Assert.That(pixelForeground.GetHashCode(), Is.EqualTo(pixelForeground2.GetHashCode()));

            // inequal test
            pixelForeground = new PixelForeground(new SimpleSymbol('a'), Colors.Red);
            pixelForeground2 = new PixelForeground(new SimpleSymbol('b'), Colors.Red);
            Assert.That(pixelForeground.GetHashCode(), Is.Not.EqualTo(pixelForeground2.GetHashCode()));
            
            pixelForeground = new PixelForeground(new SimpleSymbol('a'), Colors.Red);
            pixelForeground2 = new PixelForeground(new SimpleSymbol('a'), Colors.Blue);
            Assert.That(pixelForeground.GetHashCode(), Is.Not.EqualTo(pixelForeground2.GetHashCode()));
        }
    }
}