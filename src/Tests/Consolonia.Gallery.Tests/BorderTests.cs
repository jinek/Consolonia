using System.Threading.Tasks;
using Avalonia.Input;
using Consolonia.Gallery.Tests.Base;
using Consolonia.NUnit;
using NUnit.Framework;

namespace Consolonia.Gallery.Tests
{
    [TestFixture]
    internal class BordersTests : GalleryTestsBaseBase
    {
        [Test]
        public async Task PerformSingleTest()
        {
            await UITest.KeyInput(Key.Tab);
            await UITest.AssertHasText("┌───────┐┌────────────────────┐╔════════════════════╗",
                "│Default││LineStyle=SingleLine│║LineStyle=DoubleLine║",
                "└───────┘└────────────────────┘╚════════════════════╝",
                " ▁▁▁▁▁▁▁▁▁▁▁▁▁▁ ▗▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▖████████████████ ",
                "▕LineStyle=Edge▏▐LineStyle=EdgeWide▌█LineStyle=Bold█ ",
                " ▔▔▔▔▔▔▔▔▔▔▔▔▔▔ ▝▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▘████████████████ ");
        }
    }
}