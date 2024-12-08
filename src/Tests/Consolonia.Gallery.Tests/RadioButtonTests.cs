using System.Threading.Tasks;
using Avalonia.Input;
using Consolonia.Gallery.Tests.Base;
using Consolonia.NUnit;
using NUnit.Framework;

namespace Consolonia.Gallery.Tests
{
    [TestFixture]
    internal class RadioButtonTests : GalleryTestsBaseBase
    {
        [Test]
        public async Task PerformSingleTest()
        {
            await UITest.KeyInput(Key.Tab);
            await UITest.AssertHasText(@"\(\*\).+Option 1",
                @"\( \).+Option 2",
                @"\(■\).+Option 3");

            await UITest.KeyInput(Key.Tab, Key.Space);
            await UITest.AssertHasText(@"\( \).+Option 1",
                @"\(\*\).+Option 2",
                @"\(■\).+Option 3");

            await UITest.KeyInput(Key.Tab);
            await UITest.KeyInput(Key.Enter); //todo: check why does not react to Space
            await UITest.AssertHasText(@"\( \).+Option 1",
                @"\( \).+Option 2",
                @"\(\*\).+Option 3");
        }
    }
}