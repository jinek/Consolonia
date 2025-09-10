using System;
using System.Diagnostics;
using System.Net;
using Avalonia;
using Avalonia.Input;
using AvaloniaUI.DiagnosticsSupport;

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
            return AppBuilder.Configure<App>()
                .UseConsolonia()
                .UseAutoDetectedConsole()
                .ThrowOnErrors()
                .WithDeveloperTools()
                .LogToException();
        }
    }
}
