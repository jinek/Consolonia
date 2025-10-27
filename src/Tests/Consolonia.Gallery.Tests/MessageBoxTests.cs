using System.Threading.Tasks;
using Avalonia.Input;
using Consolonia.Gallery.Tests.Base;
using Consolonia.NUnit;
using NUnit.Framework;

namespace Consolonia.Gallery.Tests
{
    [TestFixture]
    internal class MessageBoxTests : GalleryTestsBaseBase
    {
        [Test]
        public async Task PerformSingleTest()
        {
            await UITest.KeyInput(Key.Tab);
            await UITest.KeyInput(Key.Enter); // show messagebox
            //await Task.Delay(100); // animation
            await UITest.AssertHasText("OK/Cancel Message box", "Do you want to");
            await UITest.AssertHasText("🗕", "🗖", "🗙");
            await UITest.AssertHasNoText("🗗");
            await UITest.KeyInput(Key.Escape);
            //await Task.Delay(100);  // animation
            foreach (string x in new[] { "OK/Cancel Message box", "Do you want to", "🗕", "🗖", "🗙" })
                await UITest.AssertHasNoText(x);
        }
    }
}