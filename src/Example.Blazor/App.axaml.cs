using BlazorBindingsAvalonia;
using Consolonia;
using Example.Blazor.Components;

namespace Example.Blazor
{
    public class App : BlazorBindingsApplication<MainPage>
    {
        public override void OnFrameworkInitializationCompleted()
        {
            base.OnFrameworkInitializationCompleted();

            this.InitializeConsolonia();
        }

    }
}