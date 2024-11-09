using Avalonia.Media;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Consolonia.NUnit;
using NUnit.Framework;

namespace Consolonia.Core.Tests
{
    [TestFixture]
    public class PixelTests
    {

        [Test]
        public void ConstructorColorOnly()
        {
            Pixel pixel = new Pixel(new PixelBackground(Colors.Red));
            Assert.That(pixel.Background.Color, Is.EqualTo(Colors.Red));
            Assert.That(pixel.Background.Mode, Is.EqualTo(PixelBackgroundMode.Colored));
        }

        [Test]
        public void ConstructorColorAndSymbol()
        {
            Pixel pixel = new Pixel(new SimpleSymbol("a"), Colors.Red);
            Assert.That(pixel.Foreground.Symbol.Text, Is.EqualTo("a"));
            Assert.That(pixel.Foreground.Color, Is.EqualTo(Colors.Red));
            Assert.That(pixel.Foreground.Style, Is.EqualTo(FontStyle.Normal));
            Assert.That(pixel.Foreground.Weight, Is.EqualTo(FontWeight.Normal));
            Assert.That(pixel.Foreground.TextDecorations, Is.Null);
            Assert.That(pixel.Background.Color, Is.EqualTo(Colors.Transparent));
            Assert.That(pixel.Background.Mode, Is.EqualTo(PixelBackgroundMode.Transparent));
        }

        [Test]
        public void ConstructorDrawingBoxSymbol()
        {
            Pixel pixel = new Pixel(new DrawingBoxSymbol(0b0000_1111), Colors.Red);
            Assert.That(pixel.Foreground.Symbol.Text, Is.EqualTo("┼"));
            Assert.That(pixel.Foreground.Color, Is.EqualTo(Colors.Red));
            Assert.That(pixel.Foreground.Style, Is.EqualTo(FontStyle.Normal));
            Assert.That(pixel.Foreground.Weight, Is.EqualTo(FontWeight.Normal));
            Assert.That(pixel.Foreground.TextDecorations, Is.Null);
            Assert.That(pixel.Background.Color, Is.EqualTo(Colors.Transparent));
            Assert.That(pixel.Background.Mode, Is.EqualTo(PixelBackgroundMode.Transparent));
        }

        [Test]
        public void ConstructorDrawingBoxSymbolAndColor()
        {
            Pixel pixel = new Pixel(new PixelForeground(new DrawingBoxSymbol(0b0000_1111), color: Colors.Red), new PixelBackground(Colors.Blue));
            Assert.That(pixel.Foreground.Symbol.Text, Is.EqualTo("┼"));
            Assert.That(pixel.Foreground.Color, Is.EqualTo(Colors.Red));
            Assert.That(pixel.Foreground.Style, Is.EqualTo(FontStyle.Normal));
            Assert.That(pixel.Foreground.Weight, Is.EqualTo(FontWeight.Normal));
            Assert.That(pixel.Foreground.TextDecorations, Is.Null);
            Assert.That(pixel.Background.Color, Is.EqualTo(Colors.Blue));
            Assert.That(pixel.Background.Mode, Is.EqualTo(PixelBackgroundMode.Colored));
        }

        [Test]
        public void BlendTransparentBackground()
        {
            Pixel pixel = new Pixel(new PixelBackground(Colors.Green));
            Pixel pixel2 = new Pixel(new PixelForeground(new SimpleSymbol('a'), Colors.Red), new PixelBackground(Colors.Transparent));
            Pixel newPixel = pixel.Blend(pixel2);
            Assert.That(newPixel.Foreground.Symbol.Text, Is.EqualTo("a"));
            Assert.That(newPixel.Foreground.Color, Is.EqualTo(Colors.Red));
            Assert.That(newPixel.Background.Color, Is.EqualTo(Colors.Green));
        }

        [Test]
        public void BlendColoredBackground()
        {
            Pixel pixel = new Pixel(new PixelBackground(Colors.Green));
            Pixel pixel2 = new Pixel(new PixelForeground(new SimpleSymbol('a'), Colors.Red), new PixelBackground(Colors.Blue));
            Pixel newPixel = pixel.Blend(pixel2);
            Assert.That(newPixel.Foreground.Symbol.Text, Is.EqualTo("a"));
            Assert.That(newPixel.Foreground.Color, Is.EqualTo(Colors.Red));
            Assert.That(newPixel.Background.Color, Is.EqualTo(Colors.Blue));
        }
    }
}