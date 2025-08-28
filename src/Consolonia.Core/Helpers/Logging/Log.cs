using NLog;

namespace Consolonia.Core.Helpers.Logging
{
    internal static class Log
    {
        public static ILogger Create(LogCategory category)
        {
            return LogManager.GetLogger("Consolonia." + category);
        }

        public static ILogger CreateInputLogger()
        {
            return Create(LogCategory.Input);
        }
    }
}