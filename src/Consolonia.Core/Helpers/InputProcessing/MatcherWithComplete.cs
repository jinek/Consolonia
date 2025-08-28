using System;
using Consolonia.Core.Helpers.Logging;
using NLog;

namespace Consolonia.Core.Helpers.InputProcessing
{
    public abstract class MatcherWithComplete<T, TComplete> : IMatcher<T>
    {
        private readonly ILogger _logger = Log.CreateInputLogger();

        protected MatcherWithComplete(Action<TComplete> onComplete)
        {
            OnComplete += onComplete;
        }

        public abstract AppendResult Append(T input);
        public abstract bool TryFlush();
        public abstract void Reset();

        public abstract string GetDebugInfo();

        public event Action<TComplete> OnComplete;

        protected void Complete(TComplete input)
        {
            _logger.Info($"Complete fired for {GetDebugInfo()} with input: {input}");
            OnComplete.Invoke(input);
        }
    }
}