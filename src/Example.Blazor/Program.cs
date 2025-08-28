using Avalonia;
using Consolonia;

namespace Example.Blazor
{
    internal class Program
    {
        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local Exactly why we are keeping it here
        [STAThread]
        private static void Main(string[] args)
        {
            BuildAvaloniaApp()
                .StartWithConsoleLifetime(args);
        }

        public static AppBuilder BuildAvaloniaApp()
        {
            return AppBuilder.Configure<App>()
                .UseConsoloniaBlazor()
                .UseAutoDetectedConsole()
                .LogToException();
        }
    }
}