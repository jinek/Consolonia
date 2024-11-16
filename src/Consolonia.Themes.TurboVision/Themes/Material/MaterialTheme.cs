using System;
using System.Diagnostics.CodeAnalysis;
using Consolonia.Core.Styles;

namespace Consolonia.Themes.TurboVision.Themes.TurboVisionBlack
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class MaterialTheme : ResourceIncludeBase
    {
        public MaterialTheme(Uri baseUri) : base(baseUri)
        {
        }

        public MaterialTheme(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override Uri Uri =>
            new("avares://Consolonia.Themes.TurboVision/Themes/Material/Material.axaml");
    }
}