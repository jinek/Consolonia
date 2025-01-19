using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.PreviewHost.ViewModels;
using Avalonia.PreviewHost.Views;
using Consolonia;
using Consolonia.Designer;

namespace Avalonia.PreviewHost
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            // Line below is needed to remove Avalonia data validation.
            // Without this line you will get duplicate validations from both Avalonia and CT
            BindingPlugins.DataValidators.RemoveAt(0);

            if (ApplicationLifetime is ConsoloniaLifetime lifetime)
            {
                lifetime.TopLevel = new MainWindow
                {
                    DataContext = new MainViewModel()
                };
                lifetime.Exit += Desktop_Exit;
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void Desktop_Exit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
        {
            var desktop = ApplicationLifetime as ISingleViewApplicationLifetime;

            desktop!.MainWindow!.FindControl<ConsolePreview>("PreviewPane")?.Dispose();
        }
    }
}