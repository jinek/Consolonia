using System;
using System.Diagnostics.CodeAnalysis;
using Consolonia.Core.Styles;

namespace Consolonia.Themes
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class TurboVisionTheme : ResourceIncludeBase
    {
        public TurboVisionTheme() : base(new Uri("avares://Consolonia.Themes.TurboVision/Templates/TurboVision.axaml"))
        {
        }

        public TurboVisionTheme(Uri baseUri) : base(baseUri)
        {
        }

        public TurboVisionTheme(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override Uri Uri => new("avares://Consolonia.Themes.TurboVision/Templates/TurboVision.axaml");
    }
}