using System;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace Consolonia.Themes
{
    /// <summary>
    ///     Dark theme based on Turbo Vision templates. Compatible with 16-fg / 8-bg ANSI
    /// </summary>
    public class TurboVisionDarkTheme : Styles
    {
        public TurboVisionDarkTheme(IServiceProvider sp = null)
        {
            AvaloniaXamlLoader.Load(sp, this);
        }
    }
}