using System.Threading.Tasks;
using Avalonia.Input;
using Consolonia.Gallery.Tests.Base;
using Consolonia.NUnit;
using NUnit.Framework;

namespace Consolonia.Gallery.Tests
{
    [TestFixture]
    internal class ScrollViewerTests : GalleryTestsBaseBase
    {
        [Test]
        public async Task PerformSingleTest()
        {
            await UITest.KeyInput(Key.Tab);
            await UITest.KeyInput(Key.Tab, Key.Tab, Key.Tab);
            await UITest.KeyInput(30, Key.Right);

            // Checking cow eyes visible
            await UITest.AssertHasText(@"\(o\)  \(o  \}");
        }
    }
}