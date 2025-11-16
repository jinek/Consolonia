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
        /// Assert text is present in the console buffer.
        /// Patterns starting and ending with '/' are treated as regex (e.g., /pattern/).
        /// </summary>
        /// <param name="unitTestConsole"></param>
        /// <param name="patterns">Patterns to search for. Use /pattern/ syntax for regex.</param>
        /// <returns></returns>
        public static async Task AssertHasText(this UnitTestConsole unitTestConsole, params string[] patterns)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                string printBuffer = unitTestConsole.PixelBuffer.PrintBuffer();

                foreach (string pattern in patterns)
                {
                    if (IsRegexPattern(pattern))
                    {
                        var regex = new Regex(StripRegexDelimiters(pattern));
                        bool found = regex.IsMatch(printBuffer);
                        Assert.IsTrue(found,
                            $"Regex '{pattern}' was not found at the buffer: \r\n" + printBuffer);
                    }
                    else
                    {
                        Assert.IsTrue(printBuffer.Contains(pattern, StringComparison.Ordinal), 
                            $"'{pattern}' was not found at the buffer: \r\n" + printBuffer);
                    }
                }
            });
        }


        /// <summary>
        /// Assert text poattern is NOT present in the console buffer.
        /// Patterns starting and ending with '/' are treated as regex (e.g., /pattern/).
        /// </summary>
        /// <param name="unitTestConsole"></param>
        /// <param name="patterns">Patterns to search for. Use /pattern/ syntax for regex.</param>
        /// <returns></returns>
        public static async Task AssertHasNoText(this UnitTestConsole unitTestConsole, params string[] patterns)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                string printBuffer = unitTestConsole.PixelBuffer.PrintBuffer();

                foreach (string pattern in patterns)
                {
                    if (IsRegexPattern(pattern))
                    {
                        var regex = new Regex(StripRegexDelimiters(pattern));
                        bool found = regex.IsMatch(printBuffer);
                        Assert.IsFalse(found,
                            $"Regex '{pattern}' was found at the buffer: \r\n" + printBuffer);
                    }
                    else
                    {
                        Assert.IsFalse(printBuffer.Contains(pattern, StringComparison.Ordinal),
                            $"'{pattern}' was found at the buffer: \r\n" + printBuffer);
                    }
                }
            });
        }

        /// <summary>
        /// Determines if a pattern is a regex based on /pattern/ delimiter syntax.
        /// </summary>
        /// <param name="pattern">Pattern to check</param>
        /// <returns>True if pattern starts and ends with '/' (and is not '//')</returns>
        private static bool IsRegexPattern(string pattern)
        {
            return pattern.Length >= 3 &&
                   pattern.StartsWith('/') &&
                   pattern.EndsWith('/') &&
                   !pattern.StartsWith("//", StringComparison.Ordinal) &&
                   !pattern.EndsWith("//", StringComparison.Ordinal);
        }

        /// <summary>
        /// Strips the /pattern/ delimiters from a regex pattern.
        /// </summary>
        private static string StripRegexDelimiters(string pattern)
        {
            return pattern[1..^1];
        }

    }
}