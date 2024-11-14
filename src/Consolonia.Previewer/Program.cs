using Avalonia;
using Consolonia.Core;
using Consolonia.Core.Infrastructure;
using Consolonia.PlatformSupport;

namespace Consolonia.Previewer
{
    internal class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args)
        {
            BuildAvaloniaApp()
                   .StartWithConsoleLifetime(args);
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
        {
            return AppBuilder.Configure<App>()
                  .UseConsolonia()
                  .UseAutoDetectedConsole()
                  .LogToException();
        }
        //AppBuilder.Configure<App>()
        //        .UsePlatformDetect()
        //        .WithInterFont()
        //        .LogToTrace();
    }
}
