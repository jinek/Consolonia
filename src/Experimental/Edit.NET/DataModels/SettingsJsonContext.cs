using System.Text.Json.Serialization;
using TextMateSharp.Grammars;

namespace EditNET.DataModels
{
    [JsonSerializable(typeof(Settings))]
    [JsonSourceGenerationOptions(WriteIndented = true,
        GenerationMode = JsonSourceGenerationMode.Default,
        Converters = [typeof(JsonStringEnumConverter<ThemeName>), typeof(JsonStringEnumConverter<ConsoloniaTheme>)])]
    internal partial class SettingsJsonContext : JsonSerializerContext;
}