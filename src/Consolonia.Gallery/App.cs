using System;
using Avalonia.Controls.ApplicationLifetimes;
using Consolonia.Core.Infrastructure;
using Consolonia.Gallery.View;
using Consolonia.Themes.TurboVision.Templates;

namespace Consolonia.Gallery
{
    internal class App : ConsoloniaApplication
    {
        public App()
        {
            Styles.Add(new TurboVisionTheme(new Uri("avares://Consolonia.Gallery")));
        }

        public override void OnFrameworkInitializationCompleted()
        {
            ((IClassicDesktopStyleApplicationLifetime)ApplicationLifetime)!.MainWindow = new ControlsListView();
            base.OnFrameworkInitializationCompleted();
        }
    }
}