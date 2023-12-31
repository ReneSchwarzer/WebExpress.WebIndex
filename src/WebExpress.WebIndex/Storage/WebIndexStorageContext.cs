namespace WebExpress.WebIndex.Storage
{
    public class WebIndexStorageContext
    {
        private IWebIndexStorage Index { get; set; }

        /// <summary>
        /// Returns or sets the reverse index file.
        /// </summary>
        public WebIndexStorageFile IndexFile => Index.IndexFile;

        /// <summary>
        /// Returns or sets the header.
        /// </summary>
        public WebIndexStorageSegmentHeader Header => Index.Header;

        /// <summary>
        /// Returns or sets the memory manager.
        /// </summary>
        public WebIndexStorageSegmentAllocator Allocator => Index.Allocator;

        /// <summary>
        /// Returns the statistical values that can be help to optimize the index.
        /// </summary>
        public WebIndexStorageSegmentStatistic Statistic => Index.Statistic;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="index">The index.</param>
        public WebIndexStorageContext(IWebIndexStorage index)
        {
            Index = index;
        }
    }
}