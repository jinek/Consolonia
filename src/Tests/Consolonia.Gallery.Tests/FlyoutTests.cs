using System.Threading.Tasks;
using Avalonia.Input;
using Consolonia.Gallery.Tests.Base;
using Consolonia.NUnit;
using NUnit.Framework;

namespace Consolonia.Gallery.Tests
{
    [TestFixture]
    internal class FlyoutTests : GalleryTestsBaseBase
    {
        [Test]
        public async Task PerformSingleTest()
        {
            await UITest.KeyInput(Key.Enter);
            await UITest.AssertHasText(@"Item 1\s+⏵",
                "Item 2");
            await UITest.KeyInput(Key.Right);
            await UITest.AssertHasText("Subitem 1",
                "Item 1");
            await UITest.KeyInput(Key.Left);
            await UITest.AssertHasNoText("Subitem 1");
            await UITest.KeyInput(Key.Escape);
            await UITest.AssertHasNoText("Item 1");
            await UITest.KeyInput(Key.Tab, Key.Enter);
            await UITest.AssertHasText("Flyout Content!");
            await UITest.KeyInput(Key.Escape);
            await UITest.AssertHasNoText("Flyout Content!");
        }
    }
}