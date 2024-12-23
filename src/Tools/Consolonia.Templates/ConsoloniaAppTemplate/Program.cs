using Avalonia;
using Consolonia;

namespace ConsoloniaAppTemplate
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            BuildAvaloniaApp()
                .StartWithConsoleLifetime(args);
        }

        public static AppBuilder BuildAvaloniaApp()
        {
            return AppBuilder.Configure<ConsoloniaAppTemplateApp>()
                .UseConsolonia()
                .UseAutoDetectedConsole()
                .LogToException();
        }
    }
}