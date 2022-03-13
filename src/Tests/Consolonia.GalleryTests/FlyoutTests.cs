using System.Threading.Tasks;
using Avalonia.Input;
using Consolonia.GalleryTests.Base;
using Consolonia.TestsCore;

namespace Consolonia.GalleryTests
{
    internal class FlyoutTests : GalleryTestsBaseBase
    {
        protected override async Task PerformSingleTest()
        {
            await UITest.KeyInput(Key.Enter);
            await UITest.AssertHasText(@"Item 1      \>",
                @"Item 2");
            await UITest.KeyInput(Key.Right);
            await UITest.AssertHasText(@"Subitem 1",
                @"Item 3");
            await UITest.KeyInput(Key.Left);
            await UITest.AssertHasNoText("Subitem 1");
            await UITest.KeyInput(Key.Escape);
            await UITest.AssertHasNoText("Item 1");
            await UITest.KeyInput(Key.Tab, Key.Enter);
            await UITest.AssertHasText(@"Flyout Content!");
            await UITest.KeyInput(Key.Escape);
            await UITest.AssertHasNoText(@"Flyout Content!");
        }
    }
}