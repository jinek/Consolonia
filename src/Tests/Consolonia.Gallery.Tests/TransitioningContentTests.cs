using System.Threading.Tasks;
using Avalonia.Input;
using Consolonia.Gallery.Tests.Base;
using Consolonia.NUnit;
using NUnit.Framework;

namespace Consolonia.Gallery.Tests
{
    [TestFixture]
    internal class TransitioningContentTests : GalleryTestsBaseBase
    {
        [Test]
        public async Task PerformSingleTest()
        {
            await UITest.AssertHasText("Lorem ipsum");
            // await UITest.AssertHasNoText("Duis aute irure");
            await UITest.KeyInput(Key.Tab);
            await UITest.KeyInput(Key.Tab);
            await UITest.KeyInput(Key.Tab);
            await UITest.KeyInput(Key.Tab);
            await UITest.KeyInput(Key.Space);
            await Task.Delay(500); // Wait for the animation to finish
            await UITest.AssertHasNoText("Lorem ipsum dolar sit");
            await UITest.AssertHasText("Ut enim ad minima ");
        }
    }
}