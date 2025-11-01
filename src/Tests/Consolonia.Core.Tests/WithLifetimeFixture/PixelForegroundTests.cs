// DUPFINDER_ignore

using System.Linq;
using System.Text;
using Avalonia.Media;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Consolonia.Core.Tests.WithLifetimeFixture
{
    [TestFixture]
    public class PixelForegroundTests
    {
        [Test]
        public void Constructor()
        {
            var pixelForeground = PixelForeground.Default;
            Assert.That(pixelForeground.Color, Is.EqualTo(Colors.Transparent));
            Assert.That(pixelForeground.Symbol.Character, Is.EqualTo(' '));
            Assert.That(pixelForeground.Symbol.Width, Is.EqualTo(1));
            Assert.IsNull(pixelForeground.Weight);
            Assert.IsNull(pixelForeground.Style);
            Assert.IsNull(pixelForeground.TextDecoration);
        }

        [Test]
        public void ConstructorWithSymbol()
        {
            var symbol = new Symbol('a');
            var pixelForeground = new PixelForeground(symbol, Colors.Red);
            Assert.That(pixelForeground.Color, Is.EqualTo(Colors.Red));
            Assert.That(pixelForeground.Symbol.Character, Is.EqualTo('a'));
            Assert.IsNull(pixelForeground.Weight);
            Assert.IsNull(pixelForeground.Style);
            Assert.IsNull(pixelForeground.TextDecoration);
        }

        [Test]
        public void ConstructorWithSymbolAndWeight()
        {
            var symbol = new Symbol('a');
            var pixelForeground = new PixelForeground(symbol, Colors.Red, FontWeight.Bold);
            Assert.That(pixelForeground.Color, Is.EqualTo(Colors.Red));
            Assert.That(pixelForeground.Symbol.Character, Is.EqualTo('a'));
            Assert.That(pixelForeground.Weight, Is.EqualTo(FontWeight.Bold));
            Assert.IsNull(pixelForeground.Style);
            Assert.IsNull(pixelForeground.TextDecoration);
        }

        [Test]
        public void ConstructorWithSymbolAndStyle()
        {
            var symbol = new Symbol('a');
            var pixelForeground = new PixelForeground(symbol, Colors.Red, style: FontStyle.Italic);
            Assert.That(pixelForeground.Color, Is.EqualTo(Colors.Red));
            Assert.That(pixelForeground.Symbol.Character, Is.EqualTo('a'));
            Assert.IsNull(pixelForeground.Weight);
            Assert.That(pixelForeground.Style, Is.EqualTo(FontStyle.Italic));
            Assert.IsNull(pixelForeground.TextDecoration);
        }

        [Test]
        public void ConstructorWithSymbolAndTextDecorations()
        {
            var symbol = new Symbol('a');
            TextDecorationLocation? textDecoration = TextDecorationLocation.Underline;
            var pixelForeground = new PixelForeground(symbol, Colors.Red, textDecoration: textDecoration);
            Assert.That(pixelForeground.Color, Is.EqualTo(Colors.Red));
            Assert.That(pixelForeground.Symbol.Character, Is.EqualTo('a'));
            Assert.IsNull(pixelForeground.Weight);
            Assert.IsNull(pixelForeground.Style);
            Assert.That(pixelForeground.TextDecoration, Is.EqualTo(TextDecorationLocation.Underline));
        }

        [Test]
        public void ConstructorWithWideCharacter()
        {
            Rune rune = "🎵".EnumerateRunes().First();
            var symbol = new Symbol(rune.ToString());
            var pixelForeground = new PixelForeground(symbol, Colors.Red);
            Assert.That(pixelForeground.Color, Is.EqualTo(Colors.Red));
            Assert.That(pixelForeground.Symbol.Character, Is.EqualTo(char.MinValue));
            Assert.That(pixelForeground.Symbol.Complex, Is.EqualTo("🎵️"));
            Assert.IsNull(pixelForeground.Weight);
            Assert.IsNull(pixelForeground.Style);
            Assert.IsNull(pixelForeground.TextDecoration);
        }

        [Test]
        public void Equality()
        {
            var pixelForeground = new PixelForeground(new Symbol('a'), Colors.Red);
            var pixelForeground2 = new PixelForeground(new Symbol('a'), Colors.Red);
            Assert.That(pixelForeground.Equals((object)pixelForeground2));
            Assert.That(pixelForeground.Equals(pixelForeground2));
            Assert.That(pixelForeground == pixelForeground2, Is.True);
        }

        [Test]
        public void Inequality()
        {
            var pixelForeground = new PixelForeground(new Symbol('a'), Colors.Red);
            foreach (PixelForeground variation in new PixelForeground[]
                     {
                         new(new Symbol('b'), Colors.Red),
                         new(new Symbol('a'), Colors.Blue),
                         new(new Symbol('a'), Colors.Red, FontWeight.Bold),
                         new(new Symbol('a'), Colors.Red, style: FontStyle.Italic),
                         new(new Symbol('a'), Colors.Red, textDecoration: TextDecorationLocation.Underline)
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
            var symbol = new Symbol('a');
            var pixelForeground = new PixelForeground(symbol, Colors.Red);
            var symbolAbove = new Symbol('b');
            var pixelForegroundAbove = new PixelForeground(symbolAbove, Colors.Blue);
            PixelForeground newPixelForeground = pixelForeground.Blend(pixelForegroundAbove);
            Assert.That(newPixelForeground.Color, Is.EqualTo(Colors.Blue));
            Assert.That(newPixelForeground.Symbol.Character, Is.EqualTo('b'));
        }

        [Test]
        public void BlendComplex()
        {
            var symbol = new Symbol('a');
            var pixelForeground = new PixelForeground(symbol, Colors.Red);
            var symbolAbove = new Symbol('b');
            var pixelForegroundAbove = new PixelForeground(symbolAbove, Colors.Blue, FontWeight.Bold, FontStyle.Italic,
                TextDecorationLocation.Underline);
            PixelForeground newPixelForeground = pixelForeground.Blend(pixelForegroundAbove);
            Assert.That(newPixelForeground.Color, Is.EqualTo(Colors.Blue));
            Assert.That(newPixelForeground.Symbol.Character, Is.EqualTo('b'));
            Assert.That(newPixelForeground.Weight, Is.EqualTo(FontWeight.Bold));
            Assert.That(newPixelForeground.Style, Is.EqualTo(FontStyle.Italic));
            Assert.That(newPixelForeground.TextDecoration, Is.EqualTo(TextDecorationLocation.Underline));
        }

        [Test]
        public void BlendEmoji()
        {
            var symbol = new Symbol("🎵");
            var pixelForeground = new PixelForeground(symbol, Colors.Red);
            var symbolAbove = new Symbol("🎶");
            var pixelForegroundAbove = new PixelForeground(symbolAbove, Colors.Blue);
            PixelForeground newPixelForeground = pixelForeground.Blend(pixelForegroundAbove);
            Assert.That(newPixelForeground.Color, Is.EqualTo(Colors.Blue));
            Assert.That(newPixelForeground.Symbol.Complex, Is.EqualTo("🎶️"));
        }

        [Test]
        public void HashCode()
        {
            var pixelForeground = new PixelForeground(new Symbol('a'), Colors.Red);
            var pixelForeground2 = new PixelForeground(new Symbol('a'), Colors.Red);
            Assert.That(pixelForeground.GetHashCode(), Is.EqualTo(pixelForeground2.GetHashCode()));

            // inequal test
            pixelForeground = new PixelForeground(new Symbol('a'), Colors.Red);
            pixelForeground2 = new PixelForeground(new Symbol('b'), Colors.Red);
            Assert.That(pixelForeground.GetHashCode(), Is.Not.EqualTo(pixelForeground2.GetHashCode()));

            pixelForeground = new PixelForeground(new Symbol('a'), Colors.Red);
            pixelForeground2 = new PixelForeground(new Symbol('a'), Colors.Blue);
            Assert.That(pixelForeground.GetHashCode(), Is.Not.EqualTo(pixelForeground2.GetHashCode()));
        }

        [Test]
        public void JsonSerialization()
        {
            var pixelForeground = new PixelForeground(new Symbol('a'), Colors.Red);
            string json = JsonConvert.SerializeObject(pixelForeground);
            var pixelForeground2 = JsonConvert.DeserializeObject<PixelForeground>(json);
            Assert.That(pixelForeground.Equals(pixelForeground2));
        }


        [Test]
        public void JsonSerialization2()
        {
            var pixelForeground = new PixelForeground(new Symbol('a'), Colors.Red, FontWeight.Bold,
                FontStyle.Italic, TextDecorationLocation.Underline);
            string json = JsonConvert.SerializeObject(pixelForeground);
            var pixelForeground2 = JsonConvert.DeserializeObject<PixelForeground>(json);
            Assert.That(pixelForeground.Equals(pixelForeground2));
        }

        [Test]
        public void JsonSerializationDefault()
        {
            var pixelForeground = PixelForeground.Default;
            string json = JsonConvert.SerializeObject(pixelForeground);
            var pixelForeground2 = JsonConvert.DeserializeObject<PixelForeground>(json);
            Assert.That(pixelForeground.Equals(pixelForeground2));
        }
    }
}