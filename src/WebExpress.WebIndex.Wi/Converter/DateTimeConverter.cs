using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebExpress.WebIndex.Wi.Converter
{
    /// <summary>
    /// Custom converter for DateTime to string.
    /// </summary>
    internal class DateTimeConverter : JsonConverter<DateTime>
    {
        /// <summary>
        /// Converts a string to a DateTime.
        /// </summary>
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.Parse(reader.GetString());
        }

        /// <summary>
        /// Converts a DateTime to a string.
        /// </summary>
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
