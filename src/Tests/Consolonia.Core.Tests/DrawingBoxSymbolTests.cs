using Avalonia.Media;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using NUnit.Framework;

namespace Consolonia.Core.Tests
{
    [TestFixture]
    public class DrawingBoxSymbolTests
    {
        [Test]
        public void DrawingBoxSymbolConstructor()
        {
            ISymbol symbol = new DrawingBoxSymbol(0b0000_1111);
            Assert.That(symbol.GetCharacter(), Is.EqualTo('┼'));
        }

        [Test]
        public void DrawingBoxSymbolBlend()
        {
            ISymbol symbol = new DrawingBoxSymbol(0b0000_1111);
            ISymbol symbolAbove = new DrawingBoxSymbol(0b1111_0000);
            ISymbol newSymbol = symbol.Blend(ref symbolAbove);
            Assert.That(newSymbol.GetCharacter(), Is.EqualTo('╬'));
        }

   }
}