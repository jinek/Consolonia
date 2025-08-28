using Avalonia;
using Avalonia.Logging;
using Consolonia.Core.Infrastructure;

// ReSharper disable CheckNamespace
#pragma warning disable IDE0130
namespace Consolonia
#pragma warning restore IDE0130
// ReSharper disable once ArrangeNamespaceBody
{
    public static class ExceptionSinkExtensions
    {
        public static AppBuilder LogToException(this AppBuilder builder)
        {
            Logger.Sink = new ExceptionSink();
            return builder;
        }
    }
    // ReSharper restore UnusedMember.Global
}