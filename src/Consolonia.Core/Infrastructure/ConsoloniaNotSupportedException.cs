using System;
using System.Runtime.Serialization;

namespace Consolonia.Core.Infrastructure
{
    [Serializable]
    public class ConsoloniaNotSupportedException : Exception
    {
        internal ConsoloniaNotSupportedException(NotSupportedRequest request)
        {
            Request = request;
        }

        protected ConsoloniaNotSupportedException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }

        public NotSupportedRequest Request { get; }
    }
}