using System;

namespace Consolonia.Core.Infrastructure
{
    public class ConsoloniaNotSupportedException : Exception
    {
        private readonly Type _resultType;

        internal ConsoloniaNotSupportedException(NotSupportedRequest request, Type resultType)
        {
            _resultType = resultType;
            Request = request;
        }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public NotSupportedRequest Request { get; }

        public override string Message =>
            $"Consolonia does not support the requested operation: {Request.ErrorCode}. " +
            $"Subscribe to ConsoloniaPlatform.NotSupported to intercept before it throws. " +
            $"Expected result type: {_resultType.Name}. " +
            $"Inputs: {string.Join(", ", Request.Information)}. " +
            $"Use Request.SetHandled(yourValue) to return a fallback." +
            "To ignore all errors use AppBuilder.WithIgnoreErrorsSettings()";
    }
}