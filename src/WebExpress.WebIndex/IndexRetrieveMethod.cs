namespace WebExpress.WebIndex
{
    /// <summary>
    /// Defines the methods for data retrieval.
    /// </summary>
    public enum IndexRetrieveMethod
    {
        /// <summary>
        /// Query based on an word search.
        /// </summary>
        Default,

        /// <summary>
        /// Query based on an exact phrase.
        /// </summary>
        Phrase,

        /// <summary>
        /// Query based on the proximity of search terms.
        /// </summary>
        Proximity
    }
}
