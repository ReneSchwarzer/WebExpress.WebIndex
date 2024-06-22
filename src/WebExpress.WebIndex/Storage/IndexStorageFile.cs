using System;
using System.IO;

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
        private IndexStorageBuffer Buffer { get; set; }

        /// <summary>
        /// Returns or sets the next free address.
        /// </summary>
        public ulong NextFreeAddr { get; internal set; } = 0ul;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fileName">The file name.</param>
        public IndexStorageFile(string fileName)
        {
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
            Buffer = new IndexStorageBuffer(this);
        }

        /// <summary>
        /// Allocate the memory.
        /// </summary>
        /// <param name="segment">The segment determines how much memory should be reserved.</param>
        /// <param name="size"><param>
        /// <returns>The start address of the reserved storage area.</returns>
        public ulong Alloc(uint size)
        {
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
            return Buffer.Read<T>(addr, context);
        }

        /// <summary>
        /// Reads the record from the storage medium.
        /// </summary>
        /// <param name="segment">The segment.</param>
        public T Read<T>(T segment) where T : IIndexStorageSegment
        {
            return Buffer.Read<T>(segment);
        }

        /// <summary>
        /// Writes the record to the storage medium.
        /// </summary>
        /// <param name="segment">The segment.</param>
        public void Write(IIndexStorageSegment segment)
        {
            if (segment == null)
            {
                return;
            }

            Buffer.Write(segment);
        }

        /// <summary>
        /// Ensures that all segments in the buffer is written to the storage device.
        /// </summary>
        public void Flush()
        {
            if (!FileStream.CanWrite)
            {
                return;
            }

            Buffer.Flush();
        }

        /// <summary>
        /// Delete this file from storage.
        /// </summary>
        public void Delete()
        {
            Dispose();

            File.Delete(FileName);
        }

        /// <summary>
        /// Performs cache invalidation for a specific IndexStorageSegment object.
        /// </summary>
        /// <param name="segment">The IndexStorageSegment object to be invalidated.</param>
        public void Invalidation(IIndexStorageSegment segment)
        {
            Buffer.Invalidation(segment);
        }

        /// <summary>
        /// Is called to free up resources.
        /// </summary>
        public void Dispose()
        {
            Buffer.Dispose();
            FileStream.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}