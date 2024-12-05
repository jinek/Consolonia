using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Consolonia.Core.Styles;

namespace Consolonia.Themes.TurboVision.Themes.Fluent
{
    public class FluentTheme : Styles
    {
        public FluentTheme(IServiceProvider sp = null)
        {
            AvaloniaXamlLoader.Load(sp, this);
        }
    }
}