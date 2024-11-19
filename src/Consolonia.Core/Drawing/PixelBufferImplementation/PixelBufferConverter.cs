using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    public class PixelBufferConverter : JsonConverter<PixelBuffer>
    {

        public override PixelBuffer ReadJson(JsonReader reader, Type objectType, PixelBuffer existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var jObject = JObject.Load(reader);
            var width = jObject[nameof(PixelBuffer.Width)]!.Value<ushort>();
            var height = jObject[nameof(PixelBuffer.Height)]!.Value<ushort>();
            var pixelBuffer = new PixelBuffer(width, height);
            var pixels = jObject["Pixels"];
            ArgumentNullException.ThrowIfNull(pixels);
            int i = 0;
            for (ushort y = 0; y < height; y++)
            {
                for (ushort x = 0; x < width; x++)
                {
                    var pixelRecord = pixels[i++];
                    ArgumentNullException.ThrowIfNull(pixelRecord);
                    var rdr = pixelRecord.CreateReader()!;
                    ArgumentNullException.ThrowIfNull(rdr);
                    var pixel = serializer.Deserialize<Pixel>(rdr);
                    ArgumentNullException.ThrowIfNull(pixel);
                    pixelBuffer[x, y] = pixel;
                }
            }

            return pixelBuffer;

        }

        public override void WriteJson(JsonWriter writer, PixelBuffer value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(nameof(PixelBuffer.Width));
            writer.WriteValue(value.Width);
            writer.WritePropertyName(nameof(PixelBuffer.Height));
            writer.WriteValue(value.Height);
            writer.WritePropertyName("Pixels");
            writer.WriteStartArray();
            for (ushort y = 0; y < value.Height; y++)
            {
                for (ushort x = 0; x < value.Width; x++)
                {
                    serializer.Serialize(writer, value[x, y]);
                }
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
        }
    }
}
