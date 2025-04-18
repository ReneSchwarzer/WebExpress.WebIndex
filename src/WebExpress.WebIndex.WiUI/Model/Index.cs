namespace WebExpress.WebIndex.WiUI.Model
{
    /// <summary>
    /// Represents an index file *.ws.
    /// </summary>
    public class Index
    {
        /// <summary>
        /// Returns or sets the name of the field.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Returns or sets the file name with its path.
        /// </summary>
        public string? FileNameWithPath { get; set; }
    }
}
