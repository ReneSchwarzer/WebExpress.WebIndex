using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;

namespace WebExpress.WebIndex.Storage
{
    /// <summary>
    /// A wirte buffer for buffering of segments.
    /// </summary>
    public class IndexStorageWriteBuffer : IDisposable
    {
        /// <summary>
        /// Buffer for random access.
        /// </summary>
        private readonly ConcurrentDictionary<ulong, IIndexStorageSegment> _cache;

        /// <summary>
        /// Returns the storage file.
        /// </summary>
        private IndexStorageFile StorageFile { get; set; }

        /// <summary>
        /// Returns a writer to write data to the stream.
        /// </summary>
        internal BinaryWriter Writer { get; private set; }

        /// <summary>
        /// Returns the timer for sorting out segments from the read buffer.
        /// </summary>
        private Timer Timer { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="file">A stream for the index file.</param>
        public IndexStorageWriteBuffer(IndexStorageFile file)
        {
            _cache = new ConcurrentDictionary<ulong, IIndexStorageSegment>(-1, 5000);
            StorageFile = file;
            Writer = new BinaryWriter(file.FileStream);

            Timer = new Timer((state) => Flush(), null, 10, 10);
        }

        /// <summary>
        /// Adds an segment to the end of the ring buffer.
        /// </summary>
        /// <param name="segment">The segment.</param>
        public void Cache(IIndexStorageSegment segment)
        {
            lock (StorageFile.Guard)
            {
                _cache.AddOrUpdate(segment.Addr, segment, (key, oldValue) => segment);
            }
        }

        /// <summary>
        /// Checks whether a segment exists at the given address.
        /// </summary>
        /// <param name="item">The segment.</param>
        /// <returns>True if the segment has already been stored in the buffer, false otherwise.</returns>
        public bool Contains(IIndexStorageSegment segment)
        {
            return _cache.ContainsKey(segment.Addr);
        }

        /// <summary>
        /// Checks whether a segment exists at the given address.
        /// </summary>
        /// <param name="addr">The adress of the segment.</param>
        /// <returns>True if the segment has already been stored in the buffer, false otherwise.</returns>
        public bool Contains(ulong addr)
        {
            return _cache.ContainsKey(addr);
        }

        /// <summary>
        /// Returns the segment with the given address, if available.
        /// </summary>
        /// <param name="addr">The adress of the segment.</param>
        /// <param name="segment">The segment or null.</param>
        /// <returns>The segment if cached or null.</returns>
        public bool GetSegment(ulong addr, out IIndexStorageSegment segment)
        {
            if (_cache.TryGetValue(addr, out IIndexStorageSegment res))
            {
                segment = res;

                return true;
            }

            segment = null;

            return false;
        }

        /// <summary>
        /// Ensures that all segments in the buffer is written to the storage device.
        /// </summary>
        public void Flush()
        {
            lock (StorageFile.Guard)
            {
                var segments = _cache.Values.ToList();
                _cache.Clear();

                foreach (var segment in segments)
                {
                    Writer.BaseStream.Seek((long)segment.Addr, SeekOrigin.Begin);
                    segment.Write(Writer);
                }

                Writer.Flush();
            }
        }

        /// <summary>
        /// Is called to free up resources.
        /// </summary>
        public virtual void Dispose()
        {
            Flush();
            Timer.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}