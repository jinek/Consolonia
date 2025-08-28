using System;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace Consolonia.Themes
{
    /// <summary>
    ///     A theme that mimics the look and feel of Turbo Vision applications. Compatible with 16-fg / 8-bg ANSI
    /// </summary>
    public class TurboVisionTheme : Styles
    {
        public TurboVisionTheme(IServiceProvider sp = null)
        {
            AvaloniaXamlLoader.Load(sp, this);
        }
    }
}