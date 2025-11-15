using System;
using Consolonia.Core.Text.Fonts;

namespace Consolonia.Fonts
{
#pragma warning disable CA1310 // Specify StringComparison for correctness
    public class ConsoleFontCollection : EmbeddedConsoleFontCollection
    {
        public ConsoleFontCollection()
            : base(new Uri("fonts:Consolonia"), new Uri("avares://Consolonia.Fonts/Fonts/"))
        {
        }
    }
}