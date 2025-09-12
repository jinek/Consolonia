using Avalonia.Media;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Consolonia.Core.Tests.WithLifetimeFixture
{
    [TestFixture]
    public class PixelBackgroundTests
    {
        [Test]
        public void Constructor()
        {
            PixelBackground pixelBackground = PixelBackground.Transparent;
            Assert.That(pixelBackground.Color, Is.EqualTo(Colors.Transparent));
        }

        [Test]
        public void ConstructorWithColor()
        {
            var pixelBackground = new PixelBackground(Colors.Red);
            Assert.That(pixelBackground.Color, Is.EqualTo(Colors.Red));
        }

        [Test]
        public void Equality()
        {
            var pixelBackground = new PixelBackground(Colors.Red);
            var pixelBackground2 = new PixelBackground(Colors.Red);
            Assert.That(pixelBackground.Equals((object)pixelBackground2));
            Assert.That(pixelBackground.Equals(pixelBackground2));
            Assert.That(pixelBackground == pixelBackground2);

            pixelBackground = new PixelBackground(Color.Parse("#000000FF"));
            pixelBackground2 = new PixelBackground(Color.Parse("#000000FF"));
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

            pixelBackground = new PixelBackground(Colors.Red);
            pixelBackground2 = new PixelBackground(Color.Parse("#7FFF0000"));
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

            pixelBackground = new PixelBackground(Color.Parse("#000000FF"));
            pixelBackground2 = new PixelBackground(Color.Parse("#000000FF"));
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