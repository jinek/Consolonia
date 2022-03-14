using System.Threading.Tasks;
using Avalonia.Input;
using Consolonia.GalleryTests.Base;
using Consolonia.TestsCore;
using NUnit.Framework;

namespace Consolonia.GalleryTests
{
    [TestFixture]
    internal class TextBoxTests : GalleryTestsBaseBase
    {
        protected override async Task PerformSingleTest()
        {
            await UITest.AssertHasText(@"elit\. ");
            await UITest.KeyInput(Key.Right);
            await UITest.StringInput("asd");
            await UITest.KeyInput(3, Key.Left, RawInputModifiers.Control);
            await UITest.AssertHasText(@"t, consectetur adipiscing elit\.asd");
            await UITest.KeyInput(Key.Home);
            await UITest.KeyInput(7, Key.Right, RawInputModifiers.Control);
            await UITest.AssertHasText(@"ipsum dolor sit amet, consectetur a");

            //readonly
            await UITest.KeyInput(Key.Tab);
            await UITest.StringInput("asdf");
            await UITest.AssertHasText(@"This is read only");
            await UITest.AssertHasText(@"ReadOnly watermark");
            await UITest.AssertHasText(@"This is disabled");
            await UITest.AssertHasText(@"This is disabled watermark");

            await UITest.KeyInput(Key.Tab);
            await UITest.KeyInput(Key.Tab);
            await UITest.KeyInput(Key.Delete);
            await UITest.AssertHasText(@"Floating Watermark");
            await UITest.StringInput("asd54321");
            await UITest.AssertHasText(@"Floating Watermark");
            await UITest.AssertHasText(@"asd54321");
        }
    }
}