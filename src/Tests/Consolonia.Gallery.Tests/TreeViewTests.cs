using System.Threading.Tasks;
using Avalonia.Input;
using Consolonia.Gallery.Tests.Base;
using Consolonia.NUnit;
using NUnit.Framework;

namespace Consolonia.Gallery.Tests
{
    [TestFixture]
    internal class TreeViewTests : GalleryTestsBaseBase
    {
        [Test]
        public async Task PerformSingleTest()
        {
            await UITest.KeyInput(Key.Tab);
            await UITest.AssertHasText("│⏵ Item 0  ",
                "│⏵ Item 1  ",
                "│⏵ Item 2  ");
            await UITest.AssertHasNoText("│ ⏵ Item 0 0  ");
            await UITest.KeyInput(Key.Enter);
            await UITest.AssertHasText("│▾ Item 0   ",
                "│ ⏵ Item 0 0  ",
                "│ ⏵ Item 0 1  ",
                "│ ⏵ Item 0 2  ",
                "│⏵ Item 1   ",
                "│⏵ Item 2   ");
            await UITest.KeyInput(Key.Enter);
            await UITest.AssertHasText("│⏵ Item 0  ",
                "│⏵ Item 1   ",
                "│⏵ Item 2   ");
            await UITest.AssertHasNoText("│ ⏵ Item 0 0  ");
        }
    }
}