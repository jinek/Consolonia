using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Consolonia.Controls;

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    public class PixelConverter : JsonConverter<Pixel>
    {
        public override Pixel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();

            var foreground = PixelForeground.Default;
            PixelBackground background = PixelBackground.Transparent;
            var caretStyle = CaretStyle.None;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    break;

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    string propertyName = reader.GetString();
                    reader.Read();

                    switch (propertyName)
                    {
                        case nameof(Pixel.Foreground):
                            foreground = JsonSerializer.Deserialize<PixelForeground>(ref reader, options);
                            break;
                        case nameof(Pixel.Background):
                            background = JsonSerializer.Deserialize<PixelBackground>(ref reader, options);
                            break;
                        case nameof(Pixel.CaretStyle):
                            if (reader.TokenType == JsonTokenType.String)
                            {
                                string caretStr = reader.GetString();
                                if (Enum.TryParse(caretStr, out CaretStyle parsedCaret))
                                    caretStyle = parsedCaret;
                            }
                            else if (reader.TokenType == JsonTokenType.Number)
                            {
                                caretStyle = (CaretStyle)reader.GetInt32();
                            }

                            break;
                    }
                }
            }

            return new Pixel(foreground, background, caretStyle);
        }

        public override void Write(Utf8JsonWriter writer, Pixel value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WritePropertyName(nameof(Pixel.Foreground));
            JsonSerializer.Serialize(writer, value.Foreground, options);

            writer.WritePropertyName(nameof(Pixel.Background));
            JsonSerializer.Serialize(writer, value.Background, options);

            writer.WritePropertyName(nameof(Pixel.CaretStyle));
            JsonSerializer.Serialize(writer, value.CaretStyle, options);

            writer.WriteEndObject();
        }
    }
}