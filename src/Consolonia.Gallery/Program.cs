using System;
using Avalonia;
using Consolonia.Core.Drawing.PixelBufferImplementation.EgaConsoleColor;

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
                .UseStandardConsole()
                /*.UseConsoleColorMode(new EgaConsoleColorMode())*/
                .LogToException();
        }
    }
}