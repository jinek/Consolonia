using System;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace Consolonia.Themes
{
    /// <summary>
    ///     Turbo Vision theme Blue. Compatible with 3bit background, but expands to 4bit backgrounds
    /// </summary>
    public class TurboVisionElegantTheme : Styles
    {
        public TurboVisionElegantTheme(IServiceProvider sp = null)
        {
            AvaloniaXamlLoader.Load(sp, this);
        }
    }
}