using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input.Platform;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Consolonia.Core.Drawing.PixelBufferImplementation.EgaConsoleColor;
using Consolonia.Core.Dummy;
using Consolonia.Core.Infrastructure;
using Consolonia.PlatformSupport;
using Consolonia.PlatformSupport.Clipboard;

// ReSharper disable CheckNamespace
#pragma warning disable IDE0161
#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Consolonia
#pragma warning restore IDE0130 // Namespace does not match folder structure
#pragma warning restore IDE0161
{
    public static class PlatformSupportExtensions
    {
        public static AppBuilder UseAutoDetectedConsole(this AppBuilder builder)
        {
            if (Design.IsDesignMode)
                // in design mode we can't use any console operations at all, so we use a dummy IConsole.
                return builder.UseConsole(new DummyConsole());

            IConsole console = Environment.OSVersion.Platform switch
            {
#pragma warning disable CA1416 // Validate platform compatibility
                PlatformID.Win32S or PlatformID.Win32Windows or PlatformID.Win32NT =>
                    new Win32Console(Console.IsOutputRedirected || IsWindowsTerminal()
                        ? new AnsiConsoleOutput()
                        : new WindowsLegacyConsoleOutput()),
#pragma warning restore CA1416 // Validate platform compatibility
                PlatformID.Unix or PlatformID.MacOSX => new CursesConsole(),
                _ => new DefaultNetConsole()
            };

            return builder.UseConsole(console)
                .UseAutoDetectClipboard()
                .UseAutoDetectConsoleColorMode();
        }


        /// <summary>Provides cut, copy, and paste support for the OS clipboard.</summary>
        /// <remarks>
        ///     <para>On Windows, we use the Avalonia Windows Clipboard .</para>
        ///     <para>
        ///         On Linux, when not running under Windows Subsystem for Linux (WSL), we use X11Clipboard to call X11 PInvoke
        ///         calls.
        ///     </para>
        ///     <para>
        ///         On Linux, when running under Windows Subsystem for Linux (WSL), we use WslClipboard class launches
        ///         Windows' powershell.exe via WSL interop and uses the "Set-Clipboard" and "Get-Clipboard" Powershell CmdLets.
        ///     </para>
        ///     <para>
        ///         On the Mac, we use MacClipboard class which uses the MacOS X pbcopy and pbpaste command line tools and
        ///         the Mac clipboard APIs vai P/Invoke.
        ///     </para>
        /// </remarks>
        public static AppBuilder UseAutoDetectClipboard(this AppBuilder builder)
        {
            if (OperatingSystem.IsWindows()) return builder.With<IClipboard>(new Win32Clipboard());

            if (OperatingSystem.IsMacOS()) return builder.With<IClipboard>(new MacClipboard());

            if (OperatingSystem.IsLinux())
            {
                if (IsWslPlatform())
                    return builder.With<IClipboard>(new WslClipboard());
                // alternatively use xclip CLI tool
                //return builder.With<IClipboard>(new XClipClipboard());
                return builder.With<IClipboard>(new X11Clipboard());
            }

            return builder.With<IClipboard>(new InprocessClipboard());
        }

        public static bool IsWslPlatform()
        {
            return !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WSL_DISTRO_NAME"));
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
                    {
                        // if output is redirected, or we are a windows terminal we use the win32 ANSI based console.
                        if (Console.IsOutputRedirected || IsWindowsTerminal())
                            result = new RgbConsoleColorMode();
                        else
                            result = new EgaConsoleColorMode();
                    }
                        break;
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

        private static bool IsWindowsTerminal()
        {
            return Environment.GetEnvironmentVariable("WT_SESSION") is not null ||
                   Environment.GetEnvironmentVariable("VSAPPIDNAME") != null;
        }
    }
}