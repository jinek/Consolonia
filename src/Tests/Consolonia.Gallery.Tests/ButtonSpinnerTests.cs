using System.Threading.Tasks;
using Avalonia.Input;
using Consolonia.Gallery.Tests.Base;
using Consolonia.NUnit;
using NUnit.Framework;

namespace Consolonia.Gallery.Tests
{
    [TestFixture]
    internal class ButtonSpinnerTests : GalleryTestsBaseBase
    {
        [Test]
        public async Task PerformSingleTest()
        {
            await UITest.KeyInput(Key.Tab);
            await UITest.KeyInput(Key.Tab);
            await UITest.KeyInput(Key.Tab);
            await UITest.AssertHasText("Everest");
            await UITest.KeyInput(Key.Down);
            await UITest.AssertHasText("Annapurna");
            await UITest.KeyInput(Key.Down);
            await UITest.AssertHasText("Nanga Parbat");
            await UITest.KeyInput(Key.Up);
            await UITest.AssertHasText("Annapurna");
        }
    }
}