using Avalonia;
using Avalonia.Input;
using Consolonia;

namespace ConsoloniaEdit
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
            return AppBuilder.Configure<App>()
                .UseConsolonia()
                .UseAutoDetectedConsole()
                .WithDeveloperTools(o =>
                {
                    o.Gesture = new KeyGesture(Key.F1);
                    // o.Runner = new MyRunner();
                })
                .LogToException();
        }
    }
}