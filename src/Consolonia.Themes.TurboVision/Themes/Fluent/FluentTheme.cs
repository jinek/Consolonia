using System;
using System.Diagnostics.CodeAnalysis;
using Consolonia.Core.Styles;

namespace Consolonia.Themes.TurboVision.Themes.Fluent
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class FluentTheme : ResourceIncludeBase
    {
        public FluentTheme(Uri baseUri) : base(baseUri)
        {
        }

        public FluentTheme(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override Uri Uri =>
            new("avares://Consolonia.Themes.TurboVision/Themes/Fluent/Fluent.axaml");
    }
}