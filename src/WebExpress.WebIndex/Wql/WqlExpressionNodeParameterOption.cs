namespace WebExpress.WebIndex.Wql
{
    /// <summary>
    /// Describes the parameter option expression of a wql statement.
    /// </summary>
    public class WqlExpressionNodeParameterOption<T> : IWqlExpressionNode<T> where T : IIndexItem
    {
        /// <summary>
        /// Returns the similarity value for the fuzzy search.
        /// </summary>
        public uint? Similarity { get; internal set; }

        /// <summary>
        /// Returns the distance value for the proximity search.
        /// </summary>
        public uint? Distance { get; internal set; }

        /// <summary>
        /// Constructor
        /// </summary>
        internal WqlExpressionNodeParameterOption()
        {
        }

        /// <summary>
        /// Converts the options expression to a string.
        /// </summary>
        /// <returns>The options expression as a string.</returns>
        public override string ToString()
        {
            return $"{(Similarity != null ? "~" + Similarity : "")} {(Distance != null ? ":" + Distance : "")}".Trim();
        }
    }
}