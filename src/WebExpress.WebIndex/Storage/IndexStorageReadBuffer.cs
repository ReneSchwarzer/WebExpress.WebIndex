using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace WebExpress.WebIndex.Storage
{
    /// <summary>
    /// A read buffer for buffering of segments.
    /// </summary>
    public class IndexStorageReadBuffer : IDisposable
    {
        /// <summary>
        /// Returns the maximum upper limit of the cached segments
        /// </summary>
        public static uint MaxCachedSegments { get; set; } = 50000;

        /// <summary>
        /// Buffer for random access of segments.
        /// </summary>
        private Dictionary<ulong, IndexStorageReadBufferItem> _cache;

        /// <summary>
        /// Buffer for random access of imperishable segments.
        /// </summary>
        private readonly Dictionary<ulong, IndexStorageReadBufferItem> _imperishableCache;

        /// <summary>
        /// Returns the storage file.
        /// </summary>
        private IndexStorageFile StorageFile { get; set; }

        /// <summary>
        /// Returns a reader to read the stream.
        /// </summary>
        internal BinaryReader Reader { get; private set; }

        /// <summary>
        /// Returns the timer for sorting out segments from the read buffer.
        /// </summary>
        private Timer Timer { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="file">A stream for the index file.</param>
        public IndexStorageReadBuffer(IndexStorageFile file)
        {
            _cache = new Dictionary<ulong, IndexStorageReadBufferItem>((int)MaxCachedSegments);
            _imperishableCache = new Dictionary<ulong, IndexStorageReadBufferItem>((int)MaxCachedSegments);
            StorageFile = file;
            Reader = new BinaryReader(file.FileStream);

            Timer = new Timer((state) => ReduceLifetimeAndRemoveExpiredSegments(), null, 10, 10);
        }

        /// <summary>
        /// Adds an segment to the end of the buffer.
        /// </summary>
        /// <param name="segment">The segment.</param>
        public void Cache(IIndexStorageSegment segment)
        {
            var segmentItem = new IndexStorageReadBufferItem(segment);

            if (segmentItem.Counter < uint.MaxValue)
            {
                lock (StorageFile.Guard)
                {
                    if (_cache.ContainsKey(segment.Addr))
                    {
                        _cache[segment.Addr] = segmentItem;

                        return;
                    }

                    _cache.Add(segment.Addr, segmentItem);
                }
            }
            else
            {
                lock (StorageFile.Guard)
                {
                    if (_imperishableCache.ContainsKey(segment.Addr))
                    {
                        _cache[segment.Addr] = segmentItem;

                        return;
                    }

                    _imperishableCache.Add(segment.Addr, segmentItem);
                }
            }
        }

        /// <summary>
        /// Adds an segment to the end of the buffer.
        /// </summary>
        /// <param name="segment">The segment.</param>
        public void Read(IIndexStorageSegment segment)
        {
            lock (StorageFile.Guard)
            {
                Reader.BaseStream.Seek((long)segment.Addr, SeekOrigin.Begin);
                segment.Read(Reader);

                Cache(segment);
            }
        }

        /// <summary>
        /// Reduces the lifetime of all cached segments by one unit and expired segments are removed.
        /// </summary>
        public void ReduceLifetimeAndRemoveExpiredSegments()
        {
            if (_cache.Count < 0.8 * MaxCachedSegments)
            {
                lock (StorageFile.Guard)
                {
                    // under 80% remove as needed
                    foreach (var item in _cache)
                    {
                        item.Value.IncrementCounter();
                    }

                    _cache = new Dictionary<ulong, IndexStorageReadBufferItem>(_cache.Where(x => x.Value.Counter > 0));
                }
            }
            else
            {
                // over 80% remove below average
                lock (StorageFile.Guard)
                {
                    var average = _cache.Average(x => x.Value.Counter);
                    _cache = new Dictionary<ulong, IndexStorageReadBufferItem>(_cache.Where(x => x.Value.Counter <= average));
                }
            }
        }

        /// <summary>
        /// Performs cache invalidation for a specific IndexStorageSegment object.
        /// </summary>
        /// <param name="segment">The segment object to be invalidated.</param>
        public void Invalidation(IIndexStorageSegment segment)
        {
            _cache.Remove(segment.Addr, out _);
        }

        /// <summary>
        /// Checks whether a segment exists at the given address.
        /// </summary>
        /// <param name="item">The segment.</param>
        /// <returns>True if the segment has already been stored in the buffer, false otherwise.</returns>
        public bool Contains(IIndexStorageSegment segment)
        {
            return _cache.ContainsKey(segment.Addr) || _imperishableCache.ContainsKey(segment.Addr);
        }

        /// <summary>
        /// Checks whether a segment exists at the given address.
        /// </summary>
        /// <param name="addr">The adress of the segment.</param>
        /// <returns>True if the segment has already been stored in the buffer, false otherwise.</returns>
        public bool Contains(ulong addr)
        {
            return _cache.ContainsKey(addr) || _imperishableCache.ContainsKey(addr);
        }

        /// <summary>
        /// Returns the segment with the given address, if available.
        /// </summary>
        /// <param name="addr">The adress of the segment.</param>
        /// <param name="segment">The segment or null.</param>
        /// <returns>True if the segment is cached, false otherwise.</returns>
        public bool GetSegment<T>(ulong addr, out IIndexStorageSegment segment) where T : IIndexStorageSegment
        {
            if (_cache.TryGetValue(addr, out IndexStorageReadBufferItem cached))
            {
                cached.Refresh();
                segment = cached.Segment;

                return true;
            }

            if (_imperishableCache.TryGetValue(addr, out IndexStorageReadBufferItem imperishableCached))
            {
                imperishableCached.Refresh();
                segment = imperishableCached.Segment;

                return true;
            }

            segment = null;
            return false;
        }

        /// <summary>
        /// Is called to free up resources.
        /// </summary>
        public virtual void Dispose()
        {
            Timer.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}