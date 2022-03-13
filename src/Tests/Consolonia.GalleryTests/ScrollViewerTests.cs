using System.Threading.Tasks;
using Avalonia.Input;
using Consolonia.GalleryTests.Base;
using Consolonia.TestsCore;

namespace Consolonia.GalleryTests
{
    internal class ScrollViewerTests : GalleryTestsBaseBase
    {
        protected override async Task PerformSingleTest()
        {
            await UITest.KeyInput(Key.Tab, Key.Tab, Key.Tab);
            await UITest.KeyInput(30, Key.Right);

            // Checking cow eyes visible
            await UITest.AssertHasText(@"\(o\)  \(o  \}");
        }
    }
}