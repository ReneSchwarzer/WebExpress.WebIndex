using System.Text.Json;
using System.Text.Json.Serialization;
using WebExpress.WebIndex.WiUI.Model;

namespace WebExpress.WebIndex.WiUI.Converters
{
    /// <summary>
    /// Custom converter for field typ to string.
    /// </summary>
    internal class FieldTypeConverter : JsonConverter<FieldType>
    {
        /// <summary>
        /// Converts a string to an int.
        /// </summary>
        public override FieldType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return FieldTypeExtention.FromStringValue(reader.GetString()!);
        }

        /// <summary>
        /// Converts an int to a string.
        /// </summary>
        public override void Write(Utf8JsonWriter writer, FieldType value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
