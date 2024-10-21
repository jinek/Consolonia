using System;
using Consolonia.Core.Styles;

namespace Consolonia.Themes.TurboVision.Themes.TurboVisionDark
{
    // ReSharper disable once UnusedType.Global
    public class TurboVisionDarkTheme : ResourceIncludeBase
    {
        public TurboVisionDarkTheme(Uri baseUri) : base(baseUri)
        {
        }

        public TurboVisionDarkTheme(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override Uri Uri =>
            new(@"avares://Consolonia.Themes.TurboVision/Themes/TurboVisionDark/TurboVisionDark.axaml");
    }
}