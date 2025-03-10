using System.Text;
using Avalonia.Media;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using NUnit.Framework;

namespace Consolonia.Core.Tests.WithLifetimeFixture
{
    [TestFixture]
    public class PixelBufferLayerTests
    {
        [Test]
        public void BitBltLayers()
        {
            PixelBufferLayerManager layerManager = new PixelBufferLayerManager(10, 10);
            FillBuffer(layerManager.PixelBuffer, "T");

            var layer1 = layerManager.CreateLayer(2, 2, 5, 5);
            FillBuffer(layer1, "1");
            var layer2 = layerManager.CreateLayer(3, 3, 5, 2);
            FillBuffer(layer2, "2");

            layerManager.Blend();
            var result = BufferToString(layerManager.PixelBuffer);
            Assert.AreEqual("""
                TTTTTTTTTT
                TTTTTTTTTT
                TT11111TTT
                TT122222TT
                TT122222TT
                TT11111TTT
                TT11111TTT
                TTTTTTTTTT
                TTTTTTTTTT
                TTTTTTTTTT
                """.Trim(),
                result.Trim());
        }

        [Test]
        public void BitBltLayersBringToFront()
        {
            PixelBufferLayerManager layerManager = new PixelBufferLayerManager(10, 10);
            FillBuffer(layerManager.PixelBuffer, "T");

            var layer1 = layerManager.CreateLayer(2, 2, 5, 5);
            FillBuffer(layer1, "1");
            var layer2 = layerManager.CreateLayer(3, 3, 5, 2);
            FillBuffer(layer2, "2");
            layer1.BringToFront();

            layerManager.Blend();
            var result = BufferToString(layerManager.PixelBuffer);
            Assert.AreEqual("""
                TTTTTTTTTT
                TTTTTTTTTT
                TT11111TTT
                TT111112TT
                TT111112TT
                TT11111TTT
                TT11111TTT
                TTTTTTTTTT
                TTTTTTTTTT
                TTTTTTTTTT
                """.Trim(),
                result.Trim());
        }

        [Test]
        public void BitBltLayersSendToBack()
        {
            PixelBufferLayerManager layerManager = new PixelBufferLayerManager(10, 10);
            FillBuffer(layerManager.PixelBuffer, "T");

            var layer1 = layerManager.CreateLayer(2, 2, 5, 5);
            FillBuffer(layer1, "1");
            var layer2 = layerManager.CreateLayer(3, 3, 5, 2);
            FillBuffer(layer2, "2");
            layer2.SendToBack();

            layerManager.Blend();
            var result = BufferToString(layerManager.PixelBuffer);
            Assert.AreEqual("""
                TTTTTTTTTT
                TTTTTTTTTT
                TT11111TTT
                TT111112TT
                TT111112TT
                TT11111TTT
                TT11111TTT
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