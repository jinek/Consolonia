using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace Consolonia.Core.Infrastructure
    // ReSharper disable once ArrangeNamespaceBody
{
    [UsedImplicitly]
    public class ConsoloniaException : ApplicationException
    {
        public ConsoloniaException()
        {
        }

        protected ConsoloniaException([NotNull] SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ConsoloniaException([CanBeNull] string message) : base(message)
        {
        }

        public ConsoloniaException([CanBeNull] string message, [CanBeNull] Exception innerException) : base(message,
            innerException)
        {
        }
    }
}