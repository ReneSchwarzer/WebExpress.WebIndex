using WebExpress.WebIndex.Term;

namespace WebExpress.WebIndex
{
    /// <summary>
    /// The context of an indexed document.
    /// </summary>
    public class IndexDocumemntContext : IndexContext, IIndexDocumemntContext
    {
        /// <summary>
        /// Returns the token analyzer that is valid in the context of the IndexDocument.
        /// </summary>
        public IndexTokenAnalyzer TokenAnalyzer { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public IndexDocumemntContext()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">The base context.</param>
        /// <param name="tokenAnalyzer">The tike analyzer.</param>
        public IndexDocumemntContext(IIndexContext context, IndexTokenAnalyzer tokenAnalyzer)
        {
            IndexDirectory = context.IndexDirectory;
            TokenAnalyzer = tokenAnalyzer;
        }
    }
}
