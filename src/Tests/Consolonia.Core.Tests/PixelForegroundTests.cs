using System.Linq;
using System.Text;
using Avalonia.Media;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Consolonia.Core.Tests
{
    [TestFixture]
    public class PixelForegroundTests
    {
        [Test]
        public void Constructor()
        {
            var pixelForeground = new PixelForeground();
            Assert.That(pixelForeground.Color, Is.EqualTo(Colors.Transparent));
            Assert.That(pixelForeground.Symbol.Text, Is.EqualTo(" "));
            Assert.That(pixelForeground.Symbol.Width, Is.EqualTo(1));
            Assert.That(pixelForeground.Weight, Is.EqualTo(FontWeight.Normal));
            Assert.That(pixelForeground.Style, Is.EqualTo(FontStyle.Normal));
            Assert.That(pixelForeground.TextDecoration, Is.Null);
        }

        [Test]
        public void ConstructorWithSymbol()
        {
            var symbol = new SimpleSymbol('a');
            var pixelForeground = new PixelForeground(symbol, Colors.Red);
            Assert.That(pixelForeground.Color, Is.EqualTo(Colors.Red));
            Assert.That(pixelForeground.Symbol.Text, Is.EqualTo("a"));
            Assert.That(pixelForeground.Weight, Is.EqualTo(FontWeight.Normal));
            Assert.That(pixelForeground.Style, Is.EqualTo(FontStyle.Normal));
            Assert.That(pixelForeground.TextDecoration, Is.Null);
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
            Assert.That(pixelForeground.TextDecoration, Is.Null);
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
            Assert.That(pixelForeground.TextDecoration, Is.Null);
        }

        [Test]
        public void ConstructorWithSymbolAndTextDecorations()
        {
            var symbol = new SimpleSymbol('a');
            TextDecorationLocation? textDecoration = TextDecorationLocation.Underline;
            var pixelForeground = new PixelForeground(symbol, Colors.Red, textDecoration: textDecoration);
            Assert.That(pixelForeground.Color, Is.EqualTo(Colors.Red));
            Assert.That(pixelForeground.Symbol.Text, Is.EqualTo("a"));
            Assert.That(pixelForeground.Weight, Is.EqualTo(FontWeight.Normal));
            Assert.That(pixelForeground.Style, Is.EqualTo(FontStyle.Normal));
            Assert.That(pixelForeground.TextDecoration, Is.EqualTo(TextDecorationLocation.Underline));
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
            Assert.That(pixelForeground.TextDecoration, Is.Null);
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
            foreach (PixelForeground variation in new PixelForeground[]
                     {
                         new(new SimpleSymbol('b'), Colors.Red),
                         new(new SimpleSymbol('a'), Colors.Blue),
                         new(new SimpleSymbol('a'), Colors.Red, FontWeight.Bold),
                         new(new SimpleSymbol('a'), Colors.Red, style: FontStyle.Italic),
                         new(new SimpleSymbol('a'), Colors.Red, textDecoration: TextDecorationLocation.Underline)
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
                TextDecorationLocation.Underline);
            PixelForeground newPixelForeground = pixelForeground.Blend(pixelForegroundAbove);
            Assert.That(newPixelForeground.Color, Is.EqualTo(Colors.Blue));
            Assert.That(newPixelForeground.Symbol.Text, Is.EqualTo("b"));
            Assert.That(newPixelForeground.Weight, Is.EqualTo(FontWeight.Bold));
            Assert.That(newPixelForeground.Style, Is.EqualTo(FontStyle.Italic));
            Assert.That(newPixelForeground.TextDecoration, Is.EqualTo(TextDecorationLocation.Underline));
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

        [Test]
        public void JsonSerialization()
        {
            var pixelForeground = new PixelForeground(new SimpleSymbol('a'), Colors.Red);
            string json = JsonConvert.SerializeObject(pixelForeground);
            var pixelForeground2 = JsonConvert.DeserializeObject<PixelForeground>(json);
            Assert.That(pixelForeground.Equals(pixelForeground2));
        }


        [Test]
        public void JsonSerialization2()
        {
            var pixelForeground = new PixelForeground(new SimpleSymbol('a'), Colors.Red, FontWeight.Bold,
                FontStyle.Italic, TextDecorationLocation.Underline);
            string json = JsonConvert.SerializeObject(pixelForeground);
            var pixelForeground2 = JsonConvert.DeserializeObject<PixelForeground>(json);
            Assert.That(pixelForeground.Equals(pixelForeground2));
        }

        [Test]
        public void JsonSerializationDefault()
        {
            var pixelForeground = new PixelForeground();
            string json = JsonConvert.SerializeObject(pixelForeground);
            var pixelForeground2 = JsonConvert.DeserializeObject<PixelForeground>(json);
            Assert.That(pixelForeground.Equals(pixelForeground2));
        }
    }
}