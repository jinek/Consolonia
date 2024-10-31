using System.Threading.Tasks;
using Consolonia.GalleryTests.Base;
using Consolonia.TestsCore;
using NUnit.Framework;

namespace Consolonia.GalleryTests
{
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
                "│Lorem ipsum dolor sit amet, consectetur adipiscing elit.│",
                @"│Vivamus magna. Cras in mi at felis aliquet congue. Ut a │",
                @"│est eget ligula molestie gravida. Curabitur massa. Donec│");
        }
    }
}