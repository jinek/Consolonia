using Consolonia.Controls;
using Consolonia.Controls.Brushes;
using NUnit.Framework;

namespace Consolonia.Core.Tests.WithLifetimeFixture
{
    [TestFixture]
    public class MoveConsoleCaretToPositionBrushTests
    {
        [Test]
        public void ToImmutablePreservesCaretStyle()
        {
            var brush = new MoveConsoleCaretToPositionBrush { CaretStyle = CaretStyle.SteadyUnderline };
            var immutable = brush.ToImmutable();

            Assert.That(immutable, Is.Not.Null);

            var styleProp = immutable.GetType().GetProperty("CaretStyle");
            Assert.That(styleProp, Is.Not.Null);
            var style = (CaretStyle)styleProp.GetValue(immutable);
            Assert.That(style, Is.EqualTo(CaretStyle.SteadyUnderline));
        }
    }
}
