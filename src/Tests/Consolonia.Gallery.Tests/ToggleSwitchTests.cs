using System.Threading.Tasks;
using Avalonia.Input;
using Consolonia.Gallery.Tests.Base;
using Consolonia.NUnit;
using NUnit.Framework;

namespace Consolonia.Gallery.Tests
{
    [TestFixture]
    internal class ToggleSwitchTests : GalleryTestsBaseBase
    {
        [Test]
        public async Task PerformSingleTest()
        {
            await UITest.AssertHasText("│◯    Nope");
            await UITest.KeyInput(Key.Tab);
            await UITest.KeyInput(Key.Tab);
            await UITest.KeyInput(Key.Space);
            await Task.Delay(200); // Wait for the animation to finish
            await UITest.AssertHasText(@"/⬤.+Yep/");
            await UITest.KeyInput(Key.Enter);
            await Task.Delay(200); // Wait for the animation to finish
            await UITest.AssertHasText("◯    Nope");
        }
    }
}