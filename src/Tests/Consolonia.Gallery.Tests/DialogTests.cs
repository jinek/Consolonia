using System.Threading.Tasks;
using Avalonia.Input;
using Consolonia.Gallery.Gallery.GalleryViews;
using Consolonia.Gallery.Tests.Base;
using Consolonia.NUnit;
using NUnit.Framework;

namespace Consolonia.Gallery.Tests
{
    [TestFixture]
    internal class DialogTests : GalleryTestsBaseBase
    {
        [Test]
        public async Task PerformSingleTest()
        {
            await UITest.KeyInput(Key.Tab);
            await UITest.KeyInput(Key.Enter);
            await UITest.KeyInput(Key.Tab);
            await UITest.AssertHasText("One More");
            await UITest.AssertHasText(SomeDialogWindow.DialogTitle);
            await UITest.KeyInput(Key.Escape);
            await UITest.AssertHasNoText("One More");
        }
    }
}