using System;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace Consolonia.Themes
{
    /// <summary>
    ///     Turbo Vision theme identical to TurboVisionDark. Placeholder for future customization.
    /// </summary>
    public class TurboVisionCompatibleTheme : Styles
    {
        public TurboVisionCompatibleTheme(IServiceProvider sp = null)
        {
            AvaloniaXamlLoader.Load(sp, this);
        }
    }
}