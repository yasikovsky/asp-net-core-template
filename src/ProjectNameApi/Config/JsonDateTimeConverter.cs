using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using ProjectNameApi.Extensions;

namespace ProjectNameApi.Config
{
    /// <summary>
    ///     JSON converter for parsing and converting date format to ISO (yyyy-MM-dd HH:mm:ss)
    /// </summary>
    public class JsonDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Debug.Assert(typeToConvert == typeof(DateTime));
            return DateTime.Parse(reader.GetString() ?? string.Empty);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToISODateTimeFull());
        }
    }
}