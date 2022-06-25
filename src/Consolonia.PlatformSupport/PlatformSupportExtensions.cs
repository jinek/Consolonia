// ReSharper disable once CheckNamespace

using System;
using Avalonia.Controls;
using Consolonia.Core;
using Consolonia.Core.Infrastructure;
using Consolonia.Windows;

#pragma warning disable IDE0161
namespace Consolonia.PlatformSupport
#pragma warning restore IDE0161
{
    public static class PlatformSupportExtensions
    {
        public static TAppBuilder UseAutoDetectedConsole<TAppBuilder>(this TAppBuilder builder)
            where TAppBuilder : AppBuilderBase<TAppBuilder>, new()
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