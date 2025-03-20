using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Consolonia.Core.Helpers.InputProcessing
{
    public class RegexMatcher<T>(Action<(string, T[])> onComplete, Func<T, char> toChar, string regex)
        : MatcherWithComplete<T, (string, T[])>(onComplete)
    {
        private readonly List<T> _accumulatedKeys = [];
        private readonly StringBuilder _accumulator = new();

        private readonly Regex _regex = new(regex);

        public override AppendResult Append(T input)
        {
            char c = toChar(input);

            _accumulator.Append(c);

            AppendResult matchResultInternal = MatchResultInternal(_accumulator.ToString());
            if (matchResultInternal != AppendResult.NoMatch)
                _accumulatedKeys.Add(input);
            else
                _accumulator.Length--;

            return matchResultInternal;
        }


        private AppendResult MatchResultInternal(string toTest)
        {
            return _regex.IsMatch(toTest) ? AppendResult.Match : AppendResult.NoMatch;
        }

        public override bool TryFlush()
        {
            if (_accumulator.Length == 0) return false;

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