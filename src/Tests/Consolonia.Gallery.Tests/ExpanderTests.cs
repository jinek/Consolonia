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
        [Order(1)]
        public async Task TestStart()
        {
            await UITest.KeyInput(Key.Tab);
            await UITest.AssertHasText(" Up ˄ ", "Down ˅", "˂Left", "Right˃");
            await UITest.AssertHasNoText("Top content");
            await UITest.AssertHasNoText("Right content");
            await UITest.AssertHasNoText("Left content");
            await UITest.AssertHasNoText("Bottom content");
            await UITest.KeyInput(1, Key.Tab, RawInputModifiers.Shift);
        }

        [Test]
        [Order(2)]
        public async Task TestUp()
        {
            await UITest.KeyInput(Key.Tab);
            await UITest.AssertHasText("Up.*˄");
            await UITest.AssertHasNoText("Top content");

            await UITest.KeyInput(Key.Space);
            await UITest.AssertHasText("Up.*˅");
            await UITest.AssertHasText("Top content");

            await UITest.KeyInput(Key.Space);
            await UITest.AssertHasText("Up.*˄");
            await UITest.AssertHasNoText("Top content");
            await UITest.KeyInput(1, Key.Tab, RawInputModifiers.Shift);
        }

        [Test]
        [Order(3)]
        public async Task TestDown()
        {
            await UITest.KeyInput(Key.Tab);
            await UITest.KeyInput(Key.Tab);
            await UITest.AssertHasText("Down.*˅");
            await UITest.AssertHasNoText("Bottom content");

            await UITest.KeyInput(Key.Space);
            await UITest.AssertHasText("Down.*˄");
            await UITest.AssertHasText("Bottom content");

            await UITest.KeyInput(Key.Space);
            await UITest.AssertHasText("Down.*▾");
            await UITest.AssertHasNoText("Bottom content");
            await UITest.KeyInput(1, Key.Tab, RawInputModifiers.Shift);
            await UITest.KeyInput(1, Key.Tab, RawInputModifiers.Shift);
        }

        [Test]
        [Order(4)]
        public async Task TestLeft()
        {
            await UITest.KeyInput(Key.Tab);
            await UITest.KeyInput(Key.Tab);
            await UITest.KeyInput(Key.Tab);
            await UITest.AssertHasText("˂.*Left");
            await UITest.AssertHasNoText("Left content");

            await UITest.KeyInput(Key.Space);
            await UITest.AssertHasText("˃.*Left");
            await UITest.AssertHasText("Left content");

            await UITest.KeyInput(Key.Space);
            await UITest.AssertHasText("˂.*Left");
            await UITest.AssertHasNoText("Left content");
            await UITest.KeyInput(1, Key.Tab, RawInputModifiers.Shift);
            await UITest.KeyInput(1, Key.Tab, RawInputModifiers.Shift);
            await UITest.KeyInput(1, Key.Tab, RawInputModifiers.Shift);
        }

        [Test]
        [Order(5)]
        public async Task TestRight()
        {
            await UITest.KeyInput(Key.Tab);
            await UITest.KeyInput(Key.Tab);
            await UITest.KeyInput(Key.Tab);
            await UITest.KeyInput(Key.Tab);
            await UITest.AssertHasText("Right.*˃");
            await UITest.AssertHasNoText("Right content");

            await UITest.KeyInput(Key.Space);
            await UITest.AssertHasText("Right.*˂");
            await UITest.AssertHasText("Right content");

            await UITest.KeyInput(Key.Space);
            await UITest.AssertHasText("Right.*˃");
            await UITest.AssertHasNoText("Right content");
            await UITest.KeyInput(1, Key.Tab, RawInputModifiers.Shift);
            await UITest.KeyInput(1, Key.Tab, RawInputModifiers.Shift);
            await UITest.KeyInput(1, Key.Tab, RawInputModifiers.Shift);
            await UITest.KeyInput(1, Key.Tab, RawInputModifiers.Shift);
        }
    }
}