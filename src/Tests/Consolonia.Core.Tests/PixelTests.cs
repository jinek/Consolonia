using Avalonia.Media;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using NUnit.Framework;

namespace Consolonia.Core.Tests
{
    [TestFixture]
    public class PixelTests
    {
        [Test]
        public void PixelConstructorColorOnly()
        {
            Pixel pixel = new Pixel(Colors.Red);
            Assert.That(pixel.Background.Color, Is.EqualTo(Colors.Red));
            Assert.That(pixel.Background.Mode, Is.EqualTo(PixelBackgroundMode.Colored));
        }

        [Test]
        public void PixelConstructorColorAndSymbol()
        {
            Pixel pixel = new Pixel(new SimpleSymbol('a'), Colors.Red);
            Assert.That(pixel.Foreground.Symbol.GetCharacter(), Is.EqualTo('a'));
            Assert.That(pixel.Foreground.Color, Is.EqualTo(Colors.Red));
            Assert.That(pixel.Foreground.Style, Is.EqualTo(FontStyle.Normal));
            Assert.That(pixel.Foreground.Weight, Is.EqualTo(FontWeight.Normal));
            Assert.That(pixel.Foreground.TextDecorations, Is.Null);
            Assert.That(pixel.Background.Color, Is.EqualTo(Colors.Transparent));
            Assert.That(pixel.Background.Mode, Is.EqualTo(PixelBackgroundMode.Transparent));
        }

        [Test]
        public void PixelConstructorDrawingBoxSymbol()
        {
            Pixel pixel = new Pixel(0b0000_1111, Colors.Red);
            Assert.That(pixel.Foreground.Symbol.GetCharacter(), Is.EqualTo('┼'));
            Assert.That(pixel.Foreground.Color, Is.EqualTo(Colors.Red));
            Assert.That(pixel.Foreground.Style, Is.EqualTo(FontStyle.Normal));
            Assert.That(pixel.Foreground.Weight, Is.EqualTo(FontWeight.Normal));
            Assert.That(pixel.Foreground.TextDecorations, Is.Null);
            Assert.That(pixel.Background.Color, Is.EqualTo(Colors.Transparent));
            Assert.That(pixel.Background.Mode, Is.EqualTo(PixelBackgroundMode.Transparent));
        }

        [Test]
        public void PixelConstructorDrawingBoxSymbolAndColor()
        {
            Pixel pixel = new Pixel(new PixelForeground(new DrawingBoxSymbol(0b0000_1111), color: Colors.Red), new PixelBackground(Colors.Blue));
            Assert.That(pixel.Foreground.Symbol.GetCharacter(), Is.EqualTo('┼'));
            Assert.That(pixel.Foreground.Color, Is.EqualTo(Colors.Red));
            Assert.That(pixel.Foreground.Style, Is.EqualTo(FontStyle.Normal));
            Assert.That(pixel.Foreground.Weight, Is.EqualTo(FontWeight.Normal));
            Assert.That(pixel.Foreground.TextDecorations, Is.Null);
            Assert.That(pixel.Background.Color, Is.EqualTo(Colors.Blue));
            Assert.That(pixel.Background.Mode, Is.EqualTo(PixelBackgroundMode.Colored));
        }
    }
}