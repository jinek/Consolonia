using System.Threading.Tasks;
using Avalonia.Input;
using Consolonia.Gallery.Tests.Base;
using Consolonia.NUnit;
using NUnit.Framework;

namespace Consolonia.Gallery.Tests
{
    [TestFixture]
    internal class NumericUpDownTests : GalleryTestsBaseBase
    {
        [Test]
        public async Task PerformSingleTest()
        {
            for (int i = 0; i < 7; i++)
                await UITest.KeyInput(Key.Tab);

            await UITest.AssertHasText("│ Minimum:    0  ▴▾");
            await UITest.KeyInput(Key.Up);
            await UITest.AssertHasText("│ Minimum:    1.0  ▴▾");
            await UITest.KeyInput(Key.Down);
            await UITest.AssertHasText("│ Minimum:    0.0  ▴▾");
        }
    }
}