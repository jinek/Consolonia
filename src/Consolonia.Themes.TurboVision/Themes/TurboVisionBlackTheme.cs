using System;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace Consolonia.Themes
{
    public class TurboVisionBlackTheme : Styles
    {
        public TurboVisionBlackTheme(IServiceProvider sp = null)
        {
            AvaloniaXamlLoader.Load(sp, this);
        }
    }
}