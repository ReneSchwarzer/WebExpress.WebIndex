namespace WebExpress.WebIndex.Wi.Model
{
    /// <summary>
    /// Represents the types of attributes that can be used.
    /// </summary>
    internal enum AttributeType
    {
        /// <summary>
        /// Attribute type for plain text.
        /// </summary>
        Text,

        /// <summary>
        /// Attribute type for boolean values.
        /// </summary>
        Bool,

        /// <summary>
        /// Attribute type for integer values.
        /// </summary>
        Int,

        /// <summary>
        /// Attribute type for double precision floating point numbers.
        /// </summary>
        Double
    }

    /// <summary>
    /// Provides extension methods for the AttributeType enumeration.
    /// </summary>
    internal static class AttributeTypeExtention
    {
        /// <summary>
        /// Converts the AttributeType value to a corresponding System.Type.
        /// </summary>
        /// <param name="type">The AttributeType value to convert.</param>
        /// <returns>The System.Type that corresponds to the given AttributeType value.</returns>
        public static Type ToType(this AttributeType type)
        {
            return type switch
            {
                AttributeType.Text => typeof(string),
                AttributeType.Bool => typeof(bool),
                AttributeType.Int => typeof(int),
                AttributeType.Double => typeof(double),
                _ => typeof(string)
            };
        }

        /// <summary>
        /// Converts the AttributeType value to a corresponding string representation.
        /// </summary>
        /// <param name="type">The AttributeType value to convert.</param>
        /// <returns>The string representation of the given AttributeType value.</returns>
        public static string ToString(this AttributeType type)
        {
            return type switch
            {
                AttributeType.Text => "Text",
                AttributeType.Bool => "Boolean",
                AttributeType.Int => "Integer",
                AttributeType.Double => "Double",
                _ => "Text"
            };
        }

        /// <summary>
        /// Converts a string to a corresponding AttributeType value.
        /// </summary>
        /// <param name="str">The string to convert.</param>
        /// <returns>The AttributeType value that corresponds to the given string.</returns>
        public static AttributeType FromStringValue(string str)
        {
            return str switch
            {
                "Text" => AttributeType.Text,
                "Boolean" => AttributeType.Bool,
                "Integer" => AttributeType.Int,
                "Double" => AttributeType.Double,
                _ => AttributeType.Text
            };
        }
    }
}
