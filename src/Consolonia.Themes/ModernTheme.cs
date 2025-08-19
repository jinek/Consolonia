using System;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace Consolonia.Themes
{
    public class ModernTheme : Styles
    {
        public ModernTheme(IServiceProvider sp = null)
        {
            AvaloniaXamlLoader.Load(sp, this);
        }
    }

    [Obsolete("FluentTheme is deprecated. Use ModernTheme instead.")]
    public class FluentTheme : ModernTheme
    { 
    }
}