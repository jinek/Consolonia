using System;

namespace Consolonia.Core.Infrastructure
{
    public class ConsoloniaNotSupportedException : Exception
    {
        internal ConsoloniaNotSupportedException(NotSupportedRequest request)
        {
            Request = request;
        }

        public NotSupportedRequest Request { get; }
    }
}