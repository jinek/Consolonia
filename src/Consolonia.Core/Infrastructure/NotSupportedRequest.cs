using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

// ReSharper disable MemberCanBePrivate.Global

namespace Consolonia.Core.Infrastructure
{
    public sealed class NotSupportedRequest(int errorCode, IList<object> information)
    {
        public bool Handled { get; private set; }
        public int ErrorCode { get; } = errorCode;

        public ReadOnlyCollection<object> Information { get; } = new(information);

        [DebuggerStepThrough]
        internal void CheckHandled()
        {
            if (!Handled)
                throw new ConsoloniaNotSupportedException(this);
        }

        public void SetHandled()
        {
            Handled = true;
        }
    }
}