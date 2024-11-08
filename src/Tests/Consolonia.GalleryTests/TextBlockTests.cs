using System.Threading.Tasks;
using Consolonia.GalleryTests.Base;
using Consolonia.TestsCore;
using NUnit.Framework;

namespace Consolonia.GalleryTests
{
    /// <summary>
    ///     Unit test for TextBlock view
    /// </summary>
    [TestFixture]
    internal class TextBlockTests : GalleryTestsBaseBase
    {
        protected override Task PerformSingleTest()
        {
            return UITest.AssertHasText("This is TextBlock",
                "Text trimming with charac...",
                "Text trimming with word...",
                "│Left aligned text    ",
                "   Center aligned text    ",
                "Right aligned text│",

                // multiline
                "│Vivamus magna. Cras in mi at felis aliquet congue. Ut a │",
                "│est eget ligula molestie gravida. Curabitur massa. Donec│",
                // special chars, emojis, etc.
                "𐓏𐓘𐓻𐓘𐓻𐓟 𐒻𐓟", "𝄞", "🎵", "“𝔉𝔞𝔫𝔠𝔶”", "ﬀ", "ﬁ", "½");
        }
    }
}