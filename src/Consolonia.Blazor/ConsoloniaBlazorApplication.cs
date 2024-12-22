using BlazorBindingsAvalonia;
using Consolonia.Core.Helpers;
using Microsoft.AspNetCore.Components;

namespace Consolonia.Blazor
{
    /// <summary>
    ///     Use this application as your app base class to use Consolonia.Blazor Blazor engine.
    /// </summary>
    /// <typeparam name="TComponent">The root window for the application.</typeparam>
    public class ConsoloniaBlazorApplication<TComponent> : BlazorBindingsApplication<TComponent>
        where TComponent : IComponent
    {
        public override void OnFrameworkInitializationCompleted()
        {
            base.OnFrameworkInitializationCompleted();

            this.AddConsoloniaDesignMode();
        }
    }
}