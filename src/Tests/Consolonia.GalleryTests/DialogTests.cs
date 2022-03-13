using System.Threading.Tasks;
using Avalonia.Input;
using Consolonia.GalleryTests.Base;
using Consolonia.TestsCore;

namespace Consolonia.GalleryTests
{
    internal class DialogTests : GalleryTestsBase
    {
        protected override async Task PerformSingleTest()
        {
            await UITest.KeyInput(Key.Enter);
            await UITest.KeyInput(Key.Tab);
            await UITest.AssertHasText("One More");
            await UITest.KeyInput(Key.Escape);
            await UITest.AssertHasNoText("One More");
        }
    }
}