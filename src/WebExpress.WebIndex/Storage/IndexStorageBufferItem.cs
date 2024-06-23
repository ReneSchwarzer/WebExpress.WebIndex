﻿using System.Reflection;
using WebExpress.WebIndex.WebAttribute;

namespace WebExpress.WebIndex.Storage
{
    /// <summary>
    /// A read buffer item for buffering of segments.
    /// </summary>
    public class IndexStorageBufferItem
    {
        /// <summary>
        /// Returns or sets the lifetime.
        /// </summary>
        public static uint Lifetime { get; set; } = 100;

        /// <summary>
        /// The lifetime counter is for deletion from the buffer.
        /// </summary>
        private uint _counter;

        /// <summary>
        /// The lifetime counter is for deletion from the buffer.
        /// </summary>
        public uint Counter => _counter;

        /// <summary>
        /// Returns the segment to be cached.
        /// </summary>
        public IIndexStorageSegment Segment { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="segment">The segment to be cached.</param>
        public IndexStorageBufferItem(IIndexStorageSegment segment)
        {
            Segment = segment;

            if (segment.GetType().GetCustomAttribute<SegmentCachedAttribute>() != null)
            {
                _counter = uint.MaxValue;
            }
            else
            {
                Refresh();
            }
        }

        /// <summary>
        /// Increments the counter.
        /// </summary>
        public void IncrementCounter()
        {
            _counter--;
        }

        /// <summary>
        /// Refresh the lifetime.
        /// </summary>
        public void Refresh()
        {
            _counter = Lifetime;
        }
    }
}