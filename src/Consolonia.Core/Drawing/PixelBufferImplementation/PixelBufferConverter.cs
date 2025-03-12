using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    public class PixelBufferConverter : JsonConverter<PixelBuffer>
    {
        public override PixelBuffer ReadJson(JsonReader reader, Type objectType, PixelBuffer existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;

            JObject jObject = JObject.Load(reader);
            var size = jObject[nameof(PixelBuffer.Size)]!.Value<PixelBufferSize>();
            var position = jObject[nameof(PixelBuffer.Position)]!.Value<PixelBufferCoordinate>();
            var pixelBuffer = new PixelBuffer(position, size);
            JToken pixels = jObject["Pixels"];
            ArgumentNullException.ThrowIfNull(pixels);
            int i = 0;
            for (ushort y = 0; y < size.Height; y++)
            for (ushort x = 0; x < size.Width; x++)
            {
                JToken pixelRecord = pixels[i++];
                ArgumentNullException.ThrowIfNull(pixelRecord);
                JsonReader rdr = pixelRecord.CreateReader()!;
                ArgumentNullException.ThrowIfNull(rdr);
                var pixel = serializer.Deserialize<Pixel>(rdr);
                ArgumentNullException.ThrowIfNull(pixel);
                pixelBuffer[x, y] = pixel;
            }

            return pixelBuffer;
        }

        public override void WriteJson(JsonWriter writer, PixelBuffer value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(nameof(PixelBuffer.Size));
            writer.WriteValue(value.Size);
            writer.WritePropertyName(nameof(PixelBuffer.Position));
            writer.WriteValue(value.Position);
            writer.WritePropertyName("Pixels");
            writer.WriteStartArray();
            for (ushort y = 0; y < value.Size.Height; y++)
            for (ushort x = 0; x < value.Size.Width; x++)
                serializer.Serialize(writer, value[x, y]);
            writer.WriteEndArray();
            writer.WriteEndObject();
        }
    }
}