using Avalonia;
using Consolonia.Core;
using Consolonia.Windows;
using Consolonia.Windows.Curses;

namespace Example
{
    internal static class Program
    {
        public static int Main()
        {
            return AppBuilder.Configure<App>()
                .UseConsolonia()
                //.UseStandardConsole()
                .UseConsole(new CursesConsole())
                .LogToTrace()
                .StartWithConsoleLifetime(null);
        }
    }
}