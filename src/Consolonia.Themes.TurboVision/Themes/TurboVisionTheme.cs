using System;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace Consolonia.Themes
{
    public class TurboVisionTheme : Styles
    {
        public TurboVisionTheme(IServiceProvider sp = null)
        {
            AvaloniaXamlLoader.Load(sp, this);
        }
    }
}