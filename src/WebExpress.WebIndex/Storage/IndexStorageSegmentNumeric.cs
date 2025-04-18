namespace WebExpress.WebIndex.Storage
{
    /// <summary>
    /// Each node is a numeric value stored in a binary tree. Each node has additional 
    /// information about the value, such as its frequency, position in the document, and other relevant information that can be
    /// useful in search queries.
    /// </summary>
    /// <param name="context">The reference to the context of the index.</param>
    public class IndexStorageSegmentNumeric(IndexStorageContext context)
        : IndexStorageSegmentNumericNode(context, context.IndexFile.Alloc(SegmentSize))
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