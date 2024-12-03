using Avalonia.Input;
using Avalonia;
using BlazorBindingsAvalonia;
using Consolonia.Core.Helpers;
using Consolonia.Core.Infrastructure;
using Microsoft.AspNetCore.Components;
using Avalonia.Platform;

namespace Consolonia.Blazor
{
    /// <summary>
    /// Use this application as your app base class to use Consolonia.Blazor Blazor engine.
    /// </summary>
    /// <typeparam name="TComponent">The root window for the application.</typeparam>
    public class ConsoloniaBlazorApplication<TComponent> : BlazorBindingsApplication<TComponent>
        where TComponent : IComponent
    {
        public override void RegisterServices()
        {
            base.RegisterServices();

            AvaloniaLocator.CurrentMutable.Bind<IKeyboardNavigationHandler>()
                .ToTransient<ArrowsAndKeyboardNavigationHandler>();
        }

        public override void OnFrameworkInitializationCompleted()
        {
            base.OnFrameworkInitializationCompleted();

            this.AddConsoloniaDesignMode();
        }
    }
}
