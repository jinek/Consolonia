using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Consolonia.Core.Helpers.InputProcessor
{
    public class RegexMatcher<T>(Action<(string, T[])> onComplete, Func<T, char> toChar, string regex)
        : MatcherWithComplete<T, (string, T[])>(onComplete)
    {
        private readonly List<T> _accumulatedKeys = [];
        private readonly StringBuilder _accumulator = new();

        private readonly Regex _regex = new(regex);

        public override AccumulationResult Accumulate(T input)
        {
            char c = toChar(input);

            AccumulationResult
                matchResultInternal = MatchResultInternal(_accumulator.ToString() + c); //todo: performance
            if (matchResultInternal != AccumulationResult.NoMatch)
            {
                _accumulator.Append(c);
                _accumulatedKeys.Add(input);
            }

            return matchResultInternal;
        }


        private AccumulationResult MatchResultInternal(string toTest)
        {
            return _regex.IsMatch(toTest) ? AccumulationResult.Match : AccumulationResult.NoMatch;
        }

        public override bool TryFlush()
        {
            if (_accumulator.Length == 0) return false;

            string currentAccumulated = _accumulator.ToString();
            bool matches = MatchResultInternal(currentAccumulated) == AccumulationResult.Match;
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