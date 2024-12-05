using System;
using System.Globalization;
using System.Threading;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Styling;
using Consolonia.Core.Infrastructure;
using Consolonia.Gallery.View;
using Consolonia.Themes.TurboVision.Themes;

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
            // Styles.Add(new TurboVisionTheme());
            Styles.Add(new MaterialTheme());
            // Styles.Add(new FluentTheme());
        }

        public override void RegisterServices()
        {
            base.RegisterServices();
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