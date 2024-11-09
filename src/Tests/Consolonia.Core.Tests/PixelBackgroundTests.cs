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
        public void ConstructorWithMode()
        {
            var pixelBackground = new PixelBackground(PixelBackgroundMode.Shaded, Colors.Red);
            Assert.That(pixelBackground.Color, Is.EqualTo(Colors.Red));
            Assert.That(pixelBackground.Mode, Is.EqualTo(PixelBackgroundMode.Shaded));
        }
    }
}