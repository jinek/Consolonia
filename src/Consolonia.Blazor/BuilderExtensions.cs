using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Blazonia;
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
                            (ConsoloniaLifetime)Application.Current?.ApplicationLifetime;
                        ArgumentNullException.ThrowIfNull(lifetime);
                        return lifetime;
                    });
                    sc.AddSingleton(sp =>
                        (ISingleViewApplicationLifetime)sp.GetRequiredService<ConsoloniaLifetime>()
                    );
                    sc.AddSingleton(sp =>
                        (IControlledApplicationLifetime)sp.GetRequiredService<ConsoloniaLifetime>()
                    );
                    sc.AddTransient(sp =>
                        sp.GetRequiredService<ConsoloniaLifetime>().TopLevel?.StorageProvider);
                    sc.AddTransient(sp =>
                        sp.GetRequiredService<ConsoloniaLifetime>().TopLevel?.Clipboard);
                    sc.AddTransient(sp =>
                        sp.GetRequiredService<ConsoloniaLifetime>().TopLevel?.InsetsManager);
                    sc.AddTransient(sp =>
                        sp.GetRequiredService<ConsoloniaLifetime>().TopLevel?.InputPane);
                    sc.AddTransient(sp =>
                        sp.GetRequiredService<ConsoloniaLifetime>().TopLevel?.Launcher);
                    sc.AddTransient(sp =>
                        sp.GetRequiredService<ConsoloniaLifetime>().TopLevel?.Screens);
                    sc.AddTransient(sp =>
                        sp.GetRequiredService<ConsoloniaLifetime>().TopLevel?.FocusManager);
                    sc.AddTransient(sp =>
                        sp.GetRequiredService<ConsoloniaLifetime>().TopLevel?.PlatformSettings);

                    if (configureServices != null) configureServices(sc);
                });
        }
    }
}