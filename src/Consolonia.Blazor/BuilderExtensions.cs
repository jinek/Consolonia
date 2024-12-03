using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Platform;
using Avalonia.Input.Platform;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using Avalonia.Platform;
using BlazorBindingsAvalonia;
using Consolonia.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Consolonia.Blazor
{
    public static class BuilderExtensions
    {
        /// <summary>
        /// Use Consolonize in Blazor mode
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureServices"></param>
        /// <returns></returns>
        public static AppBuilder UseConsoloniaBlazor(this AppBuilder builder, Action<IServiceCollection> configureServices = null)
        {
            return builder
                .UseConsolonia()
                .UseAvaloniaBlazorBindings((sp) =>
                {
                    // Register services for injectoin
                    sp.AddSingleton((sp) => Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime);
                    sp.AddTransient((sp) => sp.GetService<IClassicDesktopStyleApplicationLifetime>().MainWindow.StorageProvider);
                    sp.AddTransient((sp) => sp.GetService<IClassicDesktopStyleApplicationLifetime>().MainWindow.Clipboard);
                    sp.AddTransient((sp) => sp.GetService<IClassicDesktopStyleApplicationLifetime>().MainWindow.InsetsManager);
                    sp.AddTransient((sp) => sp.GetService<IClassicDesktopStyleApplicationLifetime>().MainWindow.InputPane);
                    sp.AddTransient((sp) => sp.GetService<IClassicDesktopStyleApplicationLifetime>().MainWindow.Launcher);
                    sp.AddTransient((sp) => sp.GetService<IClassicDesktopStyleApplicationLifetime>().MainWindow.Screens);
                    sp.AddTransient((sp) => sp.GetService<IClassicDesktopStyleApplicationLifetime>().MainWindow.FocusManager);
                    sp.AddTransient((sp) => sp.GetService<IClassicDesktopStyleApplicationLifetime>().MainWindow.PlatformSettings);

                    if (configureServices != null)
                    {
                        configureServices(sp);
                    }
                });
        }
    }
}
