using System.Text;
using Avalonia;
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
                    buffer[x, y] = new Pixel(new SimpleSymbol($"{x},{y}"), Colors.Blue);
                else if (x % 3 == 1)
                    buffer[x, y] = Pixel.Empty;
                else
                    buffer[x, y] = new Pixel(new SimpleSymbol($"{x},{y}"), Colors.White, FontStyle.Italic,
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
                        Assert.That(buffer[x, y].Foreground.Symbol.Text == $"{x},{y}");
                        Assert.That(buffer[x, y].Foreground.Color == Colors.Blue);
                    }
                        break;
                    case 1:
                    {
                        Assert.That(buffer[x, y] == Pixel.Empty);
                    }
                        break;
                    case 2:
                        Assert.That(buffer[x, y].Foreground.Symbol.Text == $"{x},{y}");
                        Assert.That(buffer[x, y].Foreground.Color == Colors.White);
                        Assert.That(buffer[x, y].Foreground.Style == FontStyle.Italic);
                        Assert.That(buffer[x, y].Foreground.Weight == FontWeight.Bold);
                        Assert.That(buffer[x, y].Foreground.TextDecoration == TextDecorationLocation.Underline);
                        break;
                }
        }

        private static void AssertBufferEqual(PixelBuffer buffer1, PixelBuffer buffer2)
        {
            Assert.That(buffer1.Width == buffer2.Width);
            Assert.That(buffer1.Height == buffer2.Height);
            for (ushort y = 0; y < buffer1.Height; y++)
            for (ushort x = 0; x < buffer1.Width; x++)
                Assert.AreEqual(buffer1[x, y], buffer2[x, y]);
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

        [Test]
        public void BitBlt()
        {
            PixelBuffer target = new PixelBuffer(10,10);
            FillBuffer(target, "T");
            PixelBuffer source = new PixelBuffer(5, 5);
            FillBuffer(source, "S");
            source.Blend(new PixelPoint(2, 2), target);

            var result = BufferToString(target);
            Assert.AreEqual("""
                TTTTTTTTTT
                TTTTTTTTTT
                TTSSSSSTTT
                TTSSSSSTTT
                TTSSSSSTTT
                TTSSSSSTTT
                TTSSSSSTTT
                TTTTTTTTTT
                TTTTTTTTTT
                TTTTTTTTTT
                """.Trim(),
                result.Trim());
        }

        [Test]
        public void BitBltClipOuterBounds()
        {
            PixelBuffer target = new PixelBuffer(10, 10);
            FillBuffer(target, "T");
            PixelBuffer source = new PixelBuffer(50, 50);
            FillBuffer(source, "S");
            source.Blend(new PixelPoint(2, 2), target);

            var result = BufferToString(target);
            Assert.AreEqual("""
                TTTTTTTTTT
                TTTTTTTTTT
                TTSSSSSSSS
                TTSSSSSSSS
                TTSSSSSSSS
                TTSSSSSSSS
                TTSSSSSSSS
                TTSSSSSSSS
                TTSSSSSSSS
                TTSSSSSSSS
                """.Trim(),
                result.Trim());
        }

        [Test]
        public void BitBltClipNegativeBounds()
        {
            PixelBuffer target = new PixelBuffer(10, 10);
            FillBuffer(target, "T");
            PixelBuffer source = new PixelBuffer(5, 5);
            FillBuffer(source, "S");
            source.Blend(new PixelPoint(-2, -2), target);

            var result = BufferToString(target);
            Assert.AreEqual("""
                SSSTTTTTTT
                SSSTTTTTTT
                SSSTTTTTTT
                TTTTTTTTTT
                TTTTTTTTTT
                TTTTTTTTTT
                TTTTTTTTTT
                TTTTTTTTTT
                TTTTTTTTTT
                TTTTTTTTTT
                """.Trim(),
                result.Trim());
        }

        [Test]
        public void BitBltEmpty()
        {
            PixelBuffer target = new PixelBuffer(10, 10);
            FillBuffer(target, "T");
            PixelBuffer source = new PixelBuffer(0,0);
            source.Blend(new PixelPoint(2, 2), target);

            var result = BufferToString(target);
            Assert.AreEqual("""
                TTTTTTTTTT
                TTTTTTTTTT
                TTTTTTTTTT
                TTTTTTTTTT
                TTTTTTTTTT
                TTTTTTTTTT
                TTTTTTTTTT
                TTTTTTTTTT
                TTTTTTTTTT
                TTTTTTTTTT
                """.Trim(),
                result.Trim());
        }

        private static void FillBuffer(PixelBuffer buffer, string symbol)
        {
            for (ushort y = 0; y < buffer.Height; y++)
                for (ushort x = 0; x < buffer.Width; x++)
                    buffer[x, y] = new Pixel(new SimpleSymbol(symbol), Colors.White);
        }

        private static string BufferToString(PixelBuffer buffer)
        {
            var sb = new StringBuilder();
            for (ushort y = 0; y < buffer.Height; y++)
            {
                for (ushort x = 0; x < buffer.Width; x++)
                    sb.Append(buffer[x, y].Foreground.Symbol.Text);
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}