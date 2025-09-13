using System;
using Consolonia.Controls.Brushes;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Consolonia.Core.Tests.WithLifetimeFixture
{
    [TestFixture]
    public class LineStylesTests
    {
        [Test]
        public void TestParseSingle()
        {
            LineStyles lineStyles = LineStyles.Parse("SingleLine");
            Assert.That(lineStyles.Left, Is.EqualTo(LineStyle.SingleLine));
            Assert.That(lineStyles.Top, Is.EqualTo(LineStyle.SingleLine));
            Assert.That(lineStyles.Right, Is.EqualTo(LineStyle.SingleLine));
            Assert.That(lineStyles.Bottom, Is.EqualTo(LineStyle.SingleLine));
        }

        [Test]
        public void TestParseDouble()
        {
            LineStyles lineStyles = LineStyles.Parse("SingleLine,DoubleLine");
            Assert.That(lineStyles.Left, Is.EqualTo(LineStyle.SingleLine));
            Assert.That(lineStyles.Top, Is.EqualTo(LineStyle.DoubleLine));
            Assert.That(lineStyles.Right, Is.EqualTo(LineStyle.DoubleLine));
            Assert.That(lineStyles.Bottom, Is.EqualTo(LineStyle.DoubleLine));
        }

        [Test]
        public void TestParseTriple()
        {
            LineStyles lineStyles = LineStyles.Parse("SingleLine,DoubleLine,Edge");
            Assert.That(lineStyles.Left, Is.EqualTo(LineStyle.SingleLine));
            Assert.That(lineStyles.Top, Is.EqualTo(LineStyle.DoubleLine));
            Assert.That(lineStyles.Right, Is.EqualTo(LineStyle.Edge));
            Assert.That(lineStyles.Bottom, Is.EqualTo(LineStyle.Edge));
        }

        [Test]
        public void TestParseQuad()
        {
            LineStyles lineStyles = LineStyles.Parse("SingleLine DoubleLine Edge Bold");
            Assert.That(lineStyles.Left, Is.EqualTo(LineStyle.SingleLine));
            Assert.That(lineStyles.Top, Is.EqualTo(LineStyle.DoubleLine));
            Assert.That(lineStyles.Right, Is.EqualTo(LineStyle.Edge));
            Assert.That(lineStyles.Bottom, Is.EqualTo(LineStyle.Bold));
        }

        [Test]
        public void TestParseThrows()
        {
            Assert.Throws<ArgumentException>(() => LineStyles.Parse("SingleLine,DoubleLine,Edge,Bold,Extra"));
        }

        [Test]
        public void TestEmptyConstructor()
        {
            var lineStyles = new LineStyles();
            Assert.That(lineStyles.Left, Is.EqualTo(LineStyle.SingleLine));
            Assert.That(lineStyles.Top, Is.EqualTo(LineStyle.SingleLine));
            Assert.That(lineStyles.Right, Is.EqualTo(LineStyle.SingleLine));
            Assert.That(lineStyles.Bottom, Is.EqualTo(LineStyle.SingleLine));
        }

        [Test]
        public void TestConstructorSingle()
        {
            var lineStyles = new LineStyles(LineStyle.Edge);
            Assert.That(lineStyles.Left, Is.EqualTo(LineStyle.Edge));
            Assert.That(lineStyles.Top, Is.EqualTo(LineStyle.Edge));
            Assert.That(lineStyles.Right, Is.EqualTo(LineStyle.Edge));
            Assert.That(lineStyles.Bottom, Is.EqualTo(LineStyle.Edge));
        }

        [Test]
        public void TestConstructorDouble()
        {
            var lineStyles = new LineStyles(LineStyle.SingleLine, LineStyle.DoubleLine);
            Assert.That(lineStyles.Left, Is.EqualTo(LineStyle.SingleLine));
            Assert.That(lineStyles.Top, Is.EqualTo(LineStyle.DoubleLine));
            Assert.That(lineStyles.Right, Is.EqualTo(LineStyle.DoubleLine));
            Assert.That(lineStyles.Bottom, Is.EqualTo(LineStyle.DoubleLine));
        }

        [Test]
        public void TestConstructorTriple()
        {
            var lineStyles = new LineStyles(LineStyle.SingleLine, LineStyle.DoubleLine, LineStyle.Edge);
            Assert.That(lineStyles.Left, Is.EqualTo(LineStyle.SingleLine));
            Assert.That(lineStyles.Top, Is.EqualTo(LineStyle.DoubleLine));
            Assert.That(lineStyles.Right, Is.EqualTo(LineStyle.Edge));
            Assert.That(lineStyles.Bottom, Is.EqualTo(LineStyle.Edge));
        }

        [Test]
        public void TestConstructorQuad()
        {
            var lineStyles = new LineStyles(LineStyle.SingleLine, LineStyle.DoubleLine, LineStyle.Edge, LineStyle.Bold);
            Assert.That(lineStyles.Left, Is.EqualTo(LineStyle.SingleLine));
            Assert.That(lineStyles.Top, Is.EqualTo(LineStyle.DoubleLine));
            Assert.That(lineStyles.Right, Is.EqualTo(LineStyle.Edge));
            Assert.That(lineStyles.Bottom, Is.EqualTo(LineStyle.Bold));
        }

        /* todo low: check this test, it's same as TestConstructorTriple
         [Test]
        public void TestConstructorQuadNull()
        {
            var lineStyles = new LineStyles(LineStyle.SingleLine, LineStyle.DoubleLine, LineStyle.Edge);
            Assert.That(lineStyles.Left, Is.EqualTo(LineStyle.SingleLine));
            Assert.That(lineStyles.Top, Is.EqualTo(LineStyle.DoubleLine));
            Assert.That(lineStyles.Right, Is.EqualTo(LineStyle.Edge));
            Assert.That(lineStyles.Bottom, Is.EqualTo(LineStyle.Edge));
        }*/

        [Test]
        public void TestConstructorQuadNullAll()
        {
            var lineStyles = new LineStyles(null, null);
            Assert.That(lineStyles.Left, Is.EqualTo(LineStyle.SingleLine));
            Assert.That(lineStyles.Top, Is.EqualTo(LineStyle.SingleLine));
            Assert.That(lineStyles.Right, Is.EqualTo(LineStyle.SingleLine));
            Assert.That(lineStyles.Bottom, Is.EqualTo(LineStyle.SingleLine));
        }

        [Test]
        public void TestImplicitString()
        {
            LineStyles lineStyles = "SingleLine DoubleLine Edge Bold";
            Assert.That(lineStyles.Left, Is.EqualTo(LineStyle.SingleLine));
            Assert.That(lineStyles.Top, Is.EqualTo(LineStyle.DoubleLine));
            Assert.That(lineStyles.Right, Is.EqualTo(LineStyle.Edge));
            Assert.That(lineStyles.Bottom, Is.EqualTo(LineStyle.Bold));
        }

        [Test]
        public void TestImplicitEnum()
        {
            LineStyles lineStyles = LineStyle.DoubleLine;
            Assert.That(lineStyles.Left, Is.EqualTo(LineStyle.DoubleLine));
            Assert.That(lineStyles.Top, Is.EqualTo(LineStyle.DoubleLine));
            Assert.That(lineStyles.Right, Is.EqualTo(LineStyle.DoubleLine));
            Assert.That(lineStyles.Bottom, Is.EqualTo(LineStyle.DoubleLine));
        }

        [Test]
        public void TestJsonSerialization()
        {
            var lineStyles = new LineStyles(LineStyle.SingleLine, LineStyle.DoubleLine, LineStyle.Edge, LineStyle.Bold);
            string json = JsonConvert.SerializeObject(lineStyles);
            var deserialized = JsonConvert.DeserializeObject<LineStyles>(json);
            Assert.That(deserialized.Left, Is.EqualTo(LineStyle.SingleLine));
            Assert.That(deserialized.Top, Is.EqualTo(LineStyle.DoubleLine));
            Assert.That(deserialized.Right, Is.EqualTo(LineStyle.Edge));
            Assert.That(deserialized.Bottom, Is.EqualTo(LineStyle.Bold));
        }
    }
}