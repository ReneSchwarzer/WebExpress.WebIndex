namespace WebExpress.WebIndex.Storage
{
    /// <summary>
    /// Each term is broken down into individual characters and stored as separate nodes in a search tree. With the exception 
    /// of the root node, the TreeNode segments, which have a constant length, are stored in the data area of the reverse 
    /// index. The path determines the term. Each node, which is a complete term, points to a term segment, which has additional 
    /// information about the term, such as its frequency, position in the document, and other relevant information that can be
    /// useful in search queries.
    /// </summary>
    /// <param name="context">The reference to the context of the index.</param>
    public class IndexStorageSegmentTerm(IndexStorageContext context)
        : IndexStorageSegmentTermNode(context, context.IndexFile.Alloc(SegmentSize))
    {
        /// <summary>
        /// Initialization method for the header segment.
        /// </summary>
        /// <param name="initializationFromFile">If true, initializes from file. Otherwise, initializes and writes to file.</param>
        public virtual void Initialization(bool initializationFromFile)
        {
            if (initializationFromFile)
            {
                Context.IndexFile.Read(this);
            }
            else
            {
                Context.IndexFile.Write(this);
            }
        }
    }
}