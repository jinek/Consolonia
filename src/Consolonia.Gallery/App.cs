using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Consolonia.Gallery.View;
using Consolonia.Themes.TurboVision.Templates;

namespace Consolonia.Gallery
{
    internal class App : Application
    {
        public App()
        {
            Styles.Add(new TurboVisionTheme(new Uri("avares://Consolonia.Gallery")));
        }

        public override void OnFrameworkInitializationCompleted()
        {
            var controlsListView = new ControlsListView();
            ((IClassicDesktopStyleApplicationLifetime)ApplicationLifetime).MainWindow = controlsListView;
            base.OnFrameworkInitializationCompleted();
            MainWindowSingleton = controlsListView;
        }
        
        internal static Window MainWindowSingleton { get; private set; }
    }
}