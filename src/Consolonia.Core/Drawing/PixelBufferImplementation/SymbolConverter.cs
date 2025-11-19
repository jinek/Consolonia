using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    public class SymbolConverter : JsonConverter<Symbol>
    {
        public override Symbol Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string value = reader.GetString();
                return value != null ? new Symbol(value) : Symbol.Empty;
            }

            if (reader.TokenType == JsonTokenType.Number)
                if (reader.TryGetInt64(out long value))
                    return new Symbol((byte)value);

            return Symbol.Empty;
        }

        public override void Write(Utf8JsonWriter writer, Symbol value, JsonSerializerOptions options)
        {
            if (value.IsBoxSymbol())
                writer.WriteNumberValue(value.Pattern);
            else if (!string.IsNullOrEmpty(value.Complex))
                writer.WriteStringValue(value.Complex);
            else if (value.Character != char.MinValue)
                writer.WriteStringValue(value.Character.ToString());
            else
                writer.WriteNullValue();
        }
    }
}