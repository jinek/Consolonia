using System;
using Newtonsoft.Json;

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    public class SymbolConverter : JsonConverter<Symbol>
    {
        public override Symbol ReadJson(JsonReader reader, Type objectType, Symbol existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.Value != null)
            {
                if (reader.ValueType == typeof(string))
                    return new Symbol((string)reader.Value);

                if (reader.ValueType == typeof(long))
                {
                    long value = (long)reader.Value;
                    return new Symbol((byte)value);
                }
            }

            return Symbol.Empty;
        }

        public override void WriteJson(JsonWriter writer, Symbol value, JsonSerializer serializer)
        {
            if (value.IsBoxSymbol())
                writer.WriteValue(value.Pattern);
            else if (!string.IsNullOrEmpty(value.Complex))
                writer.WriteValue(value.Complex);
            else
                writer.WriteValue(value.Character.ToString());
        }
    }
}