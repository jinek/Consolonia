using System;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace Consolonia.Themes.TurboVision.Themes.TurboVisionBlack
{
    public class TurboVisionBlackTheme : Styles
    {
        public TurboVisionBlackTheme(IServiceProvider sp = null)
        {
            AvaloniaXamlLoader.Load(sp, this);
        }
    }
}