using Avalonia;
using Consolonia.Core;
using Consolonia.PlatformSupport;

namespace Example
{
    internal static class Program
    {
        public static int Main()
        {
            return AppBuilder.Configure<App>()
                .UseConsolonia()
                .UseStandardConsole()
                .LogToTrace()
                .StartWithConsoleLifetime(null);
        }
    }
}