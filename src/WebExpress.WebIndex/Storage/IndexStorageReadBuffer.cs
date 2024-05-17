using System.Collections.Concurrent;
using System.Linq;

namespace WebExpress.WebIndex.Storage
{
    /// <summary>
    /// A read buffer for buffering of segments.
    /// </summary>
    public class IndexStorageReadBuffer
    {
        /// <summary>
        /// Returns the maximum upper limit of the cached segments
        /// </summary>
        public static uint MaxCachedSegments {get; set;} = 50000;

        /// <summary>
        /// Buffer for random access.
        /// </summary>
        private ConcurrentDictionary<ulong, IndexStorageReadBufferItem> cache;

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
        public IndexStorageReadBuffer()
        {
            cache = new ConcurrentDictionary<ulong, IndexStorageReadBufferItem>();
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

            cache.TryAdd(segment.Addr, new IndexStorageReadBufferItem(segment));
        }

        /// <summary>
        /// Reduces the lifetime of all cached segments by one unit and expired segments are removed.
        /// </summary>
        public void ReduceLifetimeAndRemoveExpiredSegments()
        {
            if (cache.Count < 0.8 * MaxCachedSegments)
            {
                // under 80% remove as needed
                foreach (var item in cache)
                {
                    item.Value.Counter--;

                    if (item.Value.Counter <= 0)
                    {
                        cache.TryRemove(item.Key, out _);
                    }
                }
            }
            else
            {
                // over 80% remove below average
                var average = cache.Where(x => x.Value.Counter < uint.MaxValue).Average(x => x.Value.Counter);

                foreach (var item in cache.Where(x => x.Value.Counter < average))
                {
                    item.Value.Counter = 0;
                    cache.TryRemove(item.Key, out _);
                }
            }
        }

        /// <summary>
        /// Performs cache invalidation for a specific IndexStorageSegment object.
        /// </summary>
        /// <param name="segment">The segment object to be invalidated.</param>
        public void Invalidation(IIndexStorageSegment segment)
        {
            cache.TryRemove(segment.Addr, out _);
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

        /// <summary>
        /// Returns the segment with the given address, if available.
        /// </summary>
        /// <param name="addr">The adress of the segment.</param>
        /// <param name="segment">The segment or null.</param>
        /// <returns>The segment if cached or null.</returns>
        public bool GetSegment(ulong addr, out IIndexStorageSegment segment)
        {
            if (cache.TryGetValue(addr, out IndexStorageReadBufferItem res))
            {
                res.Refresh();
                segment = res.Segment;

                return true;
            }

            segment = null;
            return false;
        }
    }
}