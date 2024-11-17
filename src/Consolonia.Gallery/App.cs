using System;
using System.Globalization;
using System.Threading;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Styling;
using Consolonia.Core.Infrastructure;
using Consolonia.Gallery.View;
using Consolonia.Themes.TurboVision.Themes.TurboVisionBlack;

namespace Consolonia.Gallery
{
    internal class App : ConsoloniaApplication
    {
        static App()
        {
            // we want tests and UI to be executed with same culture
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }

        public App()
        {
            // Styles.Add(new TurboVisionTheme(new Uri("avares://Consolonia.Themes.TurboVision/Themes/TurboVisionDark/TurboVisionDark.axaml")));
            // Styles.Add(new MaterialTheme(new Uri("avares://Consolonia.Themes.TurboVision/Themes/Material/Material.axaml")));
            Styles.Add(new MaterialTheme(new Uri("avares://Consolonia.Themes.TurboVision/Themes/Fluent/Fluent.axaml")));
        }

        public override void OnFrameworkInitializationCompleted()
        {
            var lifetime = ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            if (lifetime != null)
                lifetime.MainWindow = new ControlsListView();

            base.OnFrameworkInitializationCompleted();
        }
    }
}