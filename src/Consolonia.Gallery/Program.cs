using System;
using Avalonia;
using Consolonia.Core;
using Consolonia.Core.Infrastructure;
using Consolonia.PlatformSupport;

namespace Consolonia.Gallery
{
    internal static class Program
    {
        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local Exactly why we are keeping it here
        private static void Main(string[] args)
        {
            BuildAvaloniaApp()
                .StartWithConsoleLifetime(args);
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UseConsolonia()
                .UseAutoDetectedConsole()
                .LogToException();

    }
}