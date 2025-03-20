using System;
using System.Text;

namespace Consolonia.Core.Helpers.InputProcessor
{
    /// <summary>
    /// Matches a sequence of characters that starts with a given string and ends with another given string.
    /// </summary>
    public class StartsEndsWithMatcher<T>(
        Action<string> onComplete,
        Func<T, char> toChar,
        string startsWith,
        string endsWith)
        : MatcherWithComplete<T, string>(onComplete)
    {
        private readonly StringBuilder _accumulator = new();

        public override AppendResult Append(T input)
        {
            char c = toChar(input);
            bool accumulatorWasEmpty = _accumulator.Length == 0;

            AppendResult matchResultInternal = MatchResultInternal(_accumulator.ToString() + c, accumulatorWasEmpty);
            if (matchResultInternal == AppendResult.Match)
            {
                _accumulator.Append(c);
            }

            return matchResultInternal;
        }

        private AppendResult MatchResultInternal(string toTest, bool accumulatorWasEmpty)
        {
            bool startsWithTrue = toTest.StartsWith(startsWith, StringComparison.Ordinal);

            if (startsWithTrue && toTest.EndsWith(endsWith, StringComparison.Ordinal))
            {
                string output = toTest.Substring(startsWith.Length,
                    toTest.Length - startsWith.Length - endsWith.Length);

                Complete(output);
                _accumulator.Clear();
                return AppendResult.AutoFlushed;
            }

            if (startsWithTrue || startsWith.StartsWith(toTest, StringComparison.Ordinal))
            {
                return AppendResult.Match;
            }

            if (!accumulatorWasEmpty)
            {
                _accumulator.Clear();
            }

            return AppendResult.NoMatch;
        }

        public override bool TryFlush()
        {
            return _accumulator.Length != 0;
            // always lying that we are flushed to indicate others should not flush 15F2A2C4-218D-4B4D-86CE-330A312EF6A6
        }

        public override void Reset()
        {
            // because of 15F2A2C4-218D-4B4D-86CE-330A312EF6A6 we are pretending to reset, but we are not buying the lie of the previous tag matcher 
        }
    }
}