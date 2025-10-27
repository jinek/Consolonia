using System.Threading.Tasks;
using Avalonia.Input;
using Consolonia.Gallery.Tests.Base;
using Consolonia.NUnit;
using NUnit.Framework;

namespace Consolonia.Gallery.Tests
{
    [TestFixture]
    internal class MenuTests : GalleryTestsBaseBase
    {
        [Test]
        public async Task PerformSingleTest()
        {
            await UITest.AssertHasText("First", "Second");
            await UITest.KeyInput(Key.Enter);
            await UITest.AssertHasText("Standard Menu Item", @"Ctrl\+A");
            await UITest.KeyInput(Key.Down, Key.Right);
            await UITest.AssertHasText("Submenu 1");
            await UITest.KeyInput(Key.Escape);
            await UITest.AssertHasNoText("Submenu 1");
            await UITest.KeyInput(Key.Escape);
            await UITest.AssertHasNoText("Standard Menu Item");
        }
    }
}