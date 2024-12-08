using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using BlazorBindingsAvalonia;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Consolonia
{
    public static class BuilderExtensions
    {
        /// <summary>
        ///     Use Consolonize in Blazor mode
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureServices"></param>
        /// <returns></returns>
        public static AppBuilder UseConsoloniaBlazor(this AppBuilder builder,
            Action<IServiceCollection> configureServices = null)
        {
            return builder
                .UseConsolonia()
                .UseAvaloniaBlazorBindings(sc =>
                {
                    // Register services for injectoin
                    sc.AddSingleton(_ =>
                    {
                        var lifetime =
                            (IClassicDesktopStyleApplicationLifetime)Application.Current?.ApplicationLifetime;
                        ArgumentNullException.ThrowIfNull(lifetime);
                        return lifetime;
                    });
                    sc.AddTransient(sp =>
                        sp.GetRequiredService<IClassicDesktopStyleApplicationLifetime>().MainWindow?.StorageProvider);
                    sc.AddTransient(sp =>
                        sp.GetRequiredService<IClassicDesktopStyleApplicationLifetime>().MainWindow?.Clipboard);
                    sc.AddTransient(sp =>
                        sp.GetRequiredService<IClassicDesktopStyleApplicationLifetime>().MainWindow?.InsetsManager);
                    sc.AddTransient(sp =>
                        sp.GetRequiredService<IClassicDesktopStyleApplicationLifetime>().MainWindow?.InputPane);
                    sc.AddTransient(sp =>
                        sp.GetRequiredService<IClassicDesktopStyleApplicationLifetime>().MainWindow?.Launcher);
                    sc.AddTransient(sp =>
                        sp.GetRequiredService<IClassicDesktopStyleApplicationLifetime>().MainWindow?.Screens);
                    sc.AddTransient(sp =>
                        sp.GetRequiredService<IClassicDesktopStyleApplicationLifetime>().MainWindow?.FocusManager);
                    sc.AddTransient(sp =>
                        sp.GetRequiredService<IClassicDesktopStyleApplicationLifetime>().MainWindow?.PlatformSettings);

                    if (configureServices != null) configureServices(sc);
                });
        }
    }
}