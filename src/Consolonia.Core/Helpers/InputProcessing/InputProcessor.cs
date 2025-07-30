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

            foreach (T input in chunk)
                ProcessSingleInput(input);

            _logger.Info("Trying to flush starting from index 0");
            TryFlushStartingFrom(0);
            LogMatchersState();
        }

        private void ProcessSingleInput(T input)
        {
            _logger.Info("Processing single input: {Input}", input);
            int currentTopMatcherIndex = Matchers.Length;


            StringBuilder logAllSb = new();
            logAllSb.AppendLine();
            logAllSb.AppendLine("-----------------------------------------------------");
            for (int i = 0; i < Matchers.Length; i++)
            {
                StringBuilder logSb = new();
                try
                {
                    IMatcher<T> matcher = Matchers[i];

                    _logger.Trace("Processing matcher {MatcherIndex}: {Matcher}", i, matcher.GetDebugInfo());
                    logSb.AppendJoin(" ", i.ToString(), i == _previousTopMatcherIndex ? "*" : " ",
                        matcher.GetDebugInfo());
                    AppendResult result = matcher.Append(input);
                    _logger.Trace("Matcher {MatcherIndex} result: {Result}", i, result);
                    bool isPreviousTopMatcher = i == _previousTopMatcherIndex;
                    _logger.Trace("Is previous top matcher: {IsPreviousTopMatcher}", isPreviousTopMatcher);

                    if (result == AppendResult.NoMatch)
                    {
                        logSb.Insert(0, '❌');
                        if (isPreviousTopMatcher)
                        {
                            logSb.Insert(1, "?");
                            TryFlushStartingFrom(i);
                        }
                        else
                        {
                            matcher.Reset();
                        }
                    }

                    if (result != AppendResult.NoMatch)
                    {
                        logSb.AppendJoin("", " + ", input?.ToString());
                        currentTopMatcherIndex = Math.Min(currentTopMatcherIndex, i);
                        logSb.Insert(0, i == currentTopMatcherIndex ? '*' : '✅');

                        if (i < _previousTopMatcherIndex)
                        {
                            logSb.Insert(1, "?");
                            TryFlushStartingFrom(_previousTopMatcherIndex);
                            _previousTopMatcherIndex = currentTopMatcherIndex;
                        }
                    }

                    if (result == AppendResult.AutoFlushed)
                        /* && isPreviousTopMatcher having no idea what for this was needed. But if something was autoflashed, everything else should be reset because each single input must be processed only once*/
                    {
                        logSb.Insert(0, "⚡");
                        ResetMatchersFrom(0);
                        _previousTopMatcherIndex =
                            currentTopMatcherIndex = -1; // redundant set of _previousTopMatcherIndex for readability
                        break;
                    }
                }
                finally
                {
                    logSb.AppendLine();
                    logAllSb.Append(logSb);
                }
            }

            logAllSb.AppendLine("-----------------------------------------------------");
            logAllSb.AppendLine(
                "Legend: * - top matcher, ✅ - match, ❌ - no match, ⚡ - auto flushed, ? - reset matchers from this index");
            logAllSb.AppendLine("-----------------------------------------------------");

            _logger.Info(logAllSb.ToString());

            _logger.Info(
                "Finished processing input: {Input}, current top matcher index: {CurrentTopMatcherIndex}, previous top matcher index: {PreviousTopMatcherIndex}, updating previous to current",
                input, currentTopMatcherIndex, _previousTopMatcherIndex);
            _previousTopMatcherIndex = currentTopMatcherIndex;
        }

        private void TryFlushStartingFrom(int startIndex)
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
            sb.AppendLine("-----------------------------------------------------");
            for (int i = 0; i < Matchers.Length; i++)
            {
                IMatcher<T> matcher = Matchers[i];
                sb.AppendJoin(" ", i.ToString(), i == _previousTopMatcherIndex ? "*" : " ", matcher.GetDebugInfo());
                sb.AppendLine();
            }

            sb.AppendLine("-----------------------------------------------------");
            _logger.Info(sb.ToString());
        }
    }
}