using System.Collections.Generic;
using Avalonia.Media;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using NUnit.Framework;

namespace Consolonia.Core.Tests
{
    [TestFixture]
    public class PixelTests
    {
        [Test]
        public void ConstructorCaret()
        {
            var pixel = new Pixel(true);
            Assert.That(pixel.IsCaret);
            Assert.That(pixel.Foreground == new PixelForeground());
            Assert.That(pixel.Background == new PixelBackground());
        }

        [Test]
        public void ConstructorColorOnly()
        {
            var pixel = new Pixel(new PixelBackground(Colors.Red));
            Assert.That(pixel.Background.Color, Is.EqualTo(Colors.Red));
            Assert.That(pixel.Background.Mode, Is.EqualTo(PixelBackgroundMode.Colored));
        }

        [Test]
        public void ConstructorColorAndSymbol()
        {
            var pixel = new Pixel(new SimpleSymbol("a"), Colors.Red);
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
            var pixel = new Pixel(new DrawingBoxSymbol(0b0000_1111), Colors.Red);
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
            var pixel = new Pixel(new PixelForeground(new DrawingBoxSymbol(0b0000_1111), Colors.Red),
                new PixelBackground(Colors.Blue));
            Assert.That(pixel.Foreground.Symbol.Text, Is.EqualTo("┼"));
            Assert.That(pixel.Foreground.Color, Is.EqualTo(Colors.Red));
            Assert.That(pixel.Foreground.Style, Is.EqualTo(FontStyle.Normal));
            Assert.That(pixel.Foreground.Weight, Is.EqualTo(FontWeight.Normal));
            Assert.That(pixel.Foreground.TextDecorations, Is.Null);
            Assert.That(pixel.Background.Color, Is.EqualTo(Colors.Blue));
            Assert.That(pixel.Background.Mode, Is.EqualTo(PixelBackgroundMode.Colored));
        }

        [Test]
        public void Equality()
        {
            var pixel = new Pixel(new PixelForeground(new SimpleSymbol('a'), Colors.Red),
                new PixelBackground(Colors.Blue));
            var pixel2 = new Pixel(new PixelForeground(new SimpleSymbol('a'), Colors.Red),
                new PixelBackground(Colors.Blue));
            Assert.That(pixel.Equals((object)pixel2));
            Assert.That(pixel.Equals(pixel2));
            Assert.That(pixel == pixel2);
        }

        [Test]
        public void NotEqual()
        {
            var pixel = new Pixel(new PixelForeground(new SimpleSymbol('a'), Colors.Red),
                new PixelBackground(Colors.Blue));
            var pixel2 = new Pixel(new PixelForeground(new SimpleSymbol('b'), Colors.Red),
                new PixelBackground(Colors.Blue));
            Assert.That(!pixel.Equals((object)pixel2));
            Assert.That(!pixel.Equals(pixel2));
            Assert.That(pixel != pixel2);

            pixel = new Pixel(new PixelForeground(new SimpleSymbol('a'), Colors.Red),
                new PixelBackground(Colors.Blue));
            pixel2 = new Pixel(new PixelForeground(new SimpleSymbol('a'), Colors.Blue),
                new PixelBackground(Colors.Blue));
            Assert.That(!pixel.Equals((object)pixel2));
            Assert.That(!pixel.Equals(pixel2));
            Assert.That(pixel != pixel2);
        }

        [Test]
        public void EqualityCaret()
        {
            var pixel = new Pixel(true);
            var pixel2 = new Pixel(true);
            Assert.That(pixel.Equals((object)pixel2));
            Assert.That(pixel.Equals(pixel2));
            Assert.That(pixel == pixel2);
        }

        [Test]
        public void InequalityCaret()
        {
            var pixel = new Pixel(true);
            var pixel2 = new Pixel(false);
            Assert.That(!pixel.Equals((object)pixel2));
            Assert.That(!pixel.Equals(pixel2));
            Assert.That(pixel != pixel2);
        }

        [Test]
        public void BlendTransparentBackground()
        {
            var pixel = new Pixel(new PixelBackground(Colors.Green));
            var pixel2 = new Pixel(new PixelForeground(new SimpleSymbol('a'), Colors.Red),
                new PixelBackground(Colors.Transparent));
            Pixel newPixel = pixel.Blend(pixel2);
            Assert.That(newPixel.Foreground.Symbol.Text, Is.EqualTo("a"));
            Assert.That(newPixel.Foreground.Color, Is.EqualTo(Colors.Red));
            Assert.That(newPixel.Background.Color, Is.EqualTo(Colors.Green));
        }

        [Test]
        public void BlendColoredBackground()
        {
            var pixel = new Pixel(new PixelBackground(Colors.Green));
            var pixel2 = new Pixel(new PixelForeground(new SimpleSymbol('a'), Colors.Red),
                new PixelBackground(Colors.Blue));
            Pixel newPixel = pixel.Blend(pixel2);
            Assert.That(newPixel.Foreground.Symbol.Text, Is.EqualTo("a"));
            Assert.That(newPixel.Foreground.Color, Is.EqualTo(Colors.Red));
            Assert.That(newPixel.Background.Color, Is.EqualTo(Colors.Blue));
        }

        [Test]
        public void HashCode()
        {
            var set = new HashSet<Pixel>();
            set.Add(new Pixel(new PixelForeground(new SimpleSymbol('a'), Colors.Red),
                new PixelBackground(Colors.Blue)));
            set.Add(new Pixel(new PixelForeground(new SimpleSymbol('a'), Colors.Red),
                new PixelBackground(Colors.Blue)));
            Assert.That(set.Count, Is.EqualTo(1));
        }
    }
}