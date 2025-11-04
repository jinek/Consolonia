using System.Text.Json.Serialization;

namespace EditNET.DataModels
{
    [JsonSerializable(typeof(Settings))]
    [JsonSourceGenerationOptions(WriteIndented = true,
        GenerationMode = JsonSourceGenerationMode.Default,
        Converters = [typeof(JsonStringEnumConverter)])]
    internal partial class SettingsJsonContext : JsonSerializerContext;
}