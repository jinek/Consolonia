using System.Globalization;
using System.Threading;
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
            /*Styles.Add(new TurboVisionBlackTheme());*/
            /*Styles.Add(new TurboVisionDarkTheme());*/
            
            Styles.Add(new FluentTheme());
            //todo: automatically switch to turbovision if only 16 colors are supported
            /*Styles.Add(new TurboVisionTheme());*/
            /*Styles.Add(new MaterialTheme());*/
        }
    }
}