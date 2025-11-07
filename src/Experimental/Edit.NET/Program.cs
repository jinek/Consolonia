// DUPFINDER_ignore

using Avalonia;
using Consolonia;

namespace EditNET
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            BuildAvaloniaApp()
                .StartWithConsoleLifetime(args);
        }

        private static AppBuilder BuildAvaloniaApp()
        {
            return AppBuilder.Configure<App>()
                .UseConsolonia()
                .UseAutoDetectedConsole()
                .WithDeveloperTools()
                .LogToException();
        }
    }
}