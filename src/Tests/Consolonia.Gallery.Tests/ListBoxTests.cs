using System.Threading.Tasks;
using Avalonia.Input;
using Consolonia.Gallery.Tests.Base;
using Consolonia.NUnit;
using NUnit.Framework;

namespace Consolonia.Gallery.Tests
{
    [TestFixture]
    internal class ListBoxTests : GalleryTestsBaseBase
    {
        [Test]
        public async Task PerformSingleTest()
        {
            await UITest.KeyInput(Key.Tab);
            await UITest.KeyInput(Key.Tab, RawInputModifiers.Shift);
            await UITest.KeyInput(Key.Tab, RawInputModifiers.Shift);
            await UITest.KeyInput(Key.PageDown);
            await UITest.AssertHasText("Item 49");
        }
    }
}