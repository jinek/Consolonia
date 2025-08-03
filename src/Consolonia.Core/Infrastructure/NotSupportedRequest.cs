using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

// ReSharper disable MemberCanBePrivate.Global

namespace Consolonia.Core.Infrastructure
{
    public sealed class NotSupportedRequest(NotSupportedRequestCode errorCode, IList<object> information)
    {
        //todo: low: would be much better to have an inheritor for each type of error, but we can not afford that currently
        public bool Handled { get; private set; }
        public object Result { get; private set; }
        public NotSupportedRequestCode ErrorCode { get; } = errorCode;

        public ReadOnlyCollection<object> Information { get; } = new(information);

        [DebuggerStepThrough]
        internal void CheckHandled(Type resultType)
        {
            if (!Handled)
                throw new ConsoloniaNotSupportedException(this, resultType);
        }

        public void SetHandled(object result = HandledResult)
        {
            Result = result;
            Handled = true;
        }

        public const object HandledResult = null;
    }
}