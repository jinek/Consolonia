using System;
using Avalonia.Media;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    public class PixelBackgroundConverter : JsonConverter<PixelBackground>
    {
        public override PixelBackground ReadJson(JsonReader reader, Type objectType, PixelBackground existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            JToken jToken = JToken.Load(reader);
            var color = jToken.Value<string>();
            return new PixelBackground(Color.Parse(color));
        }

        public override void WriteJson(JsonWriter writer, PixelBackground value, JsonSerializer serializer)
        {
            writer.WriteValue(value.Color.ToString());
        }
    }
}