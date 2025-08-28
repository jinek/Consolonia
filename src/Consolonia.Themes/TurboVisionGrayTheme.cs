using System;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace Consolonia.Themes
{
    /// <summary>
    ///     Turbo Vision Dark Grayed. Compatible with 16-fg / 8-bg ANSI
    /// </summary>
    public class TurboVisionGrayTheme : Styles
    {
        public TurboVisionGrayTheme(IServiceProvider sp = null)
        {
            AvaloniaXamlLoader.Load(sp, this);
        }
    }
}