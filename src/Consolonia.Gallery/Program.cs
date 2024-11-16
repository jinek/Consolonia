using System;
using Avalonia;
using Consolonia.Core;
using Consolonia.Core.Infrastructure;
using Consolonia.Designer;
using Consolonia.PlatformSupport;

namespace Consolonia.Gallery
{
    internal static class Program
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
#if DEBUG
            return AppBuilder.Configure<App>()
                .UseConsoloniaDesigner()
                .UseAutoDetectedConsole()
                .LogToException();
#else
            return AppBuilder.Configure<App>()
                .UseConsolonia()
                .UseAutoDetectedConsole()
                .LogToException();
#endif
        }

    }
}