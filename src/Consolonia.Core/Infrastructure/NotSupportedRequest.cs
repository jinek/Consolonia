using System.Diagnostics;

namespace Consolonia.Core.Infrastructure
{
    public sealed class NotSupportedRequest
    {
        internal NotSupportedRequest()
        {
        }
        
        public bool Handled { get; private set; }
        public int ErrorCode { get; internal set; }
        public object[] Information { get; set; }

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