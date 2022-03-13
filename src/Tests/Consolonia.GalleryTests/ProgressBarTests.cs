using System.Threading.Tasks;
using Consolonia.GalleryTests.Base;
using Consolonia.TestsCore;
using NUnit.Framework;

namespace Consolonia.GalleryTests
{
    [Ignore(
        "ProgressBar has annimation, thus application becomes never idle, thus hard to determine input, layout and other jobs are done")] //todo:
    internal class ProgressBarTests : GalleryTestsBaseBase
    {
        protected override async Task PerformSingleTest()
        {
            await UITest.AssertHasText(@"5%", @"50%");
        }
    }
}