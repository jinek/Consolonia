using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Consolonia.Gallery.View;
using Consolonia.Themes.TurboVision.Themes.TurboVisionDark;

namespace Consolonia.Gallery
{
    internal class App : Application
    {
        public App()
        {
            Styles.Add(new TurboVisionBlackTheme(new Uri("avares://Consolonia.Gallery")));
        }

        public override void OnFrameworkInitializationCompleted()
        {
            ((IClassicDesktopStyleApplicationLifetime)ApplicationLifetime).MainWindow = new ControlsListView();
            base.OnFrameworkInitializationCompleted();
        }
    }
}