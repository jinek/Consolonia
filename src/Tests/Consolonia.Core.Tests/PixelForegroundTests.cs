using System.Linq;
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
            var textDecorations = TextDecorations.Underline;
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
            var rune = "🎵".EnumerateRunes().First();
            var symbol = new SimpleSymbol(rune);
            var pixelForeground = new PixelForeground(symbol, Colors.Red);
            Assert.That(pixelForeground.Color, Is.EqualTo(Colors.Red));
            Assert.That(pixelForeground.Symbol.Text, Is.EqualTo("🎵"));
            Assert.That(pixelForeground.Weight, Is.EqualTo(FontWeight.Normal));
            Assert.That(pixelForeground.Style, Is.EqualTo(FontStyle.Normal));
            Assert.That(pixelForeground.TextDecorations, Is.Null);
        }

        [Test]
        public void Blend()
        {
            var symbol = new SimpleSymbol('a');
            var pixelForeground = new PixelForeground(symbol, Colors.Red);
            var symbolAbove = new SimpleSymbol('b');
            var pixelForegroundAbove = new PixelForeground(symbolAbove, Colors.Blue);
            var newPixelForeground = pixelForeground.Blend(pixelForegroundAbove);
            Assert.That(newPixelForeground.Color, Is.EqualTo(Colors.Blue));
            Assert.That(newPixelForeground.Symbol.Text, Is.EqualTo("b"));
        }

        [Test]
        public void BlendComplex()
        {
            var symbol = new SimpleSymbol('a');
            var pixelForeground = new PixelForeground(symbol, Colors.Red, FontWeight.Light, FontStyle.Normal, TextDecorations.Strikethrough);
            var symbolAbove = new SimpleSymbol('b');
            var pixelForegroundAbove = new PixelForeground(symbolAbove, Colors.Blue, FontWeight.Bold, FontStyle.Italic, TextDecorations.Underline);
            var newPixelForeground = pixelForeground.Blend(pixelForegroundAbove);
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
            var newPixelForeground = pixelForeground.Blend(pixelForegroundAbove);
            Assert.That(newPixelForeground.Color, Is.EqualTo(Colors.Blue));
            Assert.That(newPixelForeground.Symbol.Text, Is.EqualTo("🎶"));
        }
    }
}