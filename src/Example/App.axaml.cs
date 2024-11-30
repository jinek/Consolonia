using Avalonia.Controls.ApplicationLifetimes;
using Consolonia.Core.Infrastructure;
using Example.Views;

namespace Example
{
    // ReSharper disable once PartialTypeWithSinglePart
    public partial class App : ConsoloniaApplication
    {
        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                desktop.MainWindow = new DataGridTestWindow();

            base.OnFrameworkInitializationCompleted();
        }
    }
}