using System.Collections.Generic;
using System.Diagnostics;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Consolonia.Core.Tests.WithLifetimeFixture
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

            foreach ((byte code1, string _) in symbols)
            foreach ((byte code2, string _) in symbols)
            {
                ISymbol symbol1 = new DrawingBoxSymbol(code1);
                ISymbol symbol2 = new DrawingBoxSymbol(code2);
                ISymbol blendedSymbol = symbol1.Blend(ref symbol2);
                if (symbol1.Text != symbol2.Text)
                    Debug.WriteLine($"{symbol1.Text} + {symbol2.Text} => {blendedSymbol.Text}");
                Assert.That(blendedSymbol.Text, Is.Not.Null);
            }
        }

        [Test]
        public void Equality()
        {
            var symbol = new DrawingBoxSymbol(0b0000_1111);
            var symbol2 = new DrawingBoxSymbol(0b0000_1111);
            Assert.That(symbol.Equals((object)symbol2));
            Assert.That(symbol.Equals(symbol2));
            Assert.That(symbol == symbol2);
        }

        [Test]
        public void EqualityISymbol()
        {
            ISymbol symbol = new DrawingBoxSymbol(0b0000_1111);
            ISymbol symbol2 = new DrawingBoxSymbol(0b0000_1111);
            Assert.That(symbol.Equals(symbol2));
            Assert.That(symbol.Equals(symbol2));
        }

        [Test]
        public void Inequality()
        {
            var symbol = new DrawingBoxSymbol(0b0000_1111);
            var symbol2 = new DrawingBoxSymbol(0b0000_0000);
            Assert.That(!symbol.Equals((object)symbol2));
            Assert.That(!symbol.Equals(symbol2));
            Assert.That(symbol != symbol2);
        }

        [Test]
        public void InequalityISymbol()
        {
            ISymbol symbol = new DrawingBoxSymbol(0b0000_1111);
            ISymbol symbol2 = new DrawingBoxSymbol(0b0000_0000);
            Assert.That(!symbol.Equals(symbol2));
            Assert.That(!symbol.Equals(symbol2));
        }

        [Test]
        public void Hash()
        {
            var set = new HashSet<DrawingBoxSymbol>();
            set.Add(new DrawingBoxSymbol(0b0000_1111));
            set.Add(new DrawingBoxSymbol(0b0000_1111));
            Assert.That(set.Count, Is.EqualTo(1));

            var set2 = new HashSet<ISymbol>();
            set2.Add(new DrawingBoxSymbol(0b0000_1111));
            set2.Add(new DrawingBoxSymbol(0b0000_1111));
            Assert.That(set2.Count, Is.EqualTo(1));
        }

        [Test]
        public void JsonSerialization()
        {
            var symbol = new DrawingBoxSymbol(0b0000_1111);
            string json = JsonConvert.SerializeObject(symbol);
            var deserializedSymbol = JsonConvert.DeserializeObject<ISymbol>(json);
            Assert.That(symbol.Equals(deserializedSymbol));
        }
    }
}