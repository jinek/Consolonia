using System.Threading.Tasks;
using Avalonia.Input;
using Consolonia.Gallery.Tests.Base;
using Consolonia.NUnit;
using NUnit.Framework;

namespace Consolonia.Gallery.Tests
{
    [TestFixture]
    internal class ButtonTests : GalleryTestsBaseBase
    {
        [Test]
        public async Task PerformSingleTest()
        {
            await UITest.KeyInput(Key.Tab);
            await UITest.KeyInput(Key.Enter);
            await UITest.AssertHasText("Standard _?XAML Button",
                "Custom",
                "Disabled",
                "Toggle Button",
                "IsTabStop=False");
        }
    }
}