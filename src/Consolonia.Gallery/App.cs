using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml.Styling;
using Consolonia.Themes;

namespace Consolonia.Gallery
{
    internal class App : Application
    {
        internal const string TurboVisionProgramParameterUpperCase = "-TURBOVISION";

        static App()
        {
            // we want tests and UI to be executed with same culture
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (((ConsoloniaLifetime)ApplicationLifetime).IsRgbColorMode()
                && !((ConsoloniaLifetime)ApplicationLifetime).Args.Any(argument => argument != null &&
                    argument.ToUpper().EndsWith(TurboVisionProgramParameterUpperCase)))
                Styles.Add(new ModernTheme());
            else
                Styles.Add(new TurboVisionTheme());

            Styles.Add(new StyleInclude(new Uri("avares://Consolonia.AvaloniaEdit"))
                { Source = new Uri("Theme.axaml", UriKind.Relative) });

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                desktop.MainWindow = new MainWindow(); // designer runs as classic desktop

            base.OnFrameworkInitializationCompleted();
        }
    }
}