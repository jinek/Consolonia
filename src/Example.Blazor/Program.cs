using Avalonia;
using BlazorBindingsAvalonia;
using Consolonia.Core;
using Consolonia.Core.Infrastructure;
using Consolonia.PlatformSupport;

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
         => AppBuilder.Configure<App>()
            .UseConsolonia()
            .UseAutoDetectedConsole()
            .LogToException()
            .UseAvaloniaBlazorBindings(services =>
            {
            });
    }
}
