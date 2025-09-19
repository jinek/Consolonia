using Avalonia.Media;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Consolonia.Core.Tests.WithLifetimeFixture
{
    [TestFixture]
    public class PixelBufferTests
    {
        private static PixelBuffer CreateBuffer()
        {
            var buffer = new PixelBuffer(4, 5);
            for (ushort y = 0; y < buffer.Height; y++)
            for (ushort x = 0; x < buffer.Width; x++)
                if (x % 3 == 0)
                    buffer.Pixels[x, y] = new Pixel(new Symbol($"{x},{y}"), Colors.Blue);
                else if (x % 3 == 1)
                    buffer.Pixels[x, y] = Pixel.Empty;
                else
                    buffer.Pixels[x, y] = new Pixel(new Symbol($"{x},{y}"), Colors.White, FontStyle.Italic,
                        FontWeight.Bold, TextDecorationLocation.Underline);

            return buffer;
        }

        private static void AssertBuffer(PixelBuffer buffer)
        {
            Assert.That(buffer.Width == 4);
            Assert.That(buffer.Height == 5);
            for (ushort y = 0; y < buffer.Height; y++)
            for (ushort x = 0; x < buffer.Width; x++)
                switch (x % 3)
                {
                    case 0:
                    {
                        Assert.That(buffer.Pixels[x, y].Foreground.Symbol.Complex == $"{x},{y}");
                        Assert.That(buffer.Pixels[x, y].Foreground.Color == Colors.Blue);
                    }
                        break;
                    case 1:
                    {
                        Assert.That(buffer.Pixels[x, y] == Pixel.Empty);
                    }
                        break;
                    case 2:
                        Assert.That(buffer.Pixels[x, y].Foreground.Symbol.Complex == $"{x},{y}");
                        Assert.That(buffer.Pixels[x, y].Foreground.Color == Colors.White);
                        Assert.That(buffer.Pixels[x, y].Foreground.Style == FontStyle.Italic);
                        Assert.That(buffer.Pixels[x, y].Foreground.Weight == FontWeight.Bold);
                        Assert.That(buffer.Pixels[x, y].Foreground.TextDecoration == TextDecorationLocation.Underline);
                        break;
                }
        }

        private static void AssertBufferEqual(PixelBuffer buffer1, PixelBuffer buffer2)
        {
            Assert.That(buffer1.Width == buffer2.Width);
            Assert.That(buffer1.Height == buffer2.Height);
            for (ushort y = 0; y < buffer1.Height; y++)
            for (ushort x = 0; x < buffer1.Width; x++)
                Assert.AreEqual(buffer1.Pixels[x, y], buffer2.Pixels[x, y]);
        }


        [Test]
        public void Constructor()
        {
            PixelBuffer buffer = CreateBuffer();
            AssertBuffer(buffer);
        }

        [Test]
        public void JsonSerialization()
        {
            PixelBuffer buffer = CreateBuffer();
            string json = JsonConvert.SerializeObject(buffer);
            var buffer2 = JsonConvert.DeserializeObject<PixelBuffer>(json);
            AssertBufferEqual(buffer, buffer2);
        }
    }
}