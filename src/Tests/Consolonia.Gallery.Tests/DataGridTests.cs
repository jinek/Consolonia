using System.Threading.Tasks;
using Avalonia.Input;
using Consolonia.GalleryTests.Base;
using Consolonia.TestsCore;
using NUnit.Framework;

namespace Consolonia.GalleryTests
{
    [TestFixture]
    internal class DataGridTests : GalleryTestsBaseBase
    {
        protected override async Task PerformSingleTest()
        {
            await UITest.AssertHasText("Co.+Region.+Population.+Area",
                @"Afg.+ASIA \(EX. NEAR");
            await UITest.KeyInput(Key.Tab, Key.Tab);
            await UITest.KeyInput(50 /*todo: Magic number of pagedown*/, Key.PageDown);
            await UITest.AssertHasText("Co.+Region.+Population.+Area",
                "Yem",
                "21456188");
        }
    }
}