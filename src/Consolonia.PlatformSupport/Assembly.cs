using System;
using System.Runtime.CompilerServices;
using Avalonia.Metadata;

[assembly: InternalsVisibleTo("Consolonia.Core.Tests")]

[assembly: CLSCompliant(false)] //todo: should we make it compliant?

[assembly: XmlnsDefinition("https://github.com/jinek/consolonia", "Consolonia.PlatformSupport")]
[assembly: XmlnsDefinition("https://github.com/jinek/consolonia", "Consolonia.PlatformSupport.Clipboard")]