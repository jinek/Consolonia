using System;

namespace Consolonia.Core.Infrastructure
{
    public class ConsoloniaNotSupportedException : Exception
    {
        internal ConsoloniaNotSupportedException(NotSupportedRequest request)
        {
            Request = request;
        }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public NotSupportedRequest Request { get; }
    }
}