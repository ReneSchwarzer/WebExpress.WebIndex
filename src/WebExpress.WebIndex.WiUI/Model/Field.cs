namespace WebExpress.WebIndex.WiUI.Model
{
    /// <summary>
    /// Represents an index field.
    /// </summary>
    public class Field
    {
        /// <summary>
        /// Returns or sets the name of the field.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Returns or sets the type of the field.
        /// </summary>
        public FieldType Type { get; set; }

        /// <summary>
        /// Returns or sets the ignore attribute.
        /// </summary>
        public bool Ignore { get; set; }

        /// <summary>
        /// Returns or sets if the field is abstract.
        /// </summary>
        public bool Abstract { get; set; }
    }
}
