using System;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace Consolonia.Themes
{
    public class TurboVisionDarkTheme : Styles
    {
        public TurboVisionDarkTheme(IServiceProvider sp = null)
        {
            AvaloniaXamlLoader.Load(sp, this);
        }
    }
}
