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
            if (args.Length > 1) throw new NotSupportedException();

            AppBuilder.Configure<App>()
                .UseConsolonia()
                .UseAutoDetectedConsole()
                .LogToException()
                .StartWithConsoleLifetime(args);
        }
    }
}