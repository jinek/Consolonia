using System.Collections.Generic;
using Avalonia.Media;
using Consolonia.Controls;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Consolonia.Core.Tests.WithLifetimeFixture
{
    [TestFixture]
    public class PixelTests
    {
        [Test]
        public void ConstructorCaret()
        {
            var pixel = new Pixel(CaretStyle.BlinkingBar);
            Assert.That(pixel.IsCaret());
            Assert.That(pixel.Foreground == new PixelForeground());
            Assert.That(pixel.Background == new PixelBackground());
        }

        [Test]
        public void ConstructorColorOnly()
        {
            var pixel = new Pixel(new PixelBackground(Colors.Red));
            Assert.That(pixel.Background.Color, Is.EqualTo(Colors.Red));
        }

        [Test]
        public void ConstructorColorAndSymbol()
        {
            var pixel = new Pixel(new SimpleSymbol("a"), Colors.Red);
            Assert.That(pixel.Foreground.Symbol.Text, Is.EqualTo("a"));
            Assert.That(pixel.Foreground.Color, Is.EqualTo(Colors.Red));
            Assert.That(pixel.Foreground.Style, Is.EqualTo(FontStyle.Normal));
            Assert.That(pixel.Foreground.Weight, Is.EqualTo(FontWeight.Normal));
            Assert.That(pixel.Foreground.TextDecoration, Is.Null);
            Assert.That(pixel.Background.Color, Is.EqualTo(Colors.Transparent));
        }

        [Test]
        public void ConstructorDrawingBoxSymbol()
        {
            var pixel = new Pixel(new DrawingBoxSymbol(0b0000_1111), Colors.Red);
            Assert.That(pixel.Foreground.Symbol.Text, Is.EqualTo("┼"));
            Assert.That(pixel.Foreground.Color, Is.EqualTo(Colors.Red));
            Assert.That(pixel.Foreground.Style, Is.EqualTo(FontStyle.Normal));
            Assert.That(pixel.Foreground.Weight, Is.EqualTo(FontWeight.Normal));
            Assert.That(pixel.Foreground.TextDecoration, Is.Null);
            Assert.That(pixel.Background.Color, Is.EqualTo(Colors.Transparent));
        }

        [Test]
        public void ConstructorDrawingBoxSymbolAndColor()
        {
            var pixel = new Pixel(new PixelForeground(new DrawingBoxSymbol(0b0000_1111), Colors.Red),
                new PixelBackground(Colors.Blue));
            Assert.That(pixel.Foreground.Symbol.Text, Is.EqualTo("┼"));
            Assert.That(pixel.Foreground.Color, Is.EqualTo(Colors.Red));
            Assert.IsNull(pixel.Foreground.Style);
            Assert.IsNull(pixel.Foreground.Weight);
            Assert.IsNull(pixel.Foreground.TextDecoration);
            Assert.That(pixel.Background.Color, Is.EqualTo(Colors.Blue));
        }

        [Test]
        public void Equality()
        {
            var pixel1 = new Pixel(new PixelForeground(new SimpleSymbol('a'), Colors.Red),
                new PixelBackground(Colors.Blue));
            var pixel2 = new Pixel(new PixelForeground(new SimpleSymbol('a'), Colors.Red),
                new PixelBackground(Colors.Blue));
            Assert.That(pixel1.Equals((object)pixel2));
            Assert.That(pixel1.Equals(pixel2));
            Assert.That(pixel1 == pixel2);
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
            var pixel = new Pixel(CaretStyle.BlinkingBar);
            var pixel2 = new Pixel(CaretStyle.BlinkingBar);
            Assert.That(pixel.Equals((object)pixel2));
            Assert.That(pixel.Equals(pixel2));
            Assert.That(pixel == pixel2);
        }

        [Test]
        public void InequalityCaret()
        {
            var pixel = new Pixel(CaretStyle.BlinkingBar);
            var pixel2 = new Pixel(CaretStyle.None);
            Assert.That(!pixel.Equals((object)pixel2));
            Assert.That(!pixel.Equals(pixel2));
            Assert.That(pixel != pixel2);
        }

        [Test]
        public void PixelShade()
        {
            var pixel = new Pixel(new PixelForeground(new SimpleSymbol('a'),
                    Colors.Red,
                    FontWeight.Bold,
                    FontStyle.Italic,
                    TextDecorationLocation.Underline),
                new PixelBackground(Colors.Green));
            Pixel newPixel = pixel.Shade();
            Assert.That(newPixel.Foreground.Symbol.Text, Is.EqualTo("a"));
            Assert.That(newPixel.Foreground.Color, Is.EqualTo(Colors.Red.Shade()));
            Assert.That(newPixel.Background.Color, Is.EqualTo(Colors.Green.Shade()));
            Assert.That(newPixel.Foreground.Weight, Is.EqualTo(FontWeight.Bold));
            Assert.That(newPixel.Foreground.Style, Is.EqualTo(FontStyle.Italic));
            Assert.That(newPixel.Foreground.TextDecoration, Is.EqualTo(TextDecorationLocation.Underline));
        }

        [Test]
        public void PixelBrighten()
        {
            var pixel = new Pixel(new PixelForeground(new SimpleSymbol('a'),
                    Colors.Red,
                    FontWeight.Bold,
                    FontStyle.Italic,
                    TextDecorationLocation.Underline),
                new PixelBackground(Colors.Green));
            Pixel newPixel = pixel.Brighten();
            Assert.That(newPixel.Foreground.Symbol.Text, Is.EqualTo("a"));
            Assert.That(newPixel.Foreground.Color, Is.EqualTo(Colors.Red.Brighten()));
            Assert.That(newPixel.Background.Color, Is.EqualTo(Colors.Green.Brighten()));
            Assert.That(newPixel.Foreground.Weight, Is.EqualTo(FontWeight.Bold));
            Assert.That(newPixel.Foreground.Style, Is.EqualTo(FontStyle.Italic));
            Assert.That(newPixel.Foreground.TextDecoration, Is.EqualTo(TextDecorationLocation.Underline));
        }

        [Test]
        public void PixelInvert()
        {
            var pixel = new Pixel(new PixelForeground(new SimpleSymbol('a'),
                    Colors.Red,
                    FontWeight.Bold,
                    FontStyle.Italic,
                    TextDecorationLocation.Underline),
                new PixelBackground(Colors.Green));
            Pixel newPixel = pixel.Invert();
            Assert.That(newPixel.Foreground.Symbol.Text, Is.EqualTo("a"));
            Assert.That(newPixel.Foreground.Color, Is.EqualTo(Colors.Green));
            Assert.That(newPixel.Background.Color, Is.EqualTo(Colors.Red));
            Assert.That(newPixel.Foreground.Weight, Is.EqualTo(FontWeight.Bold));
            Assert.That(newPixel.Foreground.Style, Is.EqualTo(FontStyle.Italic));
            Assert.That(newPixel.Foreground.TextDecoration, Is.EqualTo(TextDecorationLocation.Underline));
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
        public void BlendShadedBackground()
        {
            var pixel = new Pixel(new PixelForeground(new SimpleSymbol("x"), Colors.Gray),
                new PixelBackground(Colors.White));
            var pixel2 = new Pixel(new PixelBackground(Color.Parse("#7F000000")));
            Pixel newPixel = pixel.Blend(pixel2);
            Assert.True(newPixel.Foreground.Symbol.Text == "x");
            // foreground should be darker than original
            Assert.True(newPixel.Foreground.Color.R < pixel.Foreground.Color.R &&
                        newPixel.Foreground.Color.G < pixel.Foreground.Color.G &&
                        newPixel.Foreground.Color.B < pixel.Foreground.Color.B);
            // background should be darker than original
            Assert.True(newPixel.Background.Color.R < pixel.Background.Color.R &&
                        newPixel.Background.Color.G < pixel.Background.Color.G &&
                        newPixel.Background.Color.B < pixel.Background.Color.B);
        }

        [Test]
        public void BlendShadedBackground2()
        {
            var pixel = new Pixel(new PixelForeground(new SimpleSymbol("x"), Colors.Gray),
                new PixelBackground(Colors.Black));
            var pixel2 = new Pixel(new PixelBackground(Color.Parse("#7F000000")));
            Pixel newPixel = pixel.Blend(pixel2);
            Assert.True(newPixel.Foreground.Symbol.Text == "x");
            // foreground should be darker than original
            Assert.True(newPixel.Foreground.Color.R < pixel.Foreground.Color.R &&
                        newPixel.Foreground.Color.G < pixel.Foreground.Color.G &&
                        newPixel.Foreground.Color.B < pixel.Foreground.Color.B);
            // background should be not lighter than original
            Assert.True(newPixel.Background.Color.R <= pixel.Background.Color.R &&
                        newPixel.Background.Color.G <= pixel.Background.Color.G &&
                        newPixel.Background.Color.B <= pixel.Background.Color.B);
        }

        [Test]
        public void TextBelowSemiTransparentBackgroundIsStillVisible()
        {
            var pixel = new Pixel(new PixelForeground(new SimpleSymbol("x"), Colors.Gray),
                new PixelBackground(Colors.White));
            var pixel2 = new Pixel(new PixelBackground(Color.Parse("#7F000000")));
            Pixel newPixel = pixel2.Blend(pixel);
            Assert.True(newPixel.Foreground.Symbol.Text == "x");
        }

        [Test]
        public void TextBelowNoneTransparentNullCharacterIsStillVisible()
        {
            var pixel = new Pixel(new PixelForeground(new SimpleSymbol("x"), Colors.Gray),
                new PixelBackground(Colors.White));
            var pixel2 = new Pixel(new PixelForeground(new SimpleSymbol(char.MinValue), Colors.White),
                new PixelBackground(Color.Parse("#7F000000")));
            Pixel newPixel = pixel2.Blend(pixel);
            Assert.True(newPixel.Foreground.Symbol.Text == "x");
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

        [Test]
        public void JsonSerialization()
        {
            var pixel = new Pixel(new PixelForeground(new SimpleSymbol('a'), Colors.Red),
                new PixelBackground(Colors.Blue));
            string json = JsonConvert.SerializeObject(pixel);
            var pixel2 = JsonConvert.DeserializeObject<Pixel>(json);
            Assert.That(pixel.Equals(pixel2));
        }
    }
}