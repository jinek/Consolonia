using Avalonia.Media;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Newtonsoft.Json;
using NLog.Config;
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
                    buffer[x, y] = new Pixel(new Symbol("👍"), Colors.Blue);
                else if (x % 3 == 2)
                    buffer[x, y] = new Pixel(new Symbol($"{x}"), Colors.White, FontStyle.Italic,
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
                        Assert.That(buffer[x, y].Foreground.Symbol.Complex == "👍");
                        Assert.That(buffer[x, y].Foreground.Color == Colors.Blue);
                    }
                        break;
                    case 1:
                    {
                        Assert.That(buffer[x, y] == Pixel.Empty);
                    }
                        break;
                    case 2:
                        Assert.That(buffer[x, y].Foreground.Symbol.Character == $"{x}"[0]);
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
        public void TestSetChar()
        {
            PixelBuffer buffer = CreateBuffer();
            buffer[1, 1] = new Pixel(new Symbol('A'), Colors.Red);
            Assert.That(buffer[1, 1].Foreground.Symbol.Character == 'A');
            Assert.That(buffer[1, 1].Foreground.Color == Colors.Red);
        }

        [Test]
        public void TestSetWideCharEmptySpace()
        {
            var buffer = new PixelBuffer(4, 1);
            
            // Set a wide character (emoji) at position 0
            buffer[0, 0] = new Pixel(new Symbol("👍"), Colors.Yellow);
            
            // Verify the wide character is at position 0 with width 2
            Assert.That(buffer[0, 0].Foreground.Symbol.Complex, Is.EqualTo("👍"));
            Assert.That(buffer[0, 0].Foreground.Symbol.Width, Is.EqualTo(2));
            Assert.That(buffer[0, 0].Foreground.Color, Is.EqualTo(Colors.Yellow));
            
            // Verify the next position is marked as Empty (overlapped by wide char)
            Assert.That(buffer[1, 0].Foreground.Symbol.Width, Is.EqualTo(0));
            Assert.That(buffer[1, 0], Is.EqualTo(Pixel.Empty));
        }

        [Test]
        public void TestSetWideCharOverwritesExisting()
        {
            var buffer = new PixelBuffer(4, 1);
            
            // Set normal characters first
            buffer[0, 0] = new Pixel(new Symbol('A'), Colors.Blue);
            buffer[1, 0] = new Pixel(new Symbol('B'), Colors.Blue);
            
            // Overwrite with wide character
            buffer[0, 0] = new Pixel(new Symbol("👍"), Colors.Yellow);
            
            // Verify wide character replaced the first char and marked second as Empty
            Assert.That(buffer[0, 0].Foreground.Symbol.Complex, Is.EqualTo("👍"));
            Assert.That(buffer[1, 0], Is.EqualTo(Pixel.Empty));
        }

        [Test]
        public void TestOverwriteWideCharWithNormalChar()
        {
            var buffer = new PixelBuffer(4, 1);
            
            // Set a wide character
            buffer[0, 0] = new Pixel(new Symbol("👍"), Colors.Yellow);
            Assert.That(buffer[1, 0], Is.EqualTo(Pixel.Empty));
            
            // Overwrite wide char with normal char
            buffer[0, 0] = new Pixel(new Symbol('A'), Colors.Blue);
            
            // Verify the wide char is replaced and the next position is restored to Space
            Assert.That(buffer[0, 0].Foreground.Symbol.Character, Is.EqualTo('A'));
            Assert.That(buffer[0, 0].Foreground.Color, Is.EqualTo(Colors.Blue));
            
            // Position 1 should be blended with Space (no longer Empty)
            Assert.That(buffer[1, 0].Foreground.Symbol.Character, Is.EqualTo(' '));
        }

        [Test]
        public void TestOverwriteEmptySpaceOfWideChar()
        {
            var buffer = new PixelBuffer(4, 1);
            
            // Set a wide character at position 0
            buffer[0, 0] = new Pixel(new Symbol("👍"), Colors.Yellow);
            Assert.That(buffer[1, 0], Is.EqualTo(Pixel.Empty));
            
            // Write into the empty space (position 1) that's part of the wide char
            buffer[1, 0] = new Pixel(new Symbol('B'), Colors.Green);
            
            // The previous wide char at position 0 should be converted to Space
            Assert.That(buffer[0, 0].Foreground.Symbol.Character, Is.EqualTo(' '));
            
            // Position 1 should have the new character
            Assert.That(buffer[1, 0].Foreground.Symbol.Character, Is.EqualTo('B'));
            Assert.That(buffer[1, 0].Foreground.Color, Is.EqualTo(Colors.Green));
        }

        [Test]
        public void TestMultipleWideCharsSequential()
        {
            var buffer = new PixelBuffer(6, 1);
            
            // Set multiple wide characters in sequence
            buffer[0, 0] = new Pixel(new Symbol("👍"), Colors.Yellow);
            buffer[2, 0] = new Pixel(new Symbol("👨"), Colors.Blue);
            buffer[4, 0] = new Pixel(new Symbol("🎉"), Colors.Red);
            
            // Verify all wide characters are set correctly
            Assert.That(buffer[0, 0].Foreground.Symbol.Complex, Is.EqualTo("👍"));
            Assert.That(buffer[1, 0], Is.EqualTo(Pixel.Empty));
            
            Assert.That(buffer[2, 0].Foreground.Symbol.Complex, Is.EqualTo("👨"));
            Assert.That(buffer[3, 0], Is.EqualTo(Pixel.Empty));
            
            Assert.That(buffer[4, 0].Foreground.Symbol.Complex, Is.EqualTo("🎉"));
            Assert.That(buffer[5, 0], Is.EqualTo(Pixel.Empty));
        }

        [Test]
        public void TestWideCharOverlapsMultiplePositions()
        {
            var buffer = new PixelBuffer(5, 1);
            
            // Fill with normal characters
            for (ushort i = 0; i < 5; i++)
                buffer[i, 0] = new Pixel(new Symbol((char)('A' + i)), Colors.White);
            
            // Set a wide character that overlaps position 2 and 3
            buffer[2, 0] = new Pixel(new Symbol("👍"), Colors.Yellow);
            
            // Positions 0 and 1 should be unchanged
            Assert.That(buffer[0, 0].Foreground.Symbol.Character, Is.EqualTo('A'));
            Assert.That(buffer[1, 0].Foreground.Symbol.Character, Is.EqualTo('B'));
            
            // Position 2 should have the wide character
            Assert.That(buffer[2, 0].Foreground.Symbol.Complex, Is.EqualTo("👍"));
            
            // Position 3 should be Empty (overlapped)
            Assert.That(buffer[3, 0], Is.EqualTo(Pixel.Empty));
            
            // Position 4 should be unchanged
            Assert.That(buffer[4, 0].Foreground.Symbol.Character, Is.EqualTo('E'));
        }

        [Test]
        public void TestWideCharAtBufferBoundary()
        {
            var buffer = new PixelBuffer(3, 1);
            
            // Set wide character at the last possible position (should only overlap what fits)
            buffer[2, 0] = new Pixel(new Symbol("👍"), Colors.Yellow);
            
            // Position 2 should have the wide character
            Assert.That(buffer[2, 0].Foreground.Symbol.Complex, Is.EqualTo("👍"));
            
            // No position 3 to check (buffer boundary)
        }

        [Test]
        public void TestWideCharReplaceWithAnotherWideChar()
        {
            var buffer = new PixelBuffer(4, 1);
            
            // Set first wide character
            buffer[0, 0] = new Pixel(new Symbol("👍"), Colors.Yellow);
            Assert.That(buffer[1, 0], Is.EqualTo(Pixel.Empty));
            
            // Replace with another wide character
            buffer[0, 0] = new Pixel(new Symbol("🎉"), Colors.Red);
            
            // Verify the replacement
            Assert.That(buffer[0, 0].Foreground.Symbol.Complex, Is.EqualTo("🎉"));
            Assert.That(buffer[0, 0].Foreground.Color, Is.EqualTo(Colors.Red));
            Assert.That(buffer[1, 0], Is.EqualTo(Pixel.Empty));
        }

        [Test]
        public void TestComplexEmojiMultiCharSequence()
        {
            var buffer = new PixelBuffer(4, 1);
            
            // Set a complex multi-character emoji (family emoji)
            buffer[0, 0] = new Pixel(new Symbol("👨‍👩‍👧‍👦"), Colors.Purple);
            
            // Verify it's treated as width 2
            Assert.That(buffer[0, 0].Foreground.Symbol.Complex, Is.EqualTo("👨‍👩‍👧‍👦"));
            Assert.That(buffer[0, 0].Foreground.Symbol.Width, Is.EqualTo(2));
            Assert.That(buffer[1, 0], Is.EqualTo(Pixel.Empty));
        }
    }
}