using Avalonia.Media;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using NUnit.Framework;

namespace Consolonia.Core.Tests
{
    [TestFixture]
    public class PixelBackgroundTests
    {
        [Test]
        public void Constructor()
        {
            var pixelBackground = new PixelBackground(Colors.Red);
            Assert.That(pixelBackground.Color, Is.EqualTo(Colors.Red));
            Assert.That(pixelBackground.Mode, Is.EqualTo(PixelBackgroundMode.Colored));
        }

        [Test]
        [TestCase(PixelBackgroundMode.Transparent)]
        [TestCase(PixelBackgroundMode.Colored)]
        [TestCase(PixelBackgroundMode.Shaded)]
        public void ConstructorWithMode(PixelBackgroundMode mode)
        {
            var pixelBackground = new PixelBackground(mode, Colors.Red);
            Assert.That(pixelBackground.Color.Equals(Colors.Red));
            Assert.That(pixelBackground.Mode.Equals(mode));
        }

        [Test]
        public void Equality()
        {
            var pixelBackground = new PixelBackground(Colors.Red);
            var pixelBackground2 = new PixelBackground(Colors.Red);
            Assert.That(pixelBackground.Equals((object)pixelBackground2));
            Assert.That(pixelBackground.Equals(pixelBackground2));
            Assert.That(pixelBackground == pixelBackground2);

            pixelBackground = new PixelBackground(PixelBackgroundMode.Transparent, Colors.Blue);
            pixelBackground2 = new PixelBackground(PixelBackgroundMode.Transparent, Colors.Blue);
            Assert.That(pixelBackground.Equals((object)pixelBackground2));
            Assert.That(pixelBackground.Equals(pixelBackground2));
            Assert.That(pixelBackground == pixelBackground2);
        }

        [Test]
        public void Inequality()
        {
            var pixelBackground = new PixelBackground(Colors.Red);
            var pixelBackground2 = new PixelBackground(Colors.Blue);
            Assert.That(!pixelBackground.Equals(pixelBackground2));
            Assert.That(pixelBackground != pixelBackground2);

            pixelBackground = new PixelBackground(PixelBackgroundMode.Colored, Colors.Red);
            pixelBackground2 = new PixelBackground(PixelBackgroundMode.Transparent, Colors.Red);
            Assert.That(!pixelBackground.Equals((object)pixelBackground2));
            Assert.That(!pixelBackground.Equals(pixelBackground2));
            Assert.That(pixelBackground != pixelBackground2);
        }

        [Test]
        public void HashCode()
        {
            var pixelBackground = new PixelBackground(Colors.Red);
            var pixelBackground2 = new PixelBackground(Colors.Red);
            Assert.That(pixelBackground.GetHashCode(), Is.EqualTo(pixelBackground2.GetHashCode()));

            pixelBackground = new PixelBackground(PixelBackgroundMode.Transparent, Colors.Blue);
            pixelBackground2 = new PixelBackground(PixelBackgroundMode.Transparent, Colors.Blue);
            Assert.That(pixelBackground.GetHashCode(), Is.EqualTo(pixelBackground2.GetHashCode()));

            // inequal hashcode
            pixelBackground = new PixelBackground(Colors.Red);
            pixelBackground2 = new PixelBackground(Colors.Blue);
            Assert.That(pixelBackground.GetHashCode(), Is.Not.EqualTo(pixelBackground2.GetHashCode()));
        }
    }
}