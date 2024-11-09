using Avalonia.Media;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using NUnit.Framework;

namespace Consolonia.Core.Tests
{
    [TestFixture]
    public class SimpleSymbolTests
    {
        [Test]
        public void CharConstructor()
        {
            ISymbol symbol = new SimpleSymbol('a');
            Assert.That(symbol.GetCharacter(), Is.EqualTo('a'));
        }

        [Test]
        public void SimpleSymbolIsWhiteSpace()
        {
            Assert.That(new SimpleSymbol('\0').IsWhiteSpace(), Is.True);
            Assert.That(new SimpleSymbol(' ').IsWhiteSpace(), Is.False);
            Assert.That(new SimpleSymbol('a').IsWhiteSpace(), Is.False);
        }

        [Test]
        public void SimpleSymbolBlend()
        {
            ISymbol symbol = new SimpleSymbol('a');
            ISymbol symbolAbove = new SimpleSymbol('b');
            ISymbol newSymbol = symbol.Blend(ref symbolAbove);
            Assert.That(newSymbol.GetCharacter(), Is.EqualTo('b'));
        }

        [Test]
        public void SimpleSymbolBlendWithWhiteSpace()
        {
            ISymbol symbol = new SimpleSymbol(' ');
            ISymbol symbolAbove = new SimpleSymbol('b');
            ISymbol newSymbol = symbol.Blend(ref symbolAbove);
            Assert.That(newSymbol.GetCharacter(), Is.EqualTo('b'));
        }

        [Test]
        public void SimpleSymbolBlendWithEmpty()
        {
            ISymbol symbol = new SimpleSymbol('a');
            ISymbol symbolAbove = new SimpleSymbol('\0');
            ISymbol newSymbol = symbol.Blend(ref symbolAbove);
            Assert.That(newSymbol.GetCharacter(), Is.EqualTo('a'));
        }
    }
}