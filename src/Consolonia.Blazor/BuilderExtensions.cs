using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
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
                    // register IApplicationLifetime interfaces, making it so you can just @inject do IClassicDesktopStyleApplicationLifetime lifetime;
                    //sp.AddSingleton((sp) => Application.Current.ApplicationLifetime as ISingleViewApplicationLifetime);
                    //sp.AddSingleton((sp) => Application.Current.ApplicationLifetime as IControlledApplicationLifetime);
                    sp.AddSingleton((sp) => Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime);

                    if (configureServices != null)
                    {
                        configureServices(sp);
                    }
                });
        }
    }
}
