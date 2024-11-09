using System.Threading.Tasks;
using Avalonia.Input;
using Consolonia.GalleryTests.Base;
using Consolonia.TestsCore;
using NUnit.Framework;

namespace Consolonia.GalleryTests
{
    [TestFixture]
    internal class DialogTests : GalleryTestsBaseBase
    {
        protected override async Task PerformSingleTest()
        {
            await UITest.KeyInput(Key.Enter);
            await UITest.KeyInput(Key.Tab);
            await UITest.AssertHasText("One More");
            await UITest.KeyInput(Key.Escape);
            await UITest.AssertHasNoText("One More");
        }
    }
}