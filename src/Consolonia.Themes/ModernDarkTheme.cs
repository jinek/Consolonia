using System;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace Consolonia.Themes
{
    /// <summary>
    ///     Modern theme with dark background palette and light colors on top
    /// </summary>
    public class ModernDarkTheme : Styles
    {
        public ModernDarkTheme(IServiceProvider sp = null)
        {
            AvaloniaXamlLoader.Load(sp, this);
        }
    }
}