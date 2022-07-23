using System;
using Avalonia;
using Consolonia.Core;
using Consolonia.PlatformSupport;

namespace Consolonia.Gallery
{
    internal static class Program
    {
        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local Exactly why we are keeping it here
        private static void Main(string[] args)
        {
            if (args.Length > 1) throw new NotSupportedException();

            AppBuilder.Configure<App>()
                .UseConsolonia()
                .UseAutoDetectedConsole()
                //.UseConsole(new CursesConsole())
                .LogToTrace()
                .StartWithConsoleLifetime(args);
        }
    }
}