using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia.Threading;
using NUnit.Framework;

namespace Consolonia.NUnit
{
    public static class TestHelpers
    {
        public static async Task AssertHasRawText(this UnitTestConsole unitTestConsole, params string[] textToSearch)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                var printBuffer = unitTestConsole.PixelBuffer.PrintBuffer();

                foreach (string text in textToSearch)
                {
                    Assert.IsTrue(printBuffer.Contains(text), $"{text} not at the buffer: \r\n" + printBuffer);
                }
            }, DispatcherPriority.Render);
        }

        public static async Task AssertHasText(this UnitTestConsole unitTestConsole, params string[] regexesToSearch)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                var printBuffer = unitTestConsole.PixelBuffer.PrintBuffer();

                foreach (var regexToSearch in regexesToSearch)
                {
                    var regex = new Regex(regexToSearch);
                    var found = regex.IsMatch(printBuffer);
                    Assert.IsTrue(found,
                        $"'{regexToSearch}' was not found at the buffer: \r\n" + printBuffer);
                }
            }, DispatcherPriority.Render);
        }

        public static async Task AssertHasText(this UnitTestConsole unitTestConsole, string regexToSearch)
        {
            (bool found, string bufferText) = await unitTestConsole.HasText(regexToSearch).ConfigureAwait(true);

            Assert.IsTrue(found,
                $"'{regexToSearch}' was not found at the buffer: \r\n" + bufferText);
        }

        public static async Task AssertHasNoText(this UnitTestConsole unitTestConsole, string regexToSearch)
        {
            (bool found, string bufferText) = await unitTestConsole.HasText(regexToSearch).ConfigureAwait(true);

            Assert.IsFalse(found,
                $"'{regexToSearch}' was found at the buffer: \r\n" + bufferText);
        }

        private static async Task<(bool, string)> HasText(this UnitTestConsole unitTestConsole, string regexToSearch)
        {
            bool found = false;
            string printBuffer = null;

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                printBuffer = unitTestConsole.PixelBuffer.PrintBuffer();

                var regex = new Regex(regexToSearch);
                found = regex.IsMatch(printBuffer);
            }, DispatcherPriority.Render).GetTask().ConfigureAwait(true);


            return (found, printBuffer);
        }
    }
}