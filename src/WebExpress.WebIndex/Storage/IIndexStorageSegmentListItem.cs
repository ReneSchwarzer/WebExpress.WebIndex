using System;

namespace WebExpress.WebIndex.Storage
{
    public interface IIndexStorageSegmentListItem : IIndexStorageSegment, IComparable
    {
        /// <summary>
        /// Returns or sets the address of the following list item.
        /// </summary>
        ulong SuccessorAddr { get; set; }
    }
}