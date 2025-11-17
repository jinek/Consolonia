using System;
using Consolonia.Core.Text.Fonts;

namespace Consolonia.Fonts
{
    public class ConsoleFontCollection : EmbeddedConsoleFontCollection
    {
        public ConsoleFontCollection()
            : base(new Uri("fonts:Consolonia"), new Uri("avares://Consolonia.Fonts/Fonts/"))
        {
        }
    }
}