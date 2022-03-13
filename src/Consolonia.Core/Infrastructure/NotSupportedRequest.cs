using System.Collections.ObjectModel;
using System.Diagnostics;

// ReSharper disable MemberCanBePrivate.Global

namespace Consolonia.Core.Infrastructure
{
    public sealed class NotSupportedRequest
    {
        public NotSupportedRequest(int errorCode, object[] information)
        {
            ErrorCode = errorCode;
            Information = new ReadOnlyCollection<object>(information);
        }

        public bool Handled { get; private set; }
        public int ErrorCode { get; }

        public ReadOnlyCollection<object> Information { get; }

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