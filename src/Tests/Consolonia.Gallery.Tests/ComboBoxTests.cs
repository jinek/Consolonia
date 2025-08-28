using System.Threading.Tasks;
using Avalonia.Input;
using Consolonia.Gallery.Tests.Base;
using Consolonia.NUnit;
using NUnit.Framework;

namespace Consolonia.Gallery.Tests
{
    [TestFixture]
    internal class ComboBoxTests : GalleryTestsBaseBase
    {
        [Test]
        public async Task PerformSingleTest()
        {
            await UITest.KeyInput(Key.Tab);
            await UITest.AssertHasText("Pick an Item",
                "Still item",
                "Null object");

            await UITest.KeyInput(Key.Enter, Key.Down, Key.Down, Key.Enter);
            await UITest.AssertHasText("Inline Item 2");

            await UITest.KeyInput(Key.Enter, Key.Down, Key.Escape);
            await UITest.AssertHasText("Inline Item 2");
            await UITest.KeyInput(Key.Enter, Key.Down, Key.Enter);
            await UITest.AssertHasText("Inline Item 3");
            await UITest.KeyInput(Key.Down);
            await UITest.AssertHasText("Inline Item 4");
            await UITest.KeyInput(Key.Tab, Key.Down);
            await UITest.AssertHasText("Hello");
        }
    }
}