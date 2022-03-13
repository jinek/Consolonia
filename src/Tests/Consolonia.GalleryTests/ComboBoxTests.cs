using System.Threading.Tasks;
using Avalonia.Input;
using Consolonia.GalleryTests.Base;
using Consolonia.TestsCore;

namespace Consolonia.GalleryTests
{
    internal class ComboBoxTests : GalleryTestsBase
    {
        protected override async Task PerformSingleTest()
        {
            await UITest.AssertHasText("Pick an Item",
                "Still item",
                "Null object");

            await UITest.KeyInput(Key.Enter, Key.Down, Key.Down, Key.Enter);
            await UITest.AssertHasText("Inline Item 2");

            await UITest.KeyInput(Key.Enter, Key.Down, Key.Escape);
            await UITest.AssertHasText("Inline Item 2");
            await UITest.KeyInput(Key.Enter, Key.Down, Key.Enter);
            await UITest.AssertHasText("Inline Item 3");
            await UITest.KeyInput(Key.Down);
            await UITest.AssertHasText("Inline Item 4");
            await UITest.KeyInput(Key.Down);
            await UITest.AssertHasText("Inline Items");
            await UITest.KeyInput(Key.Tab, Key.Down);
            await UITest.AssertHasText("Hello");
        }
    }
}