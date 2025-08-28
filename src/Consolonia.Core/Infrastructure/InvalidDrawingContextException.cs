using System;
using System.Diagnostics.CodeAnalysis;

namespace Consolonia.Core.Infrastructure
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class InvalidDrawingContextException : ApplicationException
    {
        public InvalidDrawingContextException()
        {
        }

        public InvalidDrawingContextException(string message) : base(message)
        {
        }

        public InvalidDrawingContextException(string message, Exception innerException) : base(
            message, innerException)
        {
        }
    }
}