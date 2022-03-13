using System.Threading.Tasks;
using Avalonia.Input;
using Consolonia.GalleryTests.Base;
using Consolonia.TestsCore;

namespace Consolonia.GalleryTests
{
    internal class TabControlTests : GalleryTestsBaseBase
    {
        protected override async Task PerformSingleTest()
        {
            await UITest.AssertHasText("Arch", "Leaf");

            await UITest.KeyInput(Key.Right);
            await UITest.AssertHasText("Arch", "Leaf", "This is the second page");
            await UITest.KeyInput(Key.Tab, Key.Tab, Key.Down);
            await UITest.AssertHasText("Arch", "Leaf", "This is the sec");
        }
    }
}