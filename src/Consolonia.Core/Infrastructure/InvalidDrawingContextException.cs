using System;
using JetBrains.Annotations;

// ReSharper disable UnusedMember.Global

namespace Consolonia.Core.Infrastructure
{
    public class InvalidDrawingContextException : ApplicationException
    {
        public InvalidDrawingContextException()
        {
        }

        public InvalidDrawingContextException([CanBeNull] string message) : base(message)
        {
        }

        public InvalidDrawingContextException([CanBeNull] string message, [CanBeNull] Exception innerException) : base(
            message, innerException)
        {
        }
    }
}