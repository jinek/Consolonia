using System;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace Consolonia.Themes
{
    /// <summary>
    ///     Modern theme with black background and high contrast color scheme
    /// </summary>
    public class ModernContrastTheme : Styles
    {
        public ModernContrastTheme(IServiceProvider sp = null)
        {
            AvaloniaXamlLoader.Load(sp, this);
        }
    }
}