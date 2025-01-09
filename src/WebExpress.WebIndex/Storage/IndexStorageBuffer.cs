using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace WebExpress.WebIndex.Storage
{
    /// <summary>
    /// A read buffer for buffering of segments.
    /// </summary>
    public class IndexStorageBuffer : IDisposable
    {
        /// <summary>
        /// Returns the maximum upper limit of the cached segments
        /// </summary>
        public static uint MaxCachedSegments { get; set; } = 50000;

        /// <summary>
        /// Buffer for random access of segments.
        /// </summary>
        private Dictionary<ulong, IndexStorageBufferItem> _readCache;

        /// <summary>
        /// Buffer for random access of imperishable segments.
        /// </summary>
        private readonly Dictionary<ulong, IndexStorageBufferItem> _imperishableCache;

        /// <summary>
        /// Buffer for random access.
        /// </summary>
        private readonly Dictionary<ulong, IIndexStorageSegment> _writeCache;

        /// <summary>
        /// Returns a reader to read the stream.
        /// </summary>
        private BinaryReader Reader { get; set; }

        /// <summary>
        /// Returns a writer to write data to the stream.
        /// </summary>
        private BinaryWriter Writer { get; set; }

        /// <summary>
        /// Returns the timer for sorting out segments from the read buffer.
        /// </summary>
        private Timer Timer { get; set; }

        /// <summary>
        /// Returns a guard to protect against concurrent access.
        /// </summary>
        private object Guard { get; } = new object();

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="file">A stream for the index file.</param>
        public IndexStorageBuffer(IndexStorageFile file)
        {
            _readCache = new Dictionary<ulong, IndexStorageBufferItem>((int)MaxCachedSegments);
            _imperishableCache = new Dictionary<ulong, IndexStorageBufferItem>((int)MaxCachedSegments);
            _writeCache = new Dictionary<ulong, IIndexStorageSegment>((int)MaxCachedSegments);

            Reader = new BinaryReader(file.FileStream, Encoding.UTF8);
            Writer = new BinaryWriter(file.FileStream, Encoding.UTF8);

            Timer = new Timer((state) =>
            {
                ReduceLifetimeAndRemoveExpiredSegments();
                Flush();
            }, null, 10, 10);
        }

        /// <summary>
        /// Reads the record from the storage medium.
        /// </summary>
        /// <param name="addr">The segment address.</param>
        /// <param name="context">The reference to the context of the index.</param>
        /// <typeparam name="T">The type to be read.</typeparam>
        /// <returns>The segment, how it was read by the storage medium.</returns>
        public T Read<T>(ulong addr, IndexStorageContext context) where T : IIndexStorageSegment
        {
            lock (Guard)
            {
                if (GetSegment(addr, out IIndexStorageSegment readCached))
                {
                    if (readCached is T cachedSegment)
                    {
                        return cachedSegment;
                    }
                }

                var segment = (T)Activator.CreateInstance(typeof(T), context, addr);

                Reader.BaseStream.Seek((long)segment.Addr, SeekOrigin.Begin);
                segment.Read(Reader);

                Cache(segment);

                return segment;
            }
        }

        /// <summary>
        /// Read and adds an segment to the end of the buffer.
        /// </summary>
        /// <param name="segment">The segment.</param>
        public T Read<T>(IIndexStorageSegment segment) where T : IIndexStorageSegment
        {
            lock (Guard)
            {
                if (GetSegment(segment.Addr, out IIndexStorageSegment readCached))
                {
                    return (T)readCached;
                }

                Reader.BaseStream.Seek((long)segment.Addr, SeekOrigin.Begin);
                segment.Read(Reader);

                Cache(segment);
            }

            return (T)segment;
        }

        /// <summary>
        /// Adds an segment to the end of the buffer.
        /// </summary>
        /// <param name="segment">The segment.</param>
        public void Write(IIndexStorageSegment segment)
        {
            if (segment == null)
            {
                return;
            }

            lock (Guard)
            {
                if (!_writeCache.TryAdd(segment.Addr, segment))
                {
                    _writeCache[segment.Addr] = segment;
                }
            }
        }

        /// <summary>
        /// Performs cache invalidation for a specific IndexStorageSegment object.
        /// </summary>
        /// <param name="segment">The segment object to be invalidated.</param>
        public void Invalidation(IIndexStorageSegment segment)
        {
            _readCache.Remove(segment.Addr, out _);
            _imperishableCache.Remove(segment.Addr, out _);
        }

        /// <summary>
        /// Adds an segment to the end of the buffer.
        /// </summary>
        /// <param name="segment">The segment.</param>
        private void Cache(IIndexStorageSegment segment)
        {
            var segmentItem = new IndexStorageBufferItem(segment);

            if (segmentItem.Counter < uint.MaxValue)
            {
                if (!_readCache.TryAdd(segment.Addr, segmentItem))
                {
                    _readCache[segment.Addr] = segmentItem;

                }
            }
            else
            {
                if (!_imperishableCache.TryAdd(segment.Addr, segmentItem))
                {
                    _imperishableCache[segment.Addr] = segmentItem;
                }
            }
        }

        /// <summary>
        /// Returns the segment with the given address, if available.
        /// </summary>
        /// <param name="addr">The adress of the segment.</param>
        /// <param name="segment">The segment or null.</param>
        /// <returns>True if the segment is cached, false otherwise.</returns>
        private bool GetSegment(ulong addr, out IIndexStorageSegment segment)
        {
            if (_readCache.TryGetValue(addr, out IndexStorageBufferItem cached))
            {
                cached.Refresh();
                segment = cached.Segment;

                return true;
            }

            if (_imperishableCache.TryGetValue(addr, out IndexStorageBufferItem imperishableCached))
            {
                imperishableCached.Refresh();
                segment = imperishableCached.Segment;

                return true;
            }

            if (_writeCache.TryGetValue(addr, out IIndexStorageSegment res))
            {
                segment = res;
                return true;
            }

            segment = null;
            return false;
        }

        /// <summary>
        /// Reduces the lifetime of all cached segments by one unit and expired segments are removed.
        /// </summary>
        private void ReduceLifetimeAndRemoveExpiredSegments()
        {
            if (_readCache.Count < 0.8 * MaxCachedSegments)
            {
                lock (Guard)
                {
                    // under 80% remove as needed
                    foreach (var item in _readCache)
                    {
                        item.Value.IncrementCounter();
                        if (item.Value.Counter <= 0)
                        {
                            _readCache.Remove(item.Key);
                        }
                    }
                }
            }
            else
            {
                // over 80% remove below average
                lock (Guard)
                {
                    var average = _readCache.Average(x => x.Value.Counter);
                    _readCache = new Dictionary<ulong, IndexStorageBufferItem>(_readCache.Where(x => x.Value.Counter <= average));
                }
            }
        }

        /// <summary>
        /// Ensures that all segments in the buffer is written to the storage device.
        /// </summary>
        public void Flush()
        {
            lock (Guard)
            {
                foreach (var segment in _writeCache.Values)
                {
                    if (_writeCache.Remove(segment.Addr, out IIndexStorageSegment seq))
                    {
                        Writer.BaseStream.Seek((long)seq.Addr, SeekOrigin.Begin);
                        seq.Write(Writer);
                    }
                }
            }
        }

        /// <summary>
        /// Is called to free up resources.
        /// </summary>
        public virtual void Dispose()
        {
            using var waitHandle = new ManualResetEvent(false);

            Timer.Dispose(waitHandle);
            waitHandle.WaitOne();

            Flush();
            Writer.Flush();

            GC.SuppressFinalize(this);
        }
    }
}