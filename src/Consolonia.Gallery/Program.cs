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
                .WithDeveloperTools(o =>
                {
                    o.Gesture =  new KeyGesture(Key.F1);
                   // o.Runner = new MyRunner();
                })
                .LogToException();
        }
    }
}
public record MyRunner : DeveloperToolsRunner
{
    public override bool Run(string args)
    {
        Process process = new Process();
        // process.StartInfo.WorkingDirectory = Environemnt.
        process.StartInfo.FileName = "avdt";
        process.StartInfo.UseShellExecute = true;
        process.StartInfo.CreateNoWindow = false;
        process.StartInfo.Arguments = args;
        if (process.Start())
        {
            return true;
        }

        return false;
    }
}