using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia.Threading;
using NUnit.Framework;

namespace Consolonia.NUnit
{
    public static class TestHelpers
    {
        /// <summary>
        ///     Assert text pattern(s) is present in the console buffer.
        ///     Patterns starting and ending with '/' are treated as regex (e.g., /pattern/).
        /// </summary>
        /// <param name="unitTestConsole"></param>
        /// <param name="patterns">Patterns to search for. Use /pattern/ syntax for regex.</param>
        /// <returns></returns>
        public static async Task AssertHasText(this UnitTestConsole unitTestConsole, params string[] patterns)
        {
            await AssertPatterns(unitTestConsole, patterns, true,
                (printBuffer, pattern) => $"Regex '{pattern}' was not found in the buffer: \r\n" + printBuffer);
        }

        /// <summary>
        ///     Assert text pattern(s) is NOT present in the console buffer.
        ///     Patterns starting and ending with '/' are treated as regex (e.g., /pattern/).
        /// </summary>
        /// <param name="unitTestConsole"></param>
        /// <param name="patterns">Patterns to search for. Use /pattern/ syntax for regex.</param>
        /// <returns></returns>
        public static async Task AssertHasNoText(this UnitTestConsole unitTestConsole, params string[] patterns)
        {
            await AssertPatterns(unitTestConsole, patterns, false,
                (printBuffer, pattern) => $"Regex '{pattern}' was found in the buffer: \r\n" + printBuffer);
        }

        private static async Task AssertPatterns(UnitTestConsole unitTestConsole, string[] patterns, bool shouldMatch,
            Func<string, string, string> onError)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                string printBuffer = unitTestConsole.PixelBuffer.PrintBuffer();
                foreach (string pattern in patterns)
                    if (shouldMatch)
                        Assert.IsTrue(IsMatch(printBuffer, pattern), onError(printBuffer, pattern));
                    else
                        Assert.IsFalse(IsMatch(printBuffer, pattern), onError(printBuffer, pattern));
            });
        }

        private static bool IsMatch(string printBuffer, string pattern)
        {
            if (IsRegexPattern(pattern))
            {
                var regex = new Regex(StripRegexDelimiters(pattern));
                return regex.IsMatch(printBuffer);
            }

            return printBuffer.Contains(pattern, StringComparison.Ordinal);
        }


        /// <summary>
        ///     Determines if a pattern is a regex based on /pattern/ delimiter syntax.
        /// </summary>
        /// <param name="pattern">Pattern to check</param>
        /// <returns>True if pattern starts and ends with '/' (and is not '//')</returns>
        private static bool IsRegexPattern(string pattern)
        {
            return pattern.Length >= 3 &&
                   pattern.StartsWith('/') &&
                   pattern.EndsWith('/');
        }

        /// <summary>
        ///     Strips the /pattern/ delimiters from a regex pattern.
        /// </summary>
        private static string StripRegexDelimiters(string pattern)
        {
            return pattern[1..^1];
        }
    }
}