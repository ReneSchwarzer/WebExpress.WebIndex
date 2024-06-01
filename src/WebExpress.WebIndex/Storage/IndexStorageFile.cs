using System;
using System.IO;
using WebExpress.WebIndex.Utility;

namespace WebExpress.WebIndex.Storage
{
    /// <summary>
    /// Represents a reverse index or document storage at the file level.
    /// </summary>
    public class IndexStorageFile : IDisposable
    {
        /// <summary>
        /// Returns the maximum upper limit of the cached segments
        /// </summary>
        public static uint BufferSize { get; set; } = 4 * 1024; // 4096 Byte

        /// <summary>
        /// Returns the file name.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Returns a stream for the index file.
        /// </summary>
        internal FileStream FileStream { get; private set; }

        /// <summary>
        /// Returns a buffer for caching segments.
        /// </summary>
        private IndexStorageReadBuffer ReadBuffer { get; set; }

        /// <summary>
        /// Returns a buffer for caching segments.
        /// </summary>
        private IndexStorageWriteBuffer WriteBuffer { get; set; }

        /// <summary>
        /// Returns or sets the next free address.
        /// </summary>
        public ulong NextFreeAddr { get; internal set; } = 0ul;

        /// <summary>
        /// Returns a guard to protect against concurrent access.
        /// </summary>
        internal object Guard { get; } = new object();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fileName">The file name.</param>
        public IndexStorageFile(string fileName)
        {
            #if DEBUG 
            using var profiling = Profiling.Diagnostic(); 
            #endif
            
            FileName = fileName;

            Directory.CreateDirectory(Path.GetDirectoryName(FileName));
            var options = new FileStreamOptions()
            {
                BufferSize = (int)BufferSize,
                Mode = FileMode.OpenOrCreate,
                Share = FileShare.None,
                Access = FileAccess.ReadWrite
            };

            FileStream = File.Open(FileName, options);
            ReadBuffer = new IndexStorageReadBuffer(this);
            WriteBuffer = new IndexStorageWriteBuffer(this);
        }

        /// <summary>
        /// Allocate the memory.
        /// </summary>
        /// <param name="segment">The segment determines how much memory should be reserved.</param>
        /// <param name="size"><param>
        /// <returns>The start address of the reserved storage area.</returns>
        public ulong Alloc(uint size)
        {
            #if DEBUG 
            using var profiling = Profiling.Diagnostic(); 
            #endif

            var addr = NextFreeAddr;
            NextFreeAddr += size;

            return addr;
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
            #if DEBUG 
            using var profiling = Profiling.Diagnostic(); 
            #endif

            if (ReadBuffer.GetSegment<T>(addr, out IIndexStorageSegment readCached))
            {
                return (T)readCached;
            }

            lock (Guard)
            {
                if (WriteBuffer.GetSegment(addr, out IIndexStorageSegment writeCached))
                {
                    return (T)writeCached;
                }

                var segment = (T)Activator.CreateInstance(typeof(T), context, addr);
                ReadBuffer.Read(segment);

                return segment;
            }
        }

        /// <summary>
        /// Reads the record from the storage medium.
        /// </summary>
        /// <param name="segment">The segment.</param>
        public T Read<T>(T segment) where T : IIndexStorageSegment
        {
            #if DEBUG 
            using var profiling = Profiling.Diagnostic(); 
            #endif

            if (ReadBuffer.GetSegment<T>(segment.Addr, out IIndexStorageSegment readCached))
            {
                return (T)readCached;
            }

            lock (Guard)
            {
                if (WriteBuffer.GetSegment(segment.Addr, out IIndexStorageSegment writeCached))
                {
                    return (T)writeCached;
                }

                ReadBuffer.Read(segment);
            }

            return segment;
        }

        /// <summary>
        /// Writes the record to the storage medium.
        /// </summary>
        /// <param name="segment">The segment.</param>
        public void Write(IIndexStorageSegment segment)
        {
            #if DEBUG 
            using var profiling = Profiling.Diagnostic(); 
            #endif

            ReadBuffer.Cache(segment);
            WriteBuffer.Cache(segment);
        }

        /// <summary>
        /// Ensures that all segments in the buffer is written to the storage device.
        /// </summary>
        public void Flush()
        {
            #if DEBUG 
            using var profiling = Profiling.Diagnostic(); 
            #endif

            if (!FileStream.CanWrite)
            {
                return;
            }

            WriteBuffer.Flush();
        }

        /// <summary>
        /// Performs cache invalidation for a specific IndexStorageSegment object.
        /// </summary>
        /// <param name="segment">The IndexStorageSegment object to be invalidated.</param>
        public void Invalidation(IIndexStorageSegment segment)
        {
            #if DEBUG 
            using var profiling = Profiling.Diagnostic(); 
            #endif

            ReadBuffer.Invalidation(segment);
        }

        /// <summary>
        /// Is called to free up resources.
        /// </summary>
        public void Dispose()
        {
            #if DEBUG 
            using var profiling = Profiling.Diagnostic(); 
            #endif
            
            ReadBuffer.Dispose();
            WriteBuffer.Dispose();

            FileStream.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}