using Avalonia;
using Avalonia.Markup.Xaml;
using BlazorBindingsAvalonia;
using Example.Blazor.Components;

namespace Example.Blazor
{


    public class App : BlazorBindingsApplication<MainPage>
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}