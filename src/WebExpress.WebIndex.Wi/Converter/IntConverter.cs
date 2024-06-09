using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebExpress.WebIndex.Wi.Converter
{
    /// <summary>
    /// Custom converter for int to string.
    /// </summary>
    internal class IntConverter : JsonConverter<int>
    {
        /// <summary>
        /// Converts a string to an int.
        /// </summary>
        public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return int.Parse(reader.GetString());
        }

        /// <summary>
        /// Converts an int to a string.
        /// </summary>
        public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
