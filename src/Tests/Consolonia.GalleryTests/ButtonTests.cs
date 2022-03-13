using System.Threading.Tasks;
using Avalonia.Input;
using Consolonia.GalleryTests.Base;
using Consolonia.TestsCore;
using NUnit.Framework;

namespace Consolonia.GalleryTests
{
    [TestFixture]
    internal class ButtonTests : GalleryTestsBase
    {
        protected override async Task PerformSingleTest()
        {
            await UITest.KeyInput(Key.Enter);
            await UITest.AssertHasText("Standard _?XAML Button",
                "Foreground",
                "Disabled",
                "Toggle Button",
                "IsTabStop=False");
        }
    }
}