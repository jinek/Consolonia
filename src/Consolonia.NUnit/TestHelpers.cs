using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia.Threading;
using NUnit.Framework;

namespace Consolonia.NUnit
{
    public static class TestHelpers
    {
        /// <summary>
        /// Assert text pattern(s) are present in the console buffer.
        /// </summary>
        /// <param name="unitTestConsole"></param>
        /// <param name="patterns">Patterns to search for. </param>
        /// <returns></returns>
        public static async Task AssertHasText(this UnitTestConsole unitTestConsole, params string[] patterns)
        {
            await AssertPatterns(unitTestConsole, 
                patterns, 
                isRegex: false, 
                shouldMatch: true, 
                onError: (printBuffer, pattern) => $"Regex '{pattern}' was not found in the buffer: \r\n" + printBuffer);
        }

        /// <summary>
        /// Assert text pattern(s) are NOT present in the console buffer.
        /// </summary>
        /// <param name="unitTestConsole"></param>
        /// <param name="patterns">Patterns to search for.</param>
        /// <returns></returns>
        public static async Task AssertHasNoText(this UnitTestConsole unitTestConsole, params string[] patterns)
        {
            await AssertPatterns(unitTestConsole, 
                patterns, 
                isRegex: false, 
                shouldMatch: false, 
                onError: (printBuffer, pattern) => $"Regex '{pattern}' was found in the buffer: \r\n" + printBuffer);
        }

        /// <summary>
        /// Assert text pattern(s) as regular expressions are present in the console buffer.
        /// </summary>
        /// <param name="unitTestConsole"></param>
        /// <param name="regexPatterns">Regular expressions to search for.</param>
        /// <returns></returns>
        public static async Task AssertHasMatch(this UnitTestConsole unitTestConsole, [StringSyntax(StringSyntaxAttribute.Regex)] params string[] regexPatterns)
        {
            await AssertPatterns(unitTestConsole, 
                regexPatterns, 
                isRegex: true, 
                shouldMatch: true, 
                onError: (printBuffer, pattern) => $"Regex '{pattern}' was not found in the buffer: \r\n" + printBuffer);
        }

        /// <summary>
        /// Assert text pattern(s) as regular expressions are NOT present in the console buffer.
        /// </summary>
        /// <param name="unitTestConsole"></param>
        /// <param name="regexPatterns">Regular expressions to search for. </param>
        /// <returns></returns>
        public static async Task AssertHasNoMatch(this UnitTestConsole unitTestConsole, [StringSyntax(StringSyntaxAttribute.Regex)] params string[] regexPatterns)
        {
            await AssertPatterns(unitTestConsole, 
                regexPatterns, 
                isRegex: true, 
                shouldMatch: false, 
                onError: (printBuffer, pattern) => $"Regex '{pattern}' was found in the buffer: \r\n" + printBuffer);
        }

        private static async Task AssertPatterns(UnitTestConsole unitTestConsole, string[] patterns, bool isRegex, bool shouldMatch, Func<string, string, string> onError)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                string printBuffer = unitTestConsole.PixelBuffer.PrintBuffer();
                foreach (string pattern in patterns)
                    if (shouldMatch)
                        Assert.IsTrue(IsMatch(printBuffer, isRegex, pattern), onError(printBuffer, pattern));
                    else
                        Assert.IsFalse(IsMatch(printBuffer, isRegex, pattern), onError(printBuffer, pattern));
            });
        }

        private static bool IsMatch(string printBuffer, bool isRegex, string pattern)
        {
            if (isRegex)
            {
                var regex = new Regex(pattern);
                return regex.IsMatch(printBuffer);
            }
            else
            {
                return printBuffer.Contains(pattern, StringComparison.Ordinal);
            }
        }
    }
}