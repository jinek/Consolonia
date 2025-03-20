using System;

namespace Consolonia.Core.Helpers.InputProcessor
{
    public abstract class MatcherWithComplete<T, TComplete> : IMatcher<T>
    {
        protected MatcherWithComplete(Action<TComplete> onComplete)
        {
            OnComplete += onComplete;
        }

        public abstract AppendResult Append(T input);
        public abstract bool TryFlush();
        public abstract void Reset();

        public event Action<TComplete> OnComplete;

        protected void Complete(TComplete input)
        {
            OnComplete.Invoke(input);
        }
    }
}