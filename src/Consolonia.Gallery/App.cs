using System.Globalization;
using System.Threading;
using Consolonia.Core.Infrastructure;
using Consolonia.Gallery.View;
using Consolonia.Themes;

namespace Consolonia.Gallery
{
    internal class App : ConsoloniaApplication<ControlsListView>
    {
        static App()
        {
            // we want tests and UI to be executed with same culture
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }

        public App()
        {
            
        }

        public override void OnFrameworkInitializationCompleted()
        {
            /*Styles.Add(new TurboVisionBlackTheme());*/
            /*Styles.Add(new TurboVisionDarkTheme());*/
            /*Styles.Add(new FluentTheme());*/

            if(((ConsoloniaLifetime)ApplicationLifetime).IsRgbColorMode())
                Styles.Add(new MaterialTheme());
            else
                Styles.Add(new TurboVisionTheme());
            
            base.OnFrameworkInitializationCompleted();
        }
    }
}