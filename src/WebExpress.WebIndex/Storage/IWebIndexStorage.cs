namespace WebExpress.WebIndex.Storage
{
    /// <summary>
    /// Interface for identifying an index that is kept in the file system.
    /// </summary>
    public interface IWebIndexStorage
    {
        /// <summary>
        /// Returns the file name for the reverse index.
        /// </summary>
        string FileName { get; }

        /// <summary>
        /// Returns or sets the reverse index file.
        /// </summary>
        WebIndexStorageFile IndexFile { get; }

        /// <summary>
        /// Returns or sets the header.
        /// </summary>
        WebIndexStorageSegmentHeader Header { get; }

        /// <summary>
        /// Returns or sets the memory manager.
        /// </summary>
        WebIndexStorageSegmentAllocator Allocator { get; }

        /// <summary>
        /// Returns the statistical values that can be help to optimize the index.
        /// </summary>
        WebIndexStorageSegmentStatistic Statistic { get; }
    }
}