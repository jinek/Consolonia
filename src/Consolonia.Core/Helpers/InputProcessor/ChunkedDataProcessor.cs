#nullable enable

using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Consolonia.Core.Helpers.InputProcessor
{
    public class ChunkedDataProcessor<T>(IEnumerable<IMatcher<T>> matchers)
    {
        private ImmutableArray<IMatcher<T>> Matchers { get; } = [..matchers];
        private int _previousTopMatcherIndex = -1;

        public void ProcessDataChunk(IEnumerable<T> chunk)
        {
            foreach (T input in chunk)
            {
                ProcessSingleInput(input);
            }

            Flush(0);
        }

        private void ProcessSingleInput(T input)
        {
            int currentTopMatcherIndex = Matchers.Length;

            for (int i = 0; i < Matchers.Length; i++)
            {
                IMatcher<T> matcher = Matchers[i];
                AccumulationResult result = matcher.Accumulate(input);

                bool isPreviousTopMatcher = i == _previousTopMatcherIndex;

                if (result == AccumulationResult.NoMatch && isPreviousTopMatcher)
                {
                    if (isPreviousTopMatcher)
                        Flush(i);
                    else matcher.Reset();
                }

                if (result != AccumulationResult.NoMatch)
                {
                    currentTopMatcherIndex = Math.Min(currentTopMatcherIndex, i);

                    if (i < _previousTopMatcherIndex)
                    {
                        Flush(_previousTopMatcherIndex);
                        _previousTopMatcherIndex = currentTopMatcherIndex;
                    }
                }

                if (result == AccumulationResult.AutoFlushed && isPreviousTopMatcher)
                {
                    ResetMatchersFrom(0);
                    _previousTopMatcherIndex =
                        currentTopMatcherIndex = -1; // redundant set of _previousTopMatcherIndex for readability
                    break;
                }
            }

            _previousTopMatcherIndex = currentTopMatcherIndex;
        }

        private void Flush(int startIndex)
        {
            for (int i = startIndex; i < Matchers.Length; i++)
            {
                if (Matchers[i].TryFlush())
                {
                    ResetMatchersFrom(i + 1);
                    break;
                }
            }
        }

        private void ResetMatchersFrom(int startIndex)
        {
            for (int i = startIndex; i < Matchers.Length; i++)
                Matchers[i].Reset();
        }
    }
}
