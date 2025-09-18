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
            Symbol symbol = new Symbol('a');
            Assert.That(symbol.Text, Is.EqualTo("a"));
            Assert.That(symbol.Width, Is.EqualTo(1));
        }

        [Test]
        public void ConstructorString()
        {
            Symbol symbol = new Symbol("a");
            Assert.That(symbol.Text, Is.EqualTo("a"));
            Assert.That(symbol.Width, Is.EqualTo(1));
        }

        [Test]
        public void ConstructorRune()
        {
            Symbol symbol = new Symbol(new Rune('a'));
            Assert.That(symbol.Text, Is.EqualTo("a"));
            Assert.That(symbol.Width, Is.EqualTo(1));
        }

        [Test]
        public void ConstructorRuneEmoji()
        {
            Rune rune = "👍".EnumerateRunes().First();
            Symbol symbol = new Symbol(rune);
            Assert.That(symbol.Text, Is.EqualTo("👍"));
            Assert.That(symbol.Width, Is.EqualTo(2));
        }

        [Test]
        public void ConstructorComplexEmoji()
        {
            Symbol symbol = new Symbol("👨‍👩‍👧‍👦");
            Assert.That(symbol.Text, Is.EqualTo("👨‍👩‍👧‍👦"));
            Assert.That(symbol.Width, Is.EqualTo(2));
        }

        [Test]
        public void Equality()
        {
            var symbol = new Symbol("a");
            var symbol2 = new Symbol("a");
            Assert.That(symbol.Equals((object)symbol2));
            Assert.That(symbol.Equals(symbol2));
            Assert.That(symbol == symbol2);
        }

        [Test]
        public void EqualitySymbol()
        {
            Symbol symbol = new Symbol("a");
            Symbol symbol2 = new Symbol("a");
            Assert.That(symbol.Equals(symbol2));
            Assert.That(symbol.Equals(symbol2));
        }

        [Test]
        public void Inequality()
        {
            var symbol = new Symbol("a");
            var symbol2 = new Symbol("b");
            Assert.That(!symbol.Equals((object)symbol2));
            Assert.That(!symbol.Equals(symbol2));
            Assert.That(symbol != symbol2);
        }

        [Test]
        public void InequalitySymbol()
        {
            Symbol symbol = new Symbol("a");
            Symbol symbol2 = new Symbol("b");
            Assert.That(!symbol.Equals(symbol2));
            Assert.That(!symbol.Equals(symbol2));
        }

        [Test]
        public void HashCode()
        {
            var set = new HashSet<Symbol>();
            set.Add(new Symbol("a"));
            set.Add(new Symbol("a"));
            Assert.That(set.Count, Is.EqualTo(1));

            var set2 = new HashSet<Symbol>();
            set2.Add(new Symbol("a"));
            set2.Add(new Symbol("a"));
            Assert.That(set2.Count, Is.EqualTo(1));
        }

        [Test]
        public void IsWhiteSpace()
        {
            Assert.That(new Symbol(string.Empty).NothingToDraw(), Is.True);
            Assert.That(new Symbol(" ").NothingToDraw(), Is.False);
            Assert.That(new Symbol("a").NothingToDraw(), Is.False);
        }

        [Test]
        public void Blend()
        {
            Symbol symbol = new Symbol("a");
            Symbol symbolAbove = new Symbol("b");
            Symbol newSymbol = symbol.Blend(ref symbolAbove);
            Assert.That(newSymbol.Text, Is.EqualTo("b"));
            Assert.That(newSymbol.Width, Is.EqualTo(1));
        }

        [Test]
        public void BlendWithWhiteSpace()
        {
            Symbol symbol = new Symbol(" ");
            Symbol symbolAbove = new Symbol("b");
            Symbol newSymbol = symbol.Blend(ref symbolAbove);
            Assert.That(newSymbol.Text, Is.EqualTo("b"));
            Assert.That(newSymbol.Width, Is.EqualTo(1));
        }

        [Test]
        public void BlendWithEmpty()
        {
            Symbol symbol = new Symbol("a");
            Symbol symbolAbove = new Symbol(string.Empty);
            Symbol newSymbol = symbol.Blend(ref symbolAbove);
            Assert.That(newSymbol.Text, Is.EqualTo("a"));
            Assert.That(newSymbol.Width, Is.EqualTo(1));
        }

        [Test]
        public void BlendWithEmoji()
        {
            Symbol symbol = new Symbol("a");
            Symbol symbolAbove = new Symbol("👍");
            Symbol newSymbol = symbol.Blend(ref symbolAbove);
            Assert.That(newSymbol.Text, Is.EqualTo("👍"));
            Assert.That(newSymbol.Width, Is.EqualTo(2));
        }

        [Test]
        public void JsonSerialization()
        {
            Symbol symbol = new Symbol("a");
            string json = JsonConvert.SerializeObject(symbol);
            var deserializedSymbol = JsonConvert.DeserializeObject<Symbol>(json);
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
            Symbol symbol = new Symbol(text);
            string json = JsonConvert.SerializeObject(symbol);
            var deserializedSymbol = JsonConvert.DeserializeObject<Symbol>(json);
            Assert.That(deserializedSymbol.Equals(symbol));
        }

        [Test]
        public void JsonSerializationDefault()
        {
            Symbol symbol = new Symbol();
            string json = JsonConvert.SerializeObject(symbol);
            var deserializedSymbol = JsonConvert.DeserializeObject<Symbol>(json);
            Assert.That(deserializedSymbol.Equals(symbol));
        }
    }
}