using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Consolonia;
using Consolonia.Themes;
using Example.Views;

namespace Example
{
    // ReSharper disable once PartialTypeWithSinglePart
    public partial class App : Application
    {
        public App()
        {
            //Styles.Add(new TurboVisionTheme());
            //Styles.Add(new TurboVisionBlackTheme());
            //Styles.Add(new TurboVisionDarkTheme());
            //Styles.Add(new FluentTheme());
            Styles.Add(new MaterialTheme());
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new DataGridTestWindow();
                base.OnFrameworkInitializationCompleted();

                this.InitializeConsolonia();
            }
        }
    }
}