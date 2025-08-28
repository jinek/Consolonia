using System.Threading.Tasks;
using Avalonia.Input;
using Consolonia.Gallery.Tests.Base;
using Consolonia.NUnit;
using NUnit.Framework;

namespace Consolonia.Gallery.Tests
{
    [TestFixture]
    internal class CarouselTests : GalleryTestsBaseBase
    {
        [Test]
        public async Task PerformSingleTest()
        {
            await UITest.KeyInput(Key.Tab);
            await UITest.AssertHasText("Lorem ipsum dolor sit");
            await UITest.AssertHasNoText("Duis aute irure");
            await UITest.KeyInput(Key.Tab);
            await UITest.KeyInput(Key.Tab);
            await UITest.KeyInput(Key.Tab);
            await UITest.KeyInput(Key.Space);
            await Task.Delay(200); // Wait for the animation to finish
            await UITest.AssertHasNoText("Lorem ipsum dolar sit");
            await UITest.AssertHasText("Duis aute irure");
        }
    }
}