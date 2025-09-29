using System.Text.Json.Serialization;

namespace Edit.NET.DataModels
{
    [JsonSerializable(typeof(Settings))]
    [JsonSourceGenerationOptions(WriteIndented = true, GenerationMode = JsonSourceGenerationMode.Default)]
    internal partial class SettingsJsonContext : JsonSerializerContext
    {
    }

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