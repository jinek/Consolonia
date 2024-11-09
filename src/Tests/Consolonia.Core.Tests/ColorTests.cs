using Avalonia.Media;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using NUnit.Framework;

namespace Consolonia.Core.Tests
{
    [TestFixture]
    public class ColorTests
    {
        [Test]
        public void TestShade()
        {
            Color foreground = Colors.Gray;
            Color newColor = foreground.Shade();
            Assert.That(newColor.R < foreground.R);
            Assert.That(newColor.G < foreground.G);
            Assert.That(newColor.B < foreground.B);
        }

        [Test]
        public void TestBrighten()
        {
            Color foreground = Colors.Gray;
            Color newColor = foreground.Brighten();
            Assert.That(newColor.R > foreground.R);
            Assert.That(newColor.G > foreground.G);
            Assert.That(newColor.B > foreground.B);
        }

        [Test]
        public void TestRelativeShade()
        {
            Color foreground = Colors.LightGray;
            Color background = Colors.DarkGray;
            Color newColor = foreground.Shade(background);
            Assert.That(newColor.R < foreground.R);
            Assert.That(newColor.G < foreground.G);
            Assert.That(newColor.B < foreground.B);

            Color foreground2 = Colors.DarkGray;
            Color background2 = Colors.LightGray;
            newColor = foreground2.Shade(background2);
            Assert.That(newColor.R > foreground2.R);
            Assert.That(newColor.G > foreground2.G);
            Assert.That(newColor.B > foreground2.B);
        }

        [Test]
        public void TestRelativeBrighten()
        {
            Color foreground = Colors.LightGray;
            Color background = Colors.DarkGray;
            Color newColor = foreground.Brighten(background);
            Assert.That(newColor.R > foreground.R);
            Assert.That(newColor.G > foreground.G);
            Assert.That(newColor.B > foreground.B);

            Color foreground2 = Colors.DarkGray;
            Color background2 = Colors.LightGray;
            newColor = foreground2.Brighten(background2);
            Assert.That(newColor.R < foreground2.R);
            Assert.That(newColor.G < foreground2.G);
            Assert.That(newColor.B < foreground2.B);
        }

        [Test]
        public void TestShadeBoundaries()
        {
            Color foreground = Colors.White;
            Color background = Colors.Black;
            Color newColor = foreground;
            for (int i = 0; i < byte.MaxValue; i++)
            {
                newColor = newColor.Shade(background);
                Assert.That(newColor.R < foreground.R);
                Assert.That(newColor.G < foreground.G);
                Assert.That(newColor.B < foreground.B);
                if (newColor == Colors.Black)
                    break;
            }

            Assert.That(newColor == Colors.Black);

            Color foreground2 = Colors.Black;
            Color background2 = Colors.White;
            newColor = foreground2;
            for (int i = 0; i < byte.MaxValue; i++)
            {
                newColor = newColor.Shade(background2);
                Assert.That(newColor.R > foreground2.R);
                Assert.That(newColor.G > foreground2.G);
                Assert.That(newColor.B > foreground2.B);
                if (newColor == Colors.White)
                    break;
            }

            Assert.That(newColor == Colors.White);
        }

        [Test]
        public void TestBrightenBoundaries()
        {
            Color foreground = Colors.Black;
            Color background = Colors.Black;
            Color newColor = foreground;
            for (int i = 0; i < byte.MaxValue; i++)
            {
                newColor = newColor.Brighten(background);
                Assert.That(newColor.R > foreground.R);
                Assert.That(newColor.G > foreground.G);
                Assert.That(newColor.B > foreground.B);
                if (newColor == Colors.White)
                    break;
            }

            Assert.That(newColor == Colors.White);

            Color foreground2 = Colors.White;
            Color background2 = Colors.White;
            newColor = foreground2;
            for (int i = 0; i < byte.MaxValue; i++)
            {
                newColor = newColor.Brighten(background2);
                Assert.That(newColor.R < foreground2.R);
                Assert.That(newColor.G < foreground2.G);
                Assert.That(newColor.B < foreground2.B);
                if (newColor == Colors.Black)
                    break;
            }

            Assert.That(newColor == Colors.Black);
        }
    }
}