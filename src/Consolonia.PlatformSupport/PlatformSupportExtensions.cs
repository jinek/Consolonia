using System;
using Avalonia;
using Avalonia.Controls;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Consolonia.Core.Drawing.PixelBufferImplementation.EgaConsoleColor;
using Consolonia.Core.Dummy;
using Consolonia.Core.Infrastructure;
using Consolonia.PlatformSupport;

// ReSharper disable CheckNamespace
#pragma warning disable IDE0130
#pragma warning disable IDE0161
namespace Consolonia
#pragma warning restore IDE0130
#pragma warning restore IDE0161
{
    public static class PlatformSupportExtensions
    {
        public static AppBuilder UseAutoDetectedConsole(this AppBuilder builder)
        {
            if (Design.IsDesignMode)
                // in design mode we can't use any console operations at all, so we use a dummy IConsole.
                return builder.UseConsole(new DummyConsole());

#pragma warning disable CA1416
            IConsole console = Environment.OSVersion.Platform switch
            {
                PlatformID.Win32S or PlatformID.Win32Windows or PlatformID.Win32NT => new Win32Console(),
                PlatformID.Unix or PlatformID.MacOSX => new CursesConsole(),
                _ => new DefaultNetConsole()
            };
#pragma warning restore CA1416

            return builder.UseConsole(console).UseAutoDetectConsoleColorMode();
        }

        public static AppBuilder UseAutoDetectConsoleColorMode(this AppBuilder builder)
        {
            IConsoleColorMode result;
            if (Design.IsDesignMode)
                result = new RgbConsoleColorMode();
            else
                switch (Environment.OSVersion.Platform)
                {
                    case PlatformID.Win32S or PlatformID.Win32Windows or PlatformID.Win32NT:
                    case PlatformID.MacOSX:
                        result = new RgbConsoleColorMode();
                        break;
                    case PlatformID.Unix:
                        string term = Environment.GetEnvironmentVariable("TERM");
                        result = term switch
                        {
                            "linux" or "xterm-direct" or "xterm-color" => new EgaConsoleColorMode(),
                            "xterm-256color" or "screen-256color" or "tmux-256color" => new RgbConsoleColorMode(),
                            _ => new EgaConsoleColorMode()
                        };
                        break;
                    default:
                        result = new EgaConsoleColorMode();
                        break;
                }

            return builder.UseConsoleColorMode(result);
        }
    }
}