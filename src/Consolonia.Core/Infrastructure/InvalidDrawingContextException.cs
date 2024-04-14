using System;

namespace Consolonia.Core.Infrastructure
{
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