using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace WebExpress.WebIndex.Storage
{
    /// <summary>
    /// Represents a reverse index or document storage at the file level.
    /// </summary>
    public class IndexStorageFile : IDisposable
    {
        /// <summary>
        /// Returns the file name.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Returns a stream for the index file.
        /// </summary>
        private FileStream FileStream { get; set; }

        /// <summary>
        /// Returns a buffered stream to improve the read and write performance.
        /// </summary>
        private BufferedStream BufferedStream { get; set; }

        /// <summary>
        /// Returns a reader to read the stream.
        /// </summary>
        internal BinaryReader Reader { get; private set; }

        /// <summary>
        /// Returns a writer to write data to the stream.
        /// </summary>
        internal BinaryWriter Writer { get; private set; }

        /// <summary>
        /// Returns a buffer for caching segments.
        /// </summary>
        private IndexStorageReadBuffer ReadBuffer { get; set; }

        /// <summary>
        /// Unsaved entries queue.
        /// </summary>
        private ConcurrentQueue<IIndexStorageSegment> WriteBuffer { get; } = new ConcurrentQueue<IIndexStorageSegment>();

        /// <summary>
        /// Returns the timer for sorting out segments from the read buffer.
        /// </summary>
        private Timer ReadTimer { get; set; }

        /// <summary>
        /// Returns the timer for writing the segments from the write buffer.
        /// </summary>
        private Timer WriteTimer { get; set; }

        /// <summary>
        /// Returns or sets the next free address.
        /// </summary>
        public ulong NextFreeAddr { get; internal set; } = 0ul;

        /// <summary>
        /// Returns a guard to protect against concurrent access.
        /// </summary>
        private object Guard { get; } = new object();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fileName">The file name.</param>
        public IndexStorageFile(string fileName)
        {
            FileName = fileName;

            Directory.CreateDirectory(Path.GetDirectoryName(FileName));

            if (File.Exists(FileName))
            {
                FileStream = File.Open(FileName, FileMode.OpenOrCreate);
            }
            else
            {
                FileStream = File.Open(FileName, FileMode.CreateNew);
            }

            BufferedStream = new BufferedStream(FileStream);
            Reader = new BinaryReader(BufferedStream);
            Writer = new BinaryWriter(BufferedStream);
            ReadBuffer = new IndexStorageReadBuffer(50000);

            ReadTimer = new Timer((state) => ReadBuffer.ReduceLifetimeAndRemoveExpiredSegments(), null, 0, 1000);
            WriteTimer = new Timer((state) => Flush(), null, 0, 100);
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
            if (ReadBuffer.GetSegment(addr, out IIndexStorageSegment cached))
            {
                return (T)cached;
            }

            //foreach (var seg in WriteBuffer.Where(x => x.Addr == addr))
            //{
            //    return (T)seg;
            //}

            T segment = (T)Activator.CreateInstance(typeof(T), context, addr);

            Reader.BaseStream.Seek((long)segment.Addr, SeekOrigin.Begin);
            segment.Read(Reader);

            return segment;
        }

        /// <summary>
        /// Reads the record from the storage medium.
        /// </summary>
        /// <param name="segment">The segment.</param>
        public T Read<T>(T segment) where T : IIndexStorageSegment
        {
            if (ReadBuffer.GetSegment(segment.Addr, out IIndexStorageSegment cached))
            {
                return (T)cached;
            }

            //foreach (var seg in WriteBuffer.Where(x => x.Addr == segment.Addr))
            //{
            //    return (T)seg;
            //}

            Reader.BaseStream.Seek((long)segment.Addr, SeekOrigin.Begin);
            segment.Read(Reader);

            return segment;
        }

        /// <summary>
        /// Writes the record to the storage medium.
        /// </summary>
        /// <param name="segment">The segment.</param>
        public void Write(IIndexStorageSegment segment)
        {
            ReadBuffer.Add(segment);

            if (WriteBuffer.Where(x => x.Addr == segment.Addr).Any())
            {
                return;
            }

            WriteBuffer.Enqueue(segment);
        }

        /// <summary>
        /// Ensures that all segments in the buffer is written to the storage device.
        /// </summary>
        public void Flush()
        {
            // lock queue before concurrent access
            lock (Guard)
            {
                if (!FileStream.CanWrite)
                {
                    return;
                }

                var list = new List<IIndexStorageSegment>();

                list.AddRange(WriteBuffer);
                WriteBuffer.Clear();

                // protect file writing from concurrent access
                foreach (var segment in list)
                {
                    Writer.BaseStream.Seek((long)segment.Addr, SeekOrigin.Begin);
                    segment.Write(Writer);
                }

                FileStream.Flush();
                BufferedStream.Flush();
                Writer.Flush();
            }
        }

        /// <summary>
        /// Performs cache invalidation for a specific IndexStorageSegment object.
        /// </summary>
        /// <param name="segment">The IndexStorageSegment object to be invalidated.</param>
        public void Invalidation(IIndexStorageSegment segment)
        {
            ReadBuffer.Invalidation(segment);
        }

        /// <summary>
        /// Is called to free up resources.
        /// </summary>
        public void Dispose()
        {
            Flush();

            // lock queue before concurrent access
            lock (Guard)
            {
                ReadTimer.Dispose();
                WriteTimer.Dispose();

                Reader.Close();
                Writer.Close();
                BufferedStream.Close();
                FileStream.Close();
            }
        }
    }
}