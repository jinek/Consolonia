using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;

namespace Consolonia.Gallery
{
    internal static class Program
    {
        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local Exactly why we are keeping it here
        [STAThread]
        private static void Main(string[] args)
        {
            TaskScheduler.UnobservedTaskException += (sender, eventArgs) =>
            {
                if (Debugger.IsAttached) Debugger.Break();

                ThreadPool.QueueUserWorkItem(state =>
                    throw new InvalidOperationException("An unobserved task exception occurred.", eventArgs.Exception));
            };

            BuildAvaloniaApp()
                .StartWithConsoleLifetime(args);
        }

        public static AppBuilder BuildAvaloniaApp()
        {
            return AppBuilder.Configure<App>()
                .UseConsolonia()
                .UseAutoDetectedConsole()
                .ThrowOnErrors()
                .LogToException();
        }
    }
}