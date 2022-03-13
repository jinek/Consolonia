using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia.Threading;
using NUnit.Framework;

namespace Consolonia.TestsCore
{
    public static class Helpers
    {
        public static async Task AssertHasText(this UnitTestConsole unitTestConsole, params string[] regexesToSearch)
        {
            foreach (string regexString in regexesToSearch) await unitTestConsole.AssertHasText(new[] { regexString });
        }

        public static async Task AssertHasText(this UnitTestConsole unitTestConsole, string regexToSearch)
        {
            (bool found, string bufferText) = await unitTestConsole.HasText(regexToSearch);

            Assert.IsTrue(found,
                $"'{regexToSearch}' was not found at the buffer: \r\n" + bufferText);
        }

        public static async Task AssertHasNoText(this UnitTestConsole unitTestConsole, string regexToSearch)
        {
            (bool found, string bufferText) = await unitTestConsole.HasText(regexToSearch);

            Assert.IsFalse(found,
                $"'{regexToSearch}' was found at the buffer: \r\n" + bufferText);
        }

        public static async Task<(bool, string)> HasText(this UnitTestConsole unitTestConsole, string regexToSearch)
        {
            bool found = false;
            string printBuffer = null;

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                int searchPosition = 0;
                ushort lastY = 0;

                printBuffer = unitTestConsole.PrintBuffer();

                var regex = new Regex(regexToSearch);
                found = regex.IsMatch(printBuffer);
            });


            return (found, printBuffer);
        }
    }
}