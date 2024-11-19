using Avalonia.Media;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Consolonia.Core.Tests
{
    [TestFixture]
    public class PixelBackgroundTests
    {
        [Test]
        public void Constructor()
        {
            var pixelBackground = new PixelBackground();
            Assert.That(pixelBackground.Color, Is.EqualTo(Colors.Transparent));
            Assert.That(pixelBackground.Mode, Is.EqualTo(PixelBackgroundMode.Transparent));
        }

        [Test]
        public void ConstructorWithColor()
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

#pragma warning disable CA1508 // Avoid dead conditional code
            pixelBackground = null;
            Assert.IsFalse(pixelBackground == pixelBackground2);
            pixelBackground2 = null;
            Assert.IsTrue(pixelBackground == pixelBackground2);
#pragma warning restore CA1508 // Avoid dead conditional code
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
#pragma warning disable CA1508 // Avoid dead conditional code
            pixelBackground = null;
            Assert.IsTrue(pixelBackground != pixelBackground2);
            pixelBackground2 = null;
            Assert.IsFalse(pixelBackground != pixelBackground2);
#pragma warning restore CA1508 // Avoid dead conditional code
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

        [Test]
        public void JsonSerialization()
        {
            var pixelBackground = new PixelBackground(Colors.Red);
            string json = JsonConvert.SerializeObject(pixelBackground);
            var pixelBackground2 = JsonConvert.DeserializeObject<PixelBackground>(json);
            Assert.That(pixelBackground.Equals(pixelBackground2));
        }
    }
}