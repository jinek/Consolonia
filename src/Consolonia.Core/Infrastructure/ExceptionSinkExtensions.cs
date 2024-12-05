using Avalonia;
using Avalonia.Logging;
using Consolonia.Core.Infrastructure;

namespace Consolonia
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