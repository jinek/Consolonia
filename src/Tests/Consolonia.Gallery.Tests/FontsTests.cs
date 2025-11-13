using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using Consolonia.Gallery.Gallery.GalleryViews;
using Consolonia.Gallery.Tests.Base;
using Consolonia.NUnit;
using NUnit.Framework;

namespace Consolonia.Gallery.Tests
{
    /// <summary>
    ///     Unit test for TextBlock view
    /// </summary>
    [TestFixture]
    internal class FontsTests : GalleryTestsBaseBase
    {
        [Test]
        public async Task DisplaysBasicText()
        {
            await SelectFont("ConsoleDefault");
            await UITest.AssertHasText("Hello World!");
        }

        [Test]
        public async Task DisplaysWideTermText()
        {
            await SelectFont("WideTerm");
            await UITest.AssertHasRawText("Ｈ️ｅ️ｌ️ｌ️ｏ️  Ｗ️ｏ️ｒ️ｌ️ｄ️！️");
        }

        [Test]
        public async Task DisplaysBrailleText()
        {
            await SelectFont("Braille");

            await UITest.AssertHasRawText(
"⣇⣸ ⢀⡀ ⡇ ⡇ ⢀⡀   ⡇⢸ ⢀⡀ ⡀⣀ ⡇ ⢀⣸ ⡇",
"⠇⠸ ⠣⠭ ⠣ ⠣ ⠣⠜   ⠟⠻ ⠣⠜ ⠏  ⠣ ⠣⠼ ⠅");
        }

        [Test]
        public async Task DisplaysCacaFontText()
        {
            await SelectFont("Doom");
            await UITest.AssertHasRawText(
@" _   _       _ _         _    _             _     _ ",
@"| | | |     | | |       | |  | |           | |   | |",
@"| |_| | ___ | | | ___   | |  | |  ___  _ __| | __| |",
@"|  _  |/ _ \| | |/ _ \  | |/\| | / _ \| '__| |/ _` |",
@"| | | |  __/| | | (_) | \  /\  /| (_) | |  | | (_| |",
@"\_| |_/\___||_|_|\___/   \/  \/  \___/|_|  |_|\__,_|"
                );
        }

        private static async Task SelectFont(string font)
        {
            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                var combo = await UITest.GetControl<ComboBox>("Fonts");
                FontsViewModel vm = (FontsViewModel)combo.DataContext!;
                vm.SelectedFont = vm.Fonts.FirstOrDefault(f => f.Font == font);
            });
            await Task.Delay(50);
        }
    }
}