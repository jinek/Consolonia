using System;
using System.Diagnostics.CodeAnalysis;
using Consolonia.Core.Styles;

namespace Consolonia.Themes.TurboVision.Themes.TurboVisionBlack
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
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