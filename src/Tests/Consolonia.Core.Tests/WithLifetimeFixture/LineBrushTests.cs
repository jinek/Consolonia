using Avalonia.Media;
using Consolonia.Controls.Brushes;
using NUnit.Framework;

namespace Consolonia.Core.Tests.WithLifetimeFixture
{
    [TestFixture]
    public class LineBrushTests
    {
        [Test]
        public void ToImmutablePreservesInnerBrushAndStyle()
        {
            var lineBrush = new LineBrush { Brush = Brushes.Red, LineStyle = LineStyle.DoubleLine };
            var immutable = lineBrush.ToImmutable();

            Assert.That(immutable, Is.Not.Null);

            var innerBrushProp = immutable.GetType().GetProperty("Brush");
            Assert.That(innerBrushProp, Is.Not.Null);
            var innerBrush = innerBrushProp.GetValue(immutable) as ISolidColorBrush;
            Assert.That(innerBrush, Is.Not.Null);
            Assert.That(innerBrush.Color, Is.EqualTo(Colors.Red));

            var styleProp = immutable.GetType().GetProperty("LineStyle");
            Assert.That(styleProp, Is.Not.Null);
            var lineStyles = (LineStyles)styleProp.GetValue(immutable);
            Assert.That(lineStyles.Left, Is.EqualTo(LineStyle.DoubleLine));
        }
    }
}
