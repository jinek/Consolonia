using System.Globalization;
using System.Linq;
using System.Threading;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Consolonia.Gallery.View;
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
            /*Styles.Add(new TurboVisionBlackTheme());*/
            /*Styles.Add(new TurboVisionDarkTheme());*/
            /*Styles.Add(new FluentTheme());*/


            if (((ConsoloniaLifetime)ApplicationLifetime).IsRgbColorMode()
                && !((ConsoloniaLifetime)ApplicationLifetime).Args.Any(argument => argument != null &&
                    argument.ToUpper().EndsWith(TurboVisionProgramParameterUpperCase)))
                Styles.Add(new ModernTheme());
            else
                Styles.Add(new TurboVisionTheme());

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                desktop.MainWindow = new ControlsListView(); // designer runs as classic desktop

            base.OnFrameworkInitializationCompleted();
        }
    }
}