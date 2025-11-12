using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;

namespace Consolonia.Fonts
{
    public static class AppBuilderExtensions
    {
        public static AppBuilder WithConsoleFonts(this AppBuilder appBuilder)
        {
            return appBuilder.ConfigureFonts(fontManager =>
            {
                fontManager.AddFontCollection(new ConsoleFontCollection());
            });
        }
    }
}

