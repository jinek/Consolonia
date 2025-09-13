// DUPFINDER_ignore

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Consolonia.Core.Tests.WithLifetimeFixture
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
            Rune rune = "👍".EnumerateRunes().First();
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
        public void Equality()
        {
            var symbol = new SimpleSymbol("a");
            var symbol2 = new SimpleSymbol("a");
            Assert.That(symbol.Equals((object)symbol2));
            Assert.That(symbol.Equals(symbol2));
            Assert.That(symbol == symbol2);
        }

        [Test]
        public void EqualityISymbol()
        {
            ISymbol symbol = new SimpleSymbol("a");
            ISymbol symbol2 = new SimpleSymbol("a");
            Assert.That(symbol.Equals(symbol2));
            Assert.That(symbol.Equals(symbol2));
        }

        [Test]
        public void Inequality()
        {
            var symbol = new SimpleSymbol("a");
            var symbol2 = new SimpleSymbol("b");
            Assert.That(!symbol.Equals((object)symbol2));
            Assert.That(!symbol.Equals(symbol2));
            Assert.That(symbol != symbol2);
        }

        [Test]
        public void InequalityISymbol()
        {
            ISymbol symbol = new SimpleSymbol("a");
            ISymbol symbol2 = new SimpleSymbol("b");
            Assert.That(!symbol.Equals(symbol2));
            Assert.That(!symbol.Equals(symbol2));
        }

        [Test]
        public void HashCode()
        {
            var set = new HashSet<SimpleSymbol>();
            set.Add(new SimpleSymbol("a"));
            set.Add(new SimpleSymbol("a"));
            Assert.That(set.Count, Is.EqualTo(1));

            var set2 = new HashSet<ISymbol>();
            set2.Add(new SimpleSymbol("a"));
            set2.Add(new SimpleSymbol("a"));
            Assert.That(set2.Count, Is.EqualTo(1));
        }

        [Test]
        public void IsWhiteSpace()
        {
            Assert.That(new SimpleSymbol(string.Empty).NothingToDraw(), Is.True);
            Assert.That(new SimpleSymbol(" ").NothingToDraw(), Is.False);
            Assert.That(new SimpleSymbol("a").NothingToDraw(), Is.False);
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

        [Test]
        public void JsonSerialization()
        {
            ISymbol symbol = new SimpleSymbol("a");
            string json = JsonConvert.SerializeObject(symbol);
            var deserializedSymbol = JsonConvert.DeserializeObject<ISymbol>(json);
            Assert.That(deserializedSymbol.Equals(symbol));
        }

        [Test]
        [TestCase("👍")]
        [TestCase("👨‍👩‍👧‍👦")]
        [TestCase("“")]
        [TestCase("”")]
        [TestCase("\"")]
        public void JsonSerializationCases(string text)
        {
            ISymbol symbol = new SimpleSymbol(text);
            string json = JsonConvert.SerializeObject(symbol);
            var deserializedSymbol = JsonConvert.DeserializeObject<ISymbol>(json);
            Assert.That(deserializedSymbol.Equals(symbol));
        }

        [Test]
        public void JsonSerializationDefault()
        {
            ISymbol symbol = new SimpleSymbol();
            string json = JsonConvert.SerializeObject(symbol);
            var deserializedSymbol = JsonConvert.DeserializeObject<ISymbol>(json);
            Assert.That(deserializedSymbol.Equals(symbol));
        }
    }
}