using System;
using Newtonsoft.Json;

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    public class SymbolConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsAssignableTo(typeof(ISymbol));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.ValueType == typeof(string))
                return new SimpleSymbol((string)reader.Value);
            else if (reader.ValueType == typeof(long))
                return new DrawingBoxSymbol((byte)(long)reader.Value);
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is DrawingBoxSymbol db)
                writer.WriteValue(db.UpRightDownLeft);
            else if (value is SimpleSymbol ss)
                writer.WriteValue(ss.Text);
        }
    }
}
