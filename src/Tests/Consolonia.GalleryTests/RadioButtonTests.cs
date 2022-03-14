using System.Threading.Tasks;
using Avalonia.Input;
using Consolonia.GalleryTests.Base;
using Consolonia.TestsCore;
using NUnit.Framework;

namespace Consolonia.GalleryTests
{
    [TestFixture]
    internal class RadioButtonTests : GalleryTestsBaseBase
    {
        protected override async Task PerformSingleTest()
        {
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