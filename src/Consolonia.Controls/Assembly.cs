using System;
using Avalonia.Metadata;

[assembly: CLSCompliant(false)] //todo: should we make it compliant?

[assembly: XmlnsDefinition("https://github.com/consolonia", "Consolonia.Controls")]
[assembly: XmlnsDefinition("https://github.com/consolonia", "Consolonia.Controls.Brushes")]
[assembly: XmlnsDefinition("https://github.com/consolonia", "Consolonia.Controls.Dialog")]
[assembly: XmlnsDefinition("https://github.com/consolonia", "Consolonia.Controls.Markup")]

// backward compatibility TODO remove when possible
[assembly: XmlnsDefinition("https://github.com/jinek/consolonia", "Consolonia.Controls")]
[assembly: XmlnsDefinition("https://github.com/jinek/consolonia", "Consolonia.Controls.Brushes")]
[assembly: XmlnsDefinition("https://github.com/jinek/consolonia", "Consolonia.Controls.Dialog")]
[assembly: XmlnsDefinition("https://github.com/jinek/consolonia", "Consolonia.Controls.Markup")]