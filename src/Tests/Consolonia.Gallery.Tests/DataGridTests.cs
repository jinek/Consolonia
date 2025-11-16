using System.Threading.Tasks;
using Avalonia.Input;
using Consolonia.Gallery.Tests.Base;
using Consolonia.NUnit;
using NUnit.Framework;

namespace Consolonia.Gallery.Tests
{
    [TestFixture]
    internal class DataGridTests : GalleryTestsBaseBase
    {
        [Test]
        public async Task PerformSingleTest()
        {
            await UITest.AssertHasMatch("Co.+Region.+Population.+Area",
                @"Afg.+ASIA \(EX. NEAR");
            await UITest.KeyInput(Key.Tab, Key.Tab);
            await UITest.KeyInput(50 /*todo: Magic number of pagedown*/, Key.PageDown);
            await UITest.AssertHasMatch("Co.+Region.+Population.+Area",
                "Yem",
                "21456188");
        }
    }
}