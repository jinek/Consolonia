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
            PixelBufferSurface layerManager = new PixelBufferSurface(10, 10);
            FillBuffer(layerManager, "T");

            var layer1 = layerManager.CreateLayer(2, 2, 5, 5);
            FillBuffer(layer1, "1");
            var layer2 = layerManager.CreateLayer(3, 3, 5, 2);
            FillBuffer(layer2, "2");

            layerManager.BlendLayers();
            var result = BufferToString(layerManager);
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
            PixelBufferSurface layers = new PixelBufferSurface(10, 10);
            FillBuffer(layers, "T");

            var layer1 = layers.CreateLayer(2, 2, 5, 5);
            FillBuffer(layer1, "1");
            var layer2 = layers.CreateLayer(3, 3, 5, 2);
            FillBuffer(layer2, "2");
            layer1.BringToFront();

            layers.BlendLayers();
            var result = BufferToString(layers);
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
            PixelBufferSurface layerManager = new PixelBufferSurface(10, 10);
            FillBuffer(layerManager, "T");

            var layer1 = layerManager.CreateLayer(2, 2, 5, 5);
            FillBuffer(layer1, "1");
            var layer2 = layerManager.CreateLayer(3, 3, 5, 2);
            FillBuffer(layer2, "2");
            layer2.SendToBack();

            layerManager.BlendLayers();
            var result = BufferToString(layerManager);
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
            for (ushort y = 0; y < buffer.Size.Height; y++)
                for (ushort x = 0; x < buffer.Size.Width; x++)
                    buffer[x, y] = new Pixel(new SimpleSymbol(symbol), Colors.White);
        }

        private static string BufferToString(PixelBuffer buffer)
        {
            var sb = new StringBuilder();
            for (ushort y = 0; y < buffer.Size.Height; y++)
            {
                for (ushort x = 0; x < buffer.Size.Width; x++)
                    sb.Append(buffer[x, y].Foreground.Symbol.Text);
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}