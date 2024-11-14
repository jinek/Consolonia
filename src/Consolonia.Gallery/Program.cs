using System;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
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
        {
#if DEBUG
            if (AppDomain.CurrentDomain.FriendlyName == "Avalonia.Designer.HostApp")
            {
                return AppBuilder.Configure<App>()
                    .UsePlatformDetect()
                    .WithInterFont()
                    .LogToTrace();
            }
#endif

            return AppBuilder.Configure<App>()
                .UseConsolonia()
                .UseAutoDetectedConsole()
                .LogToException();
        }

    }
}