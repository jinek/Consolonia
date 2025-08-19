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

    // TODO: Remove this class when FluentTheme is no longer needed.
    public class FluentTheme : ModernTheme
    { 
    }
}