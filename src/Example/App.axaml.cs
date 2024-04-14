using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Consolonia.Core.Infrastructure;
using Example.Views;

namespace Example
{
    public partial class App : ConsoloniaApplication
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                desktop.MainWindow = new DataGridTestWindow();

            base.OnFrameworkInitializationCompleted();
        }
    }
}