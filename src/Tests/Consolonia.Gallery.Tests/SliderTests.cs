using System.Threading.Tasks;
using Avalonia.Input;
using Consolonia.Gallery.Tests.Base;
using Consolonia.NUnit;
using NUnit.Framework;

namespace Consolonia.Gallery.Tests
{
    [TestFixture]
    internal class SliderTests : GalleryTestsBaseBase
    {
        [Test]
        public async Task PerformSingleTest()
        {
            await UITest.KeyInput(Key.Tab);
            await UITest.AssertHasText(" 🠷─────");
            await UITest.KeyInput(Key.Right);
            await UITest.KeyInput(Key.Right);
            await UITest.AssertHasText(" ─🠷────");
        }
    }
}