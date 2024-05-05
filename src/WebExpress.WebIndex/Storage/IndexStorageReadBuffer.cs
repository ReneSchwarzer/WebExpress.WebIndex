using System.Collections.Generic;

namespace WebExpress.WebIndex.Storage
{
    /// <summary>
    /// A read buffer for buffering of segments.
    /// </summary>
    public class IndexStorageReadBuffer
    {
        /// <summary>
        /// Buffer for random access.
        /// </summary>
        private readonly Dictionary<ulong, IndexStorageReadBufferItem> cache;

        /// <summary>
        /// Returns a segment if sored.
        /// </summary>
        /// <param name="addr">The address of th segment.</param>
        public IIndexStorageSegment this[ulong addr]
        {
            get
            {
                cache.TryGetValue(addr, out IndexStorageReadBufferItem value);
                value?.Refresh();

                return value?.Segment;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="capacity">The number of elements to be stored in the ring buffer.</param>
        public IndexStorageReadBuffer(uint capacity)
        {
            cache = new Dictionary<ulong, IndexStorageReadBufferItem>((int)capacity);
        }

        /// <summary>
        /// Adds an segment to the end of the ring buffer.
        /// </summary>
        /// <param name="segment">The segment.</param>
        public void Add(IIndexStorageSegment segment)
        {
            if (cache.TryGetValue(segment.Addr, out IndexStorageReadBufferItem value))
            {
                value?.Refresh();

                return;
            }

            cache.Add(segment.Addr, new IndexStorageReadBufferItem(segment));
        }

        /// <summary>
        /// Reduces the lifetime of all cached segments by one unit and expired segments are removed.
        /// </summary>
        public void ReduceLifetimeAndRemoveExpiredSegments()
        {
            var items = new List<KeyValuePair<ulong, IndexStorageReadBufferItem>>(cache);

            foreach (var item in items)
            {
                item.Value.Counter--;

                if (item.Value.Counter <= 0)
                {
                    cache.Remove(item.Key);
                }
            }
        }

        /// <summary>
        /// Performs cache invalidation for a specific IndexStorageSegment object.
        /// </summary>
        /// <param name="segment">The segment object to be invalidated.</param>
        public void Invalidation(IIndexStorageSegment segment)
        {
            cache.Remove(segment.Addr);
        }

        /// <summary>
        /// Checks whether a segment exists at the given address.
        /// </summary>
        /// <param name="item">The segment.</param>
        /// <returns>True if the segment has already been stored in the buffer, false otherwise.</returns>
        public bool Contains(IIndexStorageSegment segment)
        {
            return cache.ContainsKey(segment.Addr);
        }

        /// <summary>
        /// Checks whether a segment exists at the given address.
        /// </summary>
        /// <param name="addr">The adress of the segment.</param>
        /// <returns>True if the segment has already been stored in the buffer, false otherwise.</returns>
        public bool Contains(ulong addr)
        {
            return cache.ContainsKey(addr);
        }
    }
}