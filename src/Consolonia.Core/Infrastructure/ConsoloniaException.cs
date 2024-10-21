using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Consolonia.Core.Infrastructure
{
    public class ConsoloniaException : ApplicationException
    {
        // ReSharper disable UnusedMember.Global
        public ConsoloniaException()

        {
        }

        protected ConsoloniaException([NotNull] SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ConsoloniaException(string message) : base(message)
        {
        }

        public ConsoloniaException(string message, Exception innerException) : base(message,
            innerException)
        {
        }
        // ReSharper restore UnusedMember.Global
    }
}