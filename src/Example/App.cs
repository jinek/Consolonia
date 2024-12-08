using Consolonia;
using Consolonia.Themes;
using Example.Views;

namespace Example
{
    // ReSharper disable once PartialTypeWithSinglePart
    public partial class App : ConsoloniaApplication<DataGridTestWindow>
    {
        public App()
        {
            //Styles.Add(new TurboVisionTheme());
            //Styles.Add(new TurboVisionBlackTheme());
            //Styles.Add(new TurboVisionDarkTheme());
            //Styles.Add(new FluentTheme());
            Styles.Add(new MaterialTheme());
        }
    }
}