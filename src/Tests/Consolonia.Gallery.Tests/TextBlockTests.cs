using System.Threading.Tasks;
using Avalonia.Input;
using Consolonia.Gallery.Tests.Base;
using Consolonia.NUnit;
using NUnit.Framework;

namespace Consolonia.Gallery.Tests
{
    /// <summary>
    ///     Unit test for TextBlock view
    /// </summary>
    [TestFixture]
    internal class TextBlockTests : GalleryTestsBaseBase
    {
        [Test]
        public async Task DisplaysBasicText()
        {
            await UITest.KeyInput(Key.Tab);
            await UITest.AssertHasText("This is TextBlock");
        }

        [Test]
        public async Task HandlesTrimming()
        {
            await UITest.KeyInput(Key.Tab);
            await UITest.AssertHasText(
                "Text trimming with charac...",
                "Text trimming with word...");
        }

        [Test]
        public async Task HandlesAlignment()
        {
            await UITest.KeyInput(Key.Tab);
            await UITest.AssertHasText(
                "│Left aligned text    ",
                "   Center aligned text    ",
                "        Right aligned text│");
        }

        [Test]
        public async Task HandlesMultilineText()
        {
            await UITest.KeyInput(Key.Tab);
            await UITest.AssertHasText(
                "│Vivamus magna. Cras in mi at felis aliquet congue. Ut a │",
                "│est eget ligula molestie gravida. Curabitur massa. Donec│");
        }

        [Test]
        public async Task HandlesSpecialCharacters()
        {
            await UITest.KeyInput(Key.Tab);
            await UITest.AssertHasText(
                "𐓏𐓘𐓻𐓘𐓻𐓟 𐒻𐓟", "𝄞", "🎵", "𝔉𝔞𝔫𝔠𝔶", "ﬀ", "ﬁ", "½");
        }
    }
}