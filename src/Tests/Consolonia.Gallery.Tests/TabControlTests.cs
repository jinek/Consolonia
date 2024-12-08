using System.Threading.Tasks;
using Avalonia.Input;
using Consolonia.Gallery.Tests.Base;
using Consolonia.NUnit;
using NUnit.Framework;

namespace Consolonia.Gallery.Tests
{
    [TestFixture]
    internal class TabControlTests : GalleryTestsBaseBase
    {
        [Test]
        public async Task PerformSingleTest()
        {
            await UITest.KeyInput(Key.Tab);
            await UITest.AssertHasText("Arch", "Leaf");

            await UITest.KeyInput(Key.Right);
            await UITest.AssertHasText("Arch", "Leaf", "This is the second page");
            await UITest.KeyInput(Key.Tab, Key.Tab, Key.Down);
            await UITest.AssertHasText("Arch", "Leaf", "This is the sec");
        }
    }
}