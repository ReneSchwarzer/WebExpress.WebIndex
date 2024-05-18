namespace WebExpress.WebIndex
{
    /// <summary>
    /// Represents the options for the search.
    /// </summary>
    public struct IndexRetrieveOptions
    {
        /// <summary>
        /// Returns or sets the maximum results.
        /// </summary>
        public uint MaxResults { get; internal set; } = 10000u;

        /// <summary>
        /// Constructor
        /// </summary>
        public IndexRetrieveOptions()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="maxResults">The maximum results.</param>
        public IndexRetrieveOptions(uint maxResults)
        {
            MaxResults = maxResults;
        }
    }
}
