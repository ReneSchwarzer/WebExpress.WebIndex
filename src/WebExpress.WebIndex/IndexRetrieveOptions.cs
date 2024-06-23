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
        /// Returns the methods for data retrieval.
        /// </summary>
        public IndexRetrieveMethod Method { get; internal set; }

        /// <summary>
        /// Returns the distance for proximity searches.
        /// </summary>
        public uint Distance { get; internal set; } = 0;

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
