using Avalonia;
using Consolonia.Core;
using Consolonia.Windows;

namespace Example
{
    internal static class Program
    {
        public static int Main()
        {
            return AppBuilder.Configure<App>()
                .UseConsolonia()
                //.UseStandardConsole()
                .UseConsole(new WinConsole())
                .LogToTrace()
                .StartWithConsoleLifetime(null);
        }
    }
}