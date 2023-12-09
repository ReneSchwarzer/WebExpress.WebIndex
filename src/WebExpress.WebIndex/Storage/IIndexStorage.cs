namespace WebExpress.WebIndex.Storage
{
    /// <summary>
    /// Interface for identifying an index that is kept in the file system.
    /// </summary>
    public interface IIndexStorage
    {
        /// <summary>
        /// Returns the file name for the reverse index.
        /// </summary>
        string FileName { get; }

        /// <summary>
        /// Returns or sets the reverse index file.
        /// </summary>
        IndexStorageFile IndexFile { get; }

        /// <summary>
        /// Returns or sets the header.
        /// </summary>
        IndexStorageSegmentHeader Header { get; }

        /// <summary>
        /// Returns or sets the memory manager.
        /// </summary>
        IndexStorageSegmentAllocator Allocator { get; }

        /// <summary>
        /// Returns the statistical values that can be help to optimize the index.
        /// </summary>
        IndexStorageSegmentStatistic Statistic { get; }
    }
}