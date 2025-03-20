using System.Threading.Tasks;
using Avalonia.Input;
using Consolonia.Gallery.Tests.Base;
using Consolonia.NUnit;
using NUnit.Framework;

namespace Consolonia.Gallery.Tests
{
    [TestFixture]
    internal class TooltipTests : GalleryTestsBaseBase
    {
        [Test]
        public async Task PerformSingleTest()
        {
            await UITest.KeyInput(Key.Tab);

            await UITest.AssertHasNoText("A control which pops up a hint");
            await UITest.KeyInput(Key.Space);
            await UITest.AssertHasText("A control which pops up a hint");
        }
    }
}