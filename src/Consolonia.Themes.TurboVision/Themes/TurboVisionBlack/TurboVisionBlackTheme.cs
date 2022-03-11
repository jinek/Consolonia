using System;
using Consolonia.Core.Styles;
using JetBrains.Annotations;

namespace Consolonia.Themes.TurboVision.Themes.TurboVisionBlack
{
    [UsedImplicitly]
    public class TurboVisionBlackTheme : ResourceIncludeBase
    {
        public TurboVisionBlackTheme(Uri baseUri) : base(baseUri)
        {
        }

        public TurboVisionBlackTheme(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override Uri Uri =>
            new(@"avares://Consolonia.Themes.TurboVision/Themes/TurboVisionBlack/TurboVisionBlack.axaml");
    }
}