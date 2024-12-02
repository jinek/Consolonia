using Avalonia;
using Avalonia.Markup.Xaml;
using BlazorBindingsAvalonia;

namespace Example.Blazor
{


    public class App : BlazorBindingsApplication<MainPage>
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            base.OnFrameworkInitializationCompleted();

            this.AttachDevTools();
        }
    }

    //public class BlazorBindingsApplicationMainPage : BlazorBindingsApplication<MainPage>
    //{
    //    //public App()
    //    //{
    //    //    //Styles.Add(new FluentTheme());
    //    //    //Styles.Add(new StyleInclude(XamlIlRuntimeHelpers.CreateRootServiceProviderV3(null))//(Uri?)null)
    //    //    //{
    //    //    //    Source = new Uri("avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml")
    //    //    //});
    //    //}
    //}
}