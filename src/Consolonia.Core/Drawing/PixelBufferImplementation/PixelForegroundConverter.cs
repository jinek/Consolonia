using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Avalonia.Media;

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    public class PixelForegroundConverter : JsonConverter<PixelForeground>
    {
        public override PixelForeground Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();

            Symbol symbol = Symbol.Empty;
            Color color = Colors.Transparent;
            FontWeight? weight = null;
            FontStyle? style = null;
            TextDecorationLocation? textDecoration = null;

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
                        case "Symbol":
                            symbol = JsonSerializer.Deserialize<Symbol>(ref reader, options);
                            break;
                        case "Color":
                            string colorStr = reader.GetString();
                            if (colorStr != null && Color.TryParse(colorStr, out Color parsedColor))
                                color = parsedColor;
                            break;
                        case "Weight":
                            if (reader.TokenType != JsonTokenType.Null)
                            {
                                string weightStr = reader.GetString();
                                if (Enum.TryParse<FontWeight>(weightStr, out FontWeight parsedWeight))
                                    weight = parsedWeight;
                            }
                            break;
                        case "Style":
                            if (reader.TokenType != JsonTokenType.Null)
                            {
                                string styleStr = reader.GetString();
                                if (Enum.TryParse<FontStyle>(styleStr, out FontStyle parsedStyle))
                                    style = parsedStyle;
                            }
                            break;
                        case "TextDecoration":
                            if (reader.TokenType != JsonTokenType.Null)
                            {
                                string decorationStr = reader.GetString();
                                if (Enum.TryParse<TextDecorationLocation>(decorationStr, out TextDecorationLocation parsedDecoration))
                                    textDecoration = parsedDecoration;
                            }
                            break;
                    }
                }
            }

            return new PixelForeground(symbol, color, weight, style, textDecoration);
        }

        public override void Write(Utf8JsonWriter writer, PixelForeground value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            
            writer.WritePropertyName("Symbol");
            JsonSerializer.Serialize(writer, value.Symbol, options);
            
            writer.WriteString("Color", value.Color.ToString());
            
            if (value.Weight.HasValue)
                writer.WriteString("Weight", value.Weight.Value.ToString());
            
            if (value.Style.HasValue)
                writer.WriteString("Style", value.Style.Value.ToString());
            
            if (value.TextDecoration.HasValue)
                writer.WriteString("TextDecoration", value.TextDecoration.Value.ToString());
            
            writer.WriteEndObject();
        }
    }
}
