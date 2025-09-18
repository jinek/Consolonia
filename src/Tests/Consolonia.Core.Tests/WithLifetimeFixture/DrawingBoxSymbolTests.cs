using System.Collections.Generic;
using System.Diagnostics;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Consolonia.Core.Tests.WithLifetimeFixture
{
    [TestFixture]
    public class SymbolTests
    {
        [Test]
        public void Constructor()
        {
            var symbol = new Symbol(0b0000_1111);
            Assert.That(symbol.Text, Is.EqualTo("┼"));
        }

        [Test]
        public void Blend()
        {
            var symbol = new Symbol(0b0000_0101);
            Assert.That(symbol.Text, Is.EqualTo("─"));
            var symbolAbove = new Symbol(0b0000_1010);
            Assert.That(symbolAbove.Text, Is.EqualTo("│"));
            var newSymbol = symbol.Blend(ref symbolAbove);
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

            foreach ((byte code1, string _) in symbols)
            foreach ((byte code2, string _) in symbols)
            {
                var symbol1 = new Symbol(code1);
                var symbol2 = new Symbol(code2);
                var blendedSymbol = symbol1.Blend(ref symbol2);
                if (symbol1.Text != symbol2.Text)
                    Debug.WriteLine($"{symbol1.Text} + {symbol2.Text} => {blendedSymbol.Text}");
                Assert.That(blendedSymbol.Text, Is.Not.Null);
            }
        }

        [Test]
        public void Equality()
        {
            var symbol = new Symbol(0b0000_1111);
            var symbol2 = new Symbol(0b0000_1111);
            Assert.That(symbol.Equals((object)symbol2));
            Assert.That(symbol.Equals(symbol2));
            Assert.That(symbol == symbol2);
        }

        [Test]
        public void Equalityvar()
        {
            var symbol = new Symbol(0b0000_1111);
            var symbol2 = new Symbol(0b0000_1111);
            Assert.That(symbol.Equals(symbol2));
            Assert.That(symbol.Equals(symbol2));
        }

        [Test]
        public void Inequality()
        {
            var symbol = new Symbol(0b0000_1111);
            var symbol2 = new Symbol(0b0001_0001);
            Assert.That(!symbol.Equals((object)symbol2));
            Assert.That(!symbol.Equals(symbol2));
            Assert.That(symbol != symbol2);
        }

        [Test]
        public void InequalitySymbol()
        {
            var symbol = new Symbol(0b0000_1111);
            var symbol2 = new Symbol(0b0000_1010);
            Assert.That(!symbol.Equals(symbol2));
            Assert.That(!symbol.Equals(symbol2));
        }

        [Test]
        public void Hash()
        {
            var set = new HashSet<Symbol>();
            set.Add(new Symbol(0b0000_1111));
            set.Add(new Symbol(0b0000_1111));
            Assert.That(set.Count, Is.EqualTo(1));

            var set2 = new HashSet<Symbol>();
            set2.Add(new Symbol(0b0000_1111));
            set2.Add(new Symbol(0b0000_1111));
            Assert.That(set2.Count, Is.EqualTo(1));
        }

        [Test]
        public void JsonSerialization()
        {
            var symbol = new Symbol(0b0000_1111);
            string json = JsonConvert.SerializeObject(symbol);
            var deserializedSymbol = JsonConvert.DeserializeObject<Symbol>(json);
            Assert.That(symbol.Equals(deserializedSymbol));
        }
    }
}