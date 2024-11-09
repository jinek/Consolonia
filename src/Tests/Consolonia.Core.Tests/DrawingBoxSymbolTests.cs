using System.Diagnostics;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using NUnit.Framework;

namespace Consolonia.Core.Tests
{
    [TestFixture]
    public class DrawingBoxSymbolTests
    {
        [Test]
        public void Constructor()
        {
            ISymbol symbol = new DrawingBoxSymbol(0b0000_1111);
            Assert.That(symbol.Text, Is.EqualTo("┼"));
        }

        [Test]
        public void Blend()
        {
            ISymbol symbol = new DrawingBoxSymbol(0b0000_0101);
            Assert.That(symbol.Text, Is.EqualTo("─"));
            ISymbol symbolAbove = new DrawingBoxSymbol(0b0000_1010);
            Assert.That(symbolAbove.Text, Is.EqualTo("│"));
            ISymbol newSymbol = symbol.Blend(ref symbolAbove);
            Assert.That(newSymbol.Text, Is.EqualTo("┼"));
        }

        [Test]
        public void BlendAllSymbols()
        {
            var symbols = new (byte, string)[]
            {
                (0b0000_0000, " "),
                (0b0000_0001, "╵"),
                (0b0000_0010, "╶"),
                (0b0000_0011, "└"),
                (0b0000_0100, "╷"),
                (0b0000_0101, "│"),
                (0b0000_0110, "┌"),
                (0b0000_0111, "├"),
                (0b0000_1000, "╴"),
                (0b0000_1001, "┘"),
                (0b0000_1010, "─"),
                (0b0000_1011, "┴"),
                (0b0000_1100, "┐"),
                (0b0000_1101, "┤"),
                (0b0000_1110, "┬"),
                (0b0000_1111, "┼")
            };

            foreach (var (code1, _) in symbols)
            {
                foreach (var (code2, _) in symbols)
                {
                    ISymbol symbol1 = new DrawingBoxSymbol(code1);
                    ISymbol symbol2 = new DrawingBoxSymbol(code2);
                    ISymbol blendedSymbol = symbol1.Blend(ref symbol2);
                    if (symbol1.Text != symbol2.Text)
                        Debug.WriteLine($"{symbol1.Text} + {symbol2.Text} => {blendedSymbol.Text}");
                    Assert.That(blendedSymbol.Text, Is.Not.Null);
                }
            }
        }

   }
}