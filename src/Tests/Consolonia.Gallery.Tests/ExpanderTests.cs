using System.Threading.Tasks;
using Avalonia.Input;
using Consolonia.Gallery.Tests.Base;
using Consolonia.NUnit;
using NUnit.Framework;

namespace Consolonia.Gallery.Tests
{
    [TestFixture]
    internal class ExpanderTests : GalleryTestsBaseBase
    {
        [Test]
        public async Task PerformSingleTest()
        {
            await UITest.KeyInput(Key.Tab);

            await UITest.AssertHasText(" Up ▴ ", "Down ▾", "◂Left", "Right▸");
            await UITest.AssertHasNoText("Top content");
            await UITest.AssertHasNoText("Right content");
            await UITest.AssertHasNoText("Left content");
            await UITest.AssertHasNoText("Bottom content");
            
            // up test
            await UITest.KeyInput(Key.Space);
            await UITest.AssertHasText(" Up   ▾", "Down   ▾", "◂Left", "Right▸");
            await UITest.AssertHasText("Top content");

            // down test
            await UITest.KeyInput(Key.Tab);
            await UITest.KeyInput(Key.Space);
            await UITest.AssertHasText(" Up     ▾", "Down    ▴ ", "◂Left", "Right▸");
            await UITest.AssertHasText("Top content");
            await UITest.AssertHasText("Bottom content");

            // left test
            await UITest.KeyInput(Key.Tab);
            await UITest.KeyInput(Key.Space);
            await UITest.AssertHasText(" Up       ▾ ", "Down      ▴", "▸Left", "Right▸");
            await UITest.AssertHasText("Top content");
            await UITest.AssertHasText("Left content");
            await UITest.AssertHasText("Bottom content");

            // right test
            await UITest.KeyInput(Key.Tab);
            await UITest.KeyInput(Key.Space);
            await UITest.AssertHasText(" Up         ▾", "Down       ▴", "▸Left", "Right◂");
            await UITest.AssertHasText("Top content");
            await UITest.AssertHasText("Left content");
            await UITest.AssertHasText("Bottom content");
            await UITest.AssertHasText("Right content");

            // undo all test
            await UITest.KeyInput(Key.Tab);
            await UITest.KeyInput(Key.Tab);
            await UITest.KeyInput(Key.Space);
            await UITest.KeyInput(Key.Tab);
            await UITest.KeyInput(Key.Space);
            await UITest.KeyInput(Key.Tab);
            await UITest.KeyInput(Key.Space);
            await UITest.KeyInput(Key.Tab);
            await UITest.KeyInput(Key.Space);

            await UITest.AssertHasText(" Up  ▴ ", "Down ▾", "◂Left", "Right▸");
            await UITest.AssertHasNoText("Top content");
            await UITest.AssertHasNoText("Right content");
            await UITest.AssertHasNoText("Left content");
            await UITest.AssertHasNoText("Bottom content");

        }
    }
}