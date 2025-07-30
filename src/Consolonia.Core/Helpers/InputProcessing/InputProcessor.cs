#nullable enable

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Consolonia.Core.Helpers.Logging;
using NLog;

namespace Consolonia.Core.Helpers.InputProcessing
{
    public class InputProcessor<T>(IEnumerable<IMatcher<T>> matchers)
    {
        private readonly ILogger _logger = Log.CreateInputLogger();

        private int _previousTopMatcherIndex = -1;
        private ImmutableArray<IMatcher<T>> Matchers { get; } = [..matchers];

        public void ProcessChunk(IReadOnlyCollection<T> chunk)
        {
            _logger.Trace("Processing input chunk with {Count} items...", chunk.Count);
            LogMatchersState();
            foreach (T input in chunk)
                try
                {
                    ProcessSingleInput(input);
                }
                finally
                {
                    LogMatchersState();
                }

            FlushStartingFrom(0);
        }

        private void ProcessSingleInput(T input)
        {
            _logger.Info("Processing single input: {Input}", input);
            int currentTopMatcherIndex = Matchers.Length;


            for (int i = 0; i < Matchers.Length; i++)
            {
                IMatcher<T> matcher = Matchers[i];
                _logger.Trace("Processing matcher {MatcherIndex}: {Matcher}", i, matcher.GetDebugInfo());
                AppendResult result = matcher.Append(input);
                _logger.Trace("Matcher {MatcherIndex} result: {Result}", i, result);
                bool isPreviousTopMatcher = i == _previousTopMatcherIndex;
                _logger.Trace("Is previous top matcher: {IsPreviousTopMatcher}", isPreviousTopMatcher);

                if (result == AppendResult.NoMatch)
                {
                    if (isPreviousTopMatcher)
                    {
                        _logger.Trace(
                            "This matcher was previous top matcher, flushing starting from index {MatcherIndex}", i);
                        FlushStartingFrom(i);
                    }
                    else
                    {
                        _logger.Trace("This matcher was not previous top matcher, resetting it");
                        matcher.Reset();
                    }
                }

                if (result != AppendResult.NoMatch)
                {
                    _logger.Trace(
                        "Matcher {MatcherIndex} matched, setting current top matcher index to minimum with {CurrentTopMatcherIndex}",
                        i, currentTopMatcherIndex);
                    currentTopMatcherIndex = Math.Min(currentTopMatcherIndex, i);

                    if (i < _previousTopMatcherIndex)
                    {
                        _logger.Trace(
                            "Matcher {MatcherIndex} is below previous top matcher index {PreviousTopMatcherIndex}, flushing starting from it",
                            i, _previousTopMatcherIndex);
                        FlushStartingFrom(_previousTopMatcherIndex);
                        _logger.Trace("Resetting previous top matcher index to -1");
                        _previousTopMatcherIndex = currentTopMatcherIndex;
                    }
                }

                if (result == AppendResult.AutoFlushed)
                    /* && isPreviousTopMatcher having no idea what for this was needed. But if nothing was autoflashed, everything else should be reset because each single input must be processed only once*/
                {
                    _logger.Trace(
                        "Auto-flush occurred, resetting all matchers from index 0 and setting previous top matcher index to -1");
                    ResetMatchersFrom(0);
                    _previousTopMatcherIndex =
                        currentTopMatcherIndex = -1; // redundant set of _previousTopMatcherIndex for readability
                    break;
                }
            }

            _logger.Info(
                "Finished processing input: {Input}, current top matcher index: {CurrentTopMatcherIndex}, previous top matcher index: {PreviousTopMatcherIndex}, updating previous to current",
                input, currentTopMatcherIndex, _previousTopMatcherIndex);
            _previousTopMatcherIndex = currentTopMatcherIndex;
        }

        private void FlushStartingFrom(int startIndex)
        {
            for (int i = startIndex; i < Matchers.Length; i++)
                if (Matchers[i].TryFlush())
                {
                    _logger.Info(
                        "Matcher {Matcher} at index {MatcherIndex} flushed successfully, resetting matchers from index {NextMatcherIndex}",
                        Matchers[i].GetDebugInfo(), i, i + 1);
                    ResetMatchersFrom(i + 1);
                    break;
                }
            /*else
            {
                // because of 15F2A2C4-218D-4B4D-86CE-330A312EF6A6 we can not reset all others even if it seems logical
            }*/
        }

        private void ResetMatchersFrom(int startIndex)
        {
            for (int i = startIndex; i < Matchers.Length; i++)
                Matchers[i].Reset();
        }

        private void LogMatchersState()
        {
            StringBuilder sb = new("Current matchers state(* - top matcher): ");
            sb.AppendLine();
            for (int i = 0; i < Matchers.Length; i++)
            {
                IMatcher<T> matcher = Matchers[i];
                string topMatcherIndicator = i == _previousTopMatcherIndex ? "*" : string.Empty;
                sb.AppendLine($"[{topMatcherIndicator}{i}{topMatcherIndicator}: {matcher.GetDebugInfo()}] ");
            }

            _logger.Info(sb.ToString());
        }
    }
}