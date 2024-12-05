using Consolonia.Core.Infrastructure;
using Consolonia.Themes.TurboVision.Themes;
using Example.Views;

namespace Example
{
    // ReSharper disable once PartialTypeWithSinglePart
    public partial class App : ConsoloniaApplication<DataGridTestWindow>
    {
        public App()
        {
            Styles.Add(new TurboVisionTheme());
            Styles.Add(new TurboVisionBlackTheme());
            Styles.Add(new TurboVisionBlackTheme());
            Styles.Add(new TurboVisionBlackTheme());
            // Styles.Add(new FluentTheme());
            // Styles.Add(new TurboVisionTheme());
        }
    }
}