using System.Threading.Tasks;
using Avalonia.Input;
using Consolonia.Gallery.Tests.Base;
using Consolonia.NUnit;
using NUnit.Framework;

namespace Consolonia.Gallery.Tests
{
    [TestFixture]
    internal class NotificationsTests : GalleryTestsBaseBase
    {
        [Test]
        public async Task PerformSingleTest()
        {
            await UITest.AssertHasNoText("It's");
            await UITest.KeyInput(Key.Tab);
            await UITest.KeyInput(Key.Enter);
            await Task.Delay(500);
            await UITest.AssertHasText("It's");
            await Task.Delay(6000);
            await UITest.AssertHasNoText("It's");
        }
    }
}