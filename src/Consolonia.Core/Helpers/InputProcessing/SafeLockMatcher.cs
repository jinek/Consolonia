using System.Collections.Generic;
using System.Linq;

namespace Consolonia.Core.Helpers.InputProcessing
{
    /// <summary>
    ///     Safe with integer array key
    /// </summary>
    public class SafeLockMatcher : IMatcher<(int, int)>
    {
        private readonly int[] _key;
        private readonly IMatcher<int> _lockedMatcher;
        private short _keyStep;

        public SafeLockMatcher(IMatcher<int> lockedMatcher, int key1, int? key2 = null, int? otherKeys = null)
        {
            _lockedMatcher = lockedMatcher;

            var key = new List<int>([key1]);
            if (key2.HasValue) key.Add(key2.Value);

            if (otherKeys.HasValue) key.Add(otherKeys.Value);

            _key = [.. key];
        }

        public AppendResult Append((int, int) input)
        {
            if (input.Item1 == _key[_keyStep])
            {
                if (_key.Length > _keyStep + 1)
                    _keyStep++;
                AppendResult appendResult = _lockedMatcher.Append(input.Item2);
                if (appendResult is AppendResult.AutoFlushed or AppendResult.NoMatch)
                    _keyStep = 0;

                return appendResult;
            }

            if (_keyStep > 0)
            {
                _lockedMatcher.Append(0); // should reset the liers
                _lockedMatcher.Reset(); // should reset civilians
                _keyStep = 0;
            }

            return AppendResult.NoMatch;
        }

        public bool TryFlush()
        {
            bool hasFlushed = _lockedMatcher.TryFlush();
            if (hasFlushed)
                _keyStep = 0;

            return hasFlushed;
        }

        public void Reset()
        {
            _lockedMatcher.Reset();
            // doing nothing, we reset only when append does not match
        }

        public string GetDebugInfo()
        {
            return $"{GetType().Name} [{GetKeyWithCurrentStep()}] -> {_lockedMatcher.GetDebugInfo()}";

            string GetKeyWithCurrentStep()
            {
                return string.Join(", ", _key.Select((k, i) => i == _keyStep ? $"[{k}]" : k.ToString()));
            }
        }
    }
}