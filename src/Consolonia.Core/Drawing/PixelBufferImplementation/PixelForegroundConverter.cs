using System;
using Avalonia.Media;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Consolonia.Core.Drawing.PixelForegroundImplementation
{
    public class PixelForegroundConverter : JsonConverter<PixelForeground>
    {
        public override PixelForeground ReadJson(JsonReader reader, Type objectType, PixelForeground existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            JObject jObject = JObject.Load(reader);
            JsonReader rdr = jObject.CreateReader()!;

            // read in symbol from object in way that respects its converter
            Symbol symbol = serializer.Deserialize<Symbol>(rdr);
            string? weightStr = jObject[nameof(PixelForeground.Weight)]!.Value<string>()!;
            string? styleStr = jObject[nameof(PixelForeground.Style)]!.Value<string>()!;

            return new PixelForeground(
                symbol: symbol,
                color: Color.Parse(jObject[nameof(PixelForeground.Color)]!.Value<string>()),
                weight: (!String.IsNullOrEmpty(weightStr)) ? Enum.Parse<FontWeight>(weightStr) : null,
                style: (!String.IsNullOrEmpty(styleStr)) ? Enum.Parse<FontStyle>(styleStr) : null
                );
        }

        public override void WriteJson(JsonWriter writer, PixelForeground value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(nameof(PixelForeground.Symbol));
            serializer.Serialize(writer, value.Symbol);
            if (value.Weight != null)
            {
                writer.WritePropertyName(nameof(PixelForeground.Weight));
                writer.WriteValue(value.Weight.ToString());
            }
            if (value.Style != null)
            {
                writer.WritePropertyName(nameof(PixelForeground.Style));
                writer.WriteValue(value.Style.ToString());
            }
            if (value.TextDecoration != null)
            {
                writer.WritePropertyName(nameof(PixelForeground.TextDecoration));
                writer.WriteValue(value.TextDecoration.ToString());
            }
            writer.WriteEndObject();
        }
    }
}
