using System;
using Avalonia;
using Consolonia.Core;
using Consolonia.Core.Infrastructure;

#pragma warning disable IDE0161
namespace Consolonia.PlatformSupport
#pragma warning restore IDE0161
{
    public static class PlatformSupportExtensions
    {
        public static AppBuilder UseAutoDetectedConsole(this AppBuilder builder)
        {
            IConsole console = Environment.OSVersion.Platform switch
            {
                PlatformID.Win32S or PlatformID.Win32Windows or PlatformID.Win32NT => new Win32Console(),
                PlatformID.Unix or PlatformID.MacOSX => new CursesConsole(),
                _ => new DefaultNetConsole()
            };

            return builder.UseConsole(console);
        }
    }
}