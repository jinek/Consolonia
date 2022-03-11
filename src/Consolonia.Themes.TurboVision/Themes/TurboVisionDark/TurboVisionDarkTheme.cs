using System;
using Consolonia.Core.Styles;
using JetBrains.Annotations;

namespace Consolonia.Themes.TurboVision.Themes.TurboVisionDark
{
    [UsedImplicitly]
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