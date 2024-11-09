using System.Linq;
using System.Text;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using NUnit.Framework;

namespace Consolonia.Core.Tests
{
    [TestFixture]
    public class SimpleSymbolTests
    {

        [Test]
        public void ConstructorChar()
        {
            ISymbol symbol = new SimpleSymbol('a');
            Assert.That(symbol.Text, Is.EqualTo("a"));
            Assert.That(symbol.Width, Is.EqualTo(1));
        }

        [Test]
        public void ConstructorString()
        {
            ISymbol symbol = new SimpleSymbol("a");
            Assert.That(symbol.Text, Is.EqualTo("a"));
            Assert.That(symbol.Width, Is.EqualTo(1));
        }

        [Test]
        public void ConstructorRune()
        {
            ISymbol symbol = new SimpleSymbol(new Rune('a'));
            Assert.That(symbol.Text, Is.EqualTo("a"));
            Assert.That(symbol.Width, Is.EqualTo(1));
        }

        [Test]
        public void ConstructorRuneEmoji()
        {
            var rune = "👍".EnumerateRunes().First();
            ISymbol symbol = new SimpleSymbol(rune);
            Assert.That(symbol.Text, Is.EqualTo("👍"));
            Assert.That(symbol.Width, Is.EqualTo(2));
        }

        [Test]
        public void ConstructorComplexEmoji()
        {
            ISymbol symbol = new SimpleSymbol("👨‍👩‍👧‍👦");
            Assert.That(symbol.Text, Is.EqualTo("👨‍👩‍👧‍👦"));
            Assert.That(symbol.Width, Is.EqualTo(2));
        }

        [Test]
        public void IsWhiteSpace()
        {
            Assert.That(new SimpleSymbol(string.Empty).IsWhiteSpace(), Is.True);
            Assert.That(new SimpleSymbol(" ").IsWhiteSpace(), Is.False);
            Assert.That(new SimpleSymbol("a").IsWhiteSpace(), Is.False);
        }

        [Test]
        public void Blend()
        {
            ISymbol symbol = new SimpleSymbol("a");
            ISymbol symbolAbove = new SimpleSymbol("b");
            ISymbol newSymbol = symbol.Blend(ref symbolAbove);
            Assert.That(newSymbol.Text, Is.EqualTo("b"));
            Assert.That(newSymbol.Width, Is.EqualTo(1));
        }

        [Test]
        public void BlendWithWhiteSpace()
        {
            ISymbol symbol = new SimpleSymbol(" ");
            ISymbol symbolAbove = new SimpleSymbol("b");
            ISymbol newSymbol = symbol.Blend(ref symbolAbove);
            Assert.That(newSymbol.Text, Is.EqualTo("b"));
            Assert.That(newSymbol.Width, Is.EqualTo(1));
        }

        [Test]
        public void BlendWithEmpty()
        {
            ISymbol symbol = new SimpleSymbol("a");
            ISymbol symbolAbove = new SimpleSymbol(string.Empty);
            ISymbol newSymbol = symbol.Blend(ref symbolAbove);
            Assert.That(newSymbol.Text, Is.EqualTo("a"));
            Assert.That(newSymbol.Width, Is.EqualTo(1));
        }

        [Test]
        public void BlendWithEmoji()
        {
            ISymbol symbol = new SimpleSymbol("a");
            ISymbol symbolAbove = new SimpleSymbol("👍");
            ISymbol newSymbol = symbol.Blend(ref symbolAbove);
            Assert.That(newSymbol.Text, Is.EqualTo("👍"));
            Assert.That(newSymbol.Width, Is.EqualTo(2));
        }
    }
}