using System;
using System.Collections.Generic;

namespace Consolonia.Core.Helpers.InputProcessor
{
    /// <summary>
    ///     Matches everything
    /// </summary>
    public class GenericMatcher<TKey>(Action<TKey> onComplete)
        : MatcherWithComplete<TKey, TKey>(onComplete)
    {
        private readonly List<TKey> _accumulatedKeys = [];

        public override AppendResult Append(TKey input)
        {
            _accumulatedKeys.Add(input);
            return AppendResult.Match;
        }

        public override bool TryFlush()
        {
            if (_accumulatedKeys.Count == 0)
                return false;

            foreach (TKey key in _accumulatedKeys)
                Complete(key);
            _accumulatedKeys.Clear();
            return true;
        }

        public override void Reset()
        {
            _accumulatedKeys.Clear();
        }
    }
}