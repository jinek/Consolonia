using System;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace Consolonia.Themes
{
    /// <summary>
    ///     Turbo Vision Dark variant. Compatible with 3bit background, but expands to 4bit backgrounds 
    /// </summary>
    public class TurboVisionCompatibleTheme : Styles
    {
        public TurboVisionCompatibleTheme(IServiceProvider sp = null)
        {
            AvaloniaXamlLoader.Load(sp, this);
        }
    }
}