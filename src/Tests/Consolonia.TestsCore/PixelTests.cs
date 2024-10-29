using Avalonia.Media;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using NUnit.Framework;

namespace Consolonia.TestsCore
{
    [TestFixture]
    public class PixelTests 
    {

        [Test]
        public void TestShade()
        {
            Color foreground = Colors.Blue;
            var newColor = foreground.Shade();
            Assert.That(newColor.R == foreground.R);
            Assert.That(newColor.G == foreground.G);
            Assert.That(newColor.B < foreground.B);
        }

        [Test]
        public void TestBrighten()
        {
            Color foreground = Colors.Blue;
            var newColor = foreground.Brighten();
            Assert.That(newColor.R > foreground.R);
            Assert.That(newColor.G > foreground.G);
            Assert.That(newColor.B == foreground.B);
        }

        [Test]
        public void TestRelativeShade()
        {
            Color foreground = Colors.Blue;
            Color background = Colors.Black;
            var newColor = foreground.Shade(background);
            Assert.That(newColor.R == foreground.R);
            Assert.That(newColor.G == foreground.G);
            Assert.That(newColor.B < foreground.B);

            Color foreground2 = Colors.DarkGray;
            Color background2 = Colors.White;
            newColor = foreground2.Shade(background2);
            Assert.That(newColor.R > foreground2.R);
            Assert.That(newColor.G > foreground2.G);
            Assert.That(newColor.B > foreground2.B);
        }

        [Test]
        public void TestRelativeBrighten()
        {
            Color foreground = Colors.Blue;
            Color background = Colors.Black;
            var newColor = foreground.Brighten(background);
            Assert.That(newColor.R > foreground.R);
            Assert.That(newColor.G > foreground.G);
            Assert.That(newColor.B == foreground.B);

            Color foreground2 = Colors.Blue;
            Color background2 = Colors.White;
            newColor = foreground2.Brighten(background2);
            Assert.That(newColor.R == foreground2.R);
            Assert.That(newColor.G == foreground2.G);
            Assert.That(newColor.B < foreground2.B);
        }


    }
}