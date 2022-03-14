using System.Threading.Tasks;
using Avalonia.Input;
using Consolonia.GalleryTests.Base;
using Consolonia.TestsCore;
using NUnit.Framework;

namespace Consolonia.GalleryTests
{
    [TestFixture]
    internal class ListBoxTests : GalleryTestsBaseBase
    {
        protected override async Task PerformSingleTest()
        {
            await UITest.KeyInput(Key.Tab, RawInputModifiers.Shift);
            await UITest.KeyInput(Key.Tab, RawInputModifiers.Shift);
            await UITest.KeyInput(Key.PageDown);
            await UITest.AssertHasText("Item 53");
        }
    }
}