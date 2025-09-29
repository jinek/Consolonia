using Avalonia.Styling;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Edit.NET.DataModels
{
    public class Settings
    {
        public string ConsoloniaTheme { get; set; } = "TurboVisionElegant";

        public bool LightVariant { get; set; }

        public bool ShowTabs { get; set; } 

        public bool ShowSpaces { get; set; } 

        public string DefaultExtension { get; set; } = ".txt";

        public string? SyntaxTheme { get; set; }
    }
}