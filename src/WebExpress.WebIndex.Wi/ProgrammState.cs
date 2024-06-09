namespace WebExpress.WebIndex.Wi
{
    /// <summary>
    /// Enum for the different states of the program.
    /// </summary>
    internal enum ProgrammState
    {
        /// <summary>
        /// The initial state of the program.
        /// </summary>
        Initial,

        /// <summary>
        /// The state when the program is opening the index file.
        /// </summary>
        OpenIndexFile,

        /// <summary>
        /// The state when the program is opening an attribute.
        /// </summary>
        OpenAttribute
    }
}
