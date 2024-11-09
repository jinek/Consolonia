using Avalonia.Media;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using NUnit.Framework;
using NUnit.Framework.Internal;
using static Unix.Terminal.Delegates;

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
        
        [TestCase(PixelBackgroundMode.Transparent)]
        [TestCase(PixelBackgroundMode.Colored)]
        [TestCase(PixelBackgroundMode.Shaded)]
        public void ConstructorWithMode(PixelBackgroundMode mode)
        {
            var pixelBackground = new PixelBackground(mode, Colors.Red);
            Assert.That(pixelBackground.Color, Is.EqualTo(Colors.Red));
            Assert.That(pixelBackground.Mode, Is.EqualTo(mode));
        }
    }
}