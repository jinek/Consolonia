using System;
using System.Diagnostics.CodeAnalysis;
using Consolonia.Core.Styles;

namespace Consolonia.Themes
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class MaterialTheme : ResourceIncludeBase
    {
        public MaterialTheme() : base(new Uri("avares://Consolonia.Themes.TurboVision/Themes/Material/Material.axaml"))
        {
        }

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