using System.Threading.Tasks;
using Avalonia.Input;
using Consolonia.Gallery.Tests.Base;
using Consolonia.NUnit;
using NUnit.Framework;

namespace Consolonia.Gallery.Tests
{
    [TestFixture]
    internal class TextBlockTests : GalleryTestsBaseBase
    {
        [Test]
        public async Task PerformSingleTest()
        {
            await UITest.KeyInput(Key.Tab);
            await UITest.AssertHasText("This is TextBlock",
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