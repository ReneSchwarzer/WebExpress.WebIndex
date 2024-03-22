using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

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
        private readonly Dictionary<ulong, IndexStorageReadBufferItem> dict;

        /// <summary>
        /// Returns a segment if sored.
        /// </summary>
        /// <param name="addr">The address of th segment.</param>
        public IIndexStorageSegment this[ulong addr]
        {
            get 
            {
                dict.TryGetValue(addr, out IndexStorageReadBufferItem value);
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
            dict = new Dictionary<ulong, IndexStorageReadBufferItem>((int)capacity);
        }

        /// <summary>
        /// Adds an item to the end of the ring buffer.
        /// </summary>
        /// <param name="item">The segment.</param>
        public void Add(IIndexStorageSegment item)
        {
            if (dict.TryGetValue(item.Addr, out IndexStorageReadBufferItem value))
            {
                value?.Refresh();
                return;
            }

            dict.Add(item.Addr, new IndexStorageReadBufferItem(item));
        }    
    
        /// <summary>
        /// Reduces the lifetime of all cached segments by one unit and expired segments are removed.
        /// </summary>
        public void ReduceLifetimeAndRemoveExpiredSegments()
        {
            var keys = new List<ulong>();

            foreach (var item in dict)
            {
                item.Value.Counter--;

                if (item.Value.Counter <= 0)
                {
                    keys.Add(item.Key);
                }
            }

            foreach (var key in keys)
            {
                dict.Remove(key);
            }
        }

        /// <summary>
        /// Checks whether a segment exists at the given address.
        /// </summary>
        /// <param name="item">The segment.</param>
        /// <returns>True if the segment has already been stored in the buffer, false otherwise.</returns>
        public bool Contains(IIndexStorageSegment item)
        {
            return dict.ContainsKey(item.Addr);
        }

        /// <summary>
        /// Checks whether a segment exists at the given address.
        /// </summary>
        /// <param name="addr">The adress of the segment.</param>
        /// <returns>True if the segment has already been stored in the buffer, false otherwise.</returns>
        public bool Contains(ulong addr)
        {
            return dict.ContainsKey(addr);
        }
    }
}