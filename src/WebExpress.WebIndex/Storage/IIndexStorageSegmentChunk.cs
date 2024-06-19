using System;

namespace WebExpress.WebIndex.Storage
{
    public interface IIndexStorageSegmentChunk : IIndexStorageSegment, IComparable
    {
        /// <summary>
        /// Returns or sets the address of the following chunk item.
        /// </summary>
        ulong NextChunkAddr { get; set; }
    }
}