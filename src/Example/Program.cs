using Avalonia;
using Consolonia.Core;
using Consolonia.Core.Infrastructure;

namespace Example
{
    internal static class Program
    {
        public static int Main()
        {
            return AppBuilder.Configure<App>()
                .UseConsolonia()
                .UseStandardConsole()
                .LogToException()
                .StartWithConsoleLifetime(null);
        }
    }
}