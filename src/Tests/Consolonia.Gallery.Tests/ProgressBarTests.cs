using System.Threading.Tasks;
using Consolonia.Gallery.Tests.Base;
using Consolonia.NUnit;
using NUnit.Framework;

namespace Consolonia.Gallery.Tests
{
    [TestFixture]
    [Ignore(
        "ProgressBar has annimation, thus application becomes never idle, thus hard to determine input, layout and other jobs are done")] //todo:
    internal class ProgressBarTests : GalleryTestsBaseBase
    {
        [Test]
        public async Task PerformSingleTest()
        {
            await UITest.AssertHasText("5%", "50%");
        }
    }
}