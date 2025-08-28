using System;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace Consolonia.Themes
{
    /// <summary>
    ///     Turbo Vision theme identical to TurboVisionDark. Placeholder for future customization.
    /// </summary>
    public class TurboVisionElegantTheme : Styles
    {
        public TurboVisionElegantTheme(IServiceProvider sp = null)
        {
            AvaloniaXamlLoader.Load(sp, this);
        }
    }
}