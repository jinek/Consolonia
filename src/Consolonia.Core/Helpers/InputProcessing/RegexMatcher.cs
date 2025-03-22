using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Consolonia.Core.Helpers.InputProcessing
{
    public class RegexMatcher<T>(
        Action<(string, T[])> onComplete,
        Func<T, char> toChar,
        string regex,
        int? autoFlushOnLength = null)
        : MatcherWithComplete<T, (string, T[])>(onComplete)
    {
        private readonly List<T> _accumulatedKeys = [];
        private readonly StringBuilder _accumulator = new();

        private readonly Regex _regex = new(regex);

        public override AppendResult Append(T input)
        {
            char c = toChar(input);

            _accumulator.Append(c);

            string accumulatedString = _accumulator.ToString();
            AppendResult matchResultInternal = MatchResultInternal(accumulatedString);
            if (matchResultInternal != AppendResult.NoMatch)
            {
                _accumulatedKeys.Add(input);

                if (autoFlushOnLength != null && _accumulator.Length == (int)autoFlushOnLength)
                {
                    Complete((accumulatedString, _accumulatedKeys.ToArray()));
                    Reset();
                    return AppendResult.AutoFlushed;
                }
            }
            else
            {
                _accumulator.Length--;
            }

            return matchResultInternal;
        }


        private AppendResult MatchResultInternal(string toTest)
        {
            return _regex.IsMatch(toTest) ? AppendResult.Match : AppendResult.NoMatch;
        }

        public override bool TryFlush()
        {
            if (_accumulator.Length == 0) return false;

            if (autoFlushOnLength != null)
                return false;

            string currentAccumulated = _accumulator.ToString();
            bool matches = MatchResultInternal(currentAccumulated) == AppendResult.Match;
            if (matches)
            {
                Complete((currentAccumulated, _accumulatedKeys.ToArray()));
                Reset();
            }

            return matches;
        }

        public override void Reset()
        {
            _accumulator.Clear();
            _accumulatedKeys.Clear();
        }
    }
}