using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace WebExpress.WebIndex.Storage
{
    /// <summary>
    /// Implementation of the web document store, which stores the key-value pairs on disk.
    /// </summary>
    /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
    public class IndexStorageDocumentStore<T> : IIndexDocumentStore<T>, IIndexStorage where T : IIndexItem
    {
        /// <summary>
        /// Returns the file name for the reverse index.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Returns or sets the file.
        /// </summary>
        public IndexStorageFile IndexFile { get; private set; }

        /// <summary>
        /// Returns or sets the header.
        /// </summary>
        public IndexStorageSegmentHeader Header { get; private set; }

        /// <summary>
        /// Returns or sets the hash map.
        /// </summary>
        public IndexStorageSegmentHashMap HashMap { get; private set; }

        /// <summary>
        /// Returns or sets the memory manager.
        /// </summary>
        public IndexStorageSegmentAllocator Allocator { get; private set; }

        /// <summary>
        /// Returns the statistical values that can be help to optimize the index.
        /// </summary>
        public IndexStorageSegmentStatistic Statistic { get; private set; }

        /// <summary>
        /// Returns the index context.
        /// </summary>
        public IIndexContext Context { get; private set; }

        /// <summary>
        /// Returns the storage index context.
        /// </summary>
        public IndexStorageContext StorageContext { get; private set; }

        /// <summary>
        /// Returns all items.
        /// </summary>
        public IEnumerable<T> All => HashMap.All.Select(x => GetItem(x));

        /// <summary>
        /// Returns or sets the predicted capacity (number of items to store) of the document store.
        /// </summary>
        public uint Capacity { get; set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="context">The index context.</param>
        /// <param name="capacity">The predicted capacity (number of items to store) of the document store.</param>
        public IndexStorageDocumentStore(IIndexContext context, uint capacity)
        {
            Context = context;
            StorageContext = new IndexStorageContext(this);
            Capacity = capacity;
            FileName = Path.Combine(Context.IndexDirectory, $"{typeof(T).Name}.wds");

            var exists = File.Exists(FileName);
            IndexFile = new IndexStorageFile(FileName);
            Header = new IndexStorageSegmentHeader(StorageContext) { Identifier = "wds" };
            Allocator = new IndexStorageSegmentAllocatorDocumentStore(StorageContext);
            Statistic = new IndexStorageSegmentStatistic(StorageContext);
            HashMap = new IndexStorageSegmentHashMap(StorageContext, Capacity);

            Allocator.Initialization();

            if (exists)
            {
                Header = IndexFile.Read(Header);
                Allocator = IndexFile.Read(Allocator);
                Statistic = IndexFile.Read(Statistic);
                HashMap = IndexFile.Read(HashMap);
            }
            else
            {
                IndexFile.Write(Header);
                IndexFile.Write(Allocator);
                IndexFile.Write(Statistic);
                IndexFile.Write(HashMap);
            }

            IndexFile.Flush();
        }

        /// <summary>
        /// Adds an item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Add(T item)
        {
            var chunkSize = (int)IndexStorageSegmentChunk.ChunkSize;
            var json = JsonSerializer.Serialize(item);
            var bytes = CompressString(json);
            var previousSegment = default(IIndexStorageSegmentChunk);

            for (int i = 0; i < bytes.Length; i += chunkSize)
            {
                var chunk = bytes.Skip(i).Take(chunkSize).ToArray();
                var segment = default(IIndexStorageSegmentChunk);

                if (i == 0)
                {
                    segment = new IndexStorageSegmentItem(StorageContext, Allocator.Alloc(IndexStorageSegmentItem.SegmentSize))
                    {
                        Id = item.Id,
                        DataChunk = chunk
                    };

                    if (HashMap.Add(segment as IndexStorageSegmentItem) == segment)
                    {
                        Statistic.Count++;
                        IndexFile.Write(Statistic);
                    }
                }
                else
                {
                    segment = new IndexStorageSegmentChunk(StorageContext, Allocator.Alloc(IndexStorageSegmentChunk.SegmentSize))
                    {
                        DataChunk = chunk
                    };

                    previousSegment.NextChunkAddr = segment.Addr;
                    IndexFile.Write(previousSegment);
                    IndexFile.Write(segment);
                }

                previousSegment = segment;
            }
        }

        /// <summary>
        /// Update an item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Update(T item)
        {
            Delete(item);
            Add(item);
        }

        /// <summary>
        /// Removed all data from the document store.
        /// </summary>
        public void Clear()
        {
            IndexFile.NextFreeAddr = 0;
            Header = new IndexStorageSegmentHeader(StorageContext) { Identifier = "wfi" };
            Allocator = new IndexStorageSegmentAllocatorDocumentStore(StorageContext);
            Statistic = new IndexStorageSegmentStatistic(StorageContext);
            HashMap = new IndexStorageSegmentHashMap(StorageContext, Capacity);

            Allocator.Initialization();

            IndexFile.Write(Header);
            IndexFile.Write(Allocator);
            IndexFile.Write(Statistic);
            IndexFile.Write(HashMap);

            IndexFile.Flush();
        }

        /// <summary>
        /// Remove an item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Delete(T item)
        {
            var list = HashMap.GetBucket(item.Id);

            if (!list.Any())
            {
                throw new ArgumentException();
            }

            var segment = list.SkipWhile(x => x.Id != item.Id).FirstOrDefault();

            HashMap.Remove(segment);
            foreach (var chunk in segment.ChunkSegments)
            {
                Allocator.Free(chunk);
            }
        }

        /// <summary>
        /// Returns the number of items.
        /// </summary>
        /// <returns>The number of items.</returns>
        public uint Count()
        {
            return Statistic.Count;
        }

        /// <summary>
        /// Drop the index document store.
        /// </summary>
        public void Drop()
        {
            IndexFile.Delete();
        }

        /// <summary>
        /// Returns the item.
        /// </summary>
        /// <param name="id">The id of the item.</param>
        /// <returns>The item.</returns>
        public T GetItem(Guid id)
        {
            return GetItem(HashMap.GetBucket(id).SkipWhile(x => x.Id != id).FirstOrDefault());
        }

        /// <summary>
        /// Returns the item.
        /// </summary>
        /// <param name="segment">The segment of the item.</param>
        /// <returns>The item.</returns>
        private T GetItem(IndexStorageSegmentItem segment)
        {
            if (segment == null)
            {
                return default;
            }

            var bytes = new List<byte>();
            var addr = segment.NextChunkAddr;

            bytes.AddRange(segment.DataChunk);

            while (addr != 0)
            {
                var chunk = IndexFile.Read<IndexStorageSegmentChunk>(addr, StorageContext);

                if (chunk.DataChunk != null)
                {
                    bytes.AddRange(chunk.DataChunk);
                }

                addr = chunk.NextChunkAddr;
            }

            if (!bytes.Any())
            {
                return default;
            }

            var json = DecompressString([.. bytes]);
            var item = JsonSerializer.Deserialize<T>(json);

            return item;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, 
        /// or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            IndexFile.Dispose();
        }

        /// <summary>
        /// Compresses a string using GZipStream.
        /// </summary>
        /// <param name="input">The string to be compressed.</param>
        /// <returns>A byte array containing the compressed string.</returns>
        private static byte[] CompressString(string input)
        {
            using var stream = new MemoryStream();
            using var gzip = new GZipStream(stream, CompressionMode.Compress);
            var bytes = Encoding.UTF8.GetBytes(input);

            gzip.Write(bytes, 0, bytes.Length);
            gzip.Close();

            return stream.ToArray();
        }

        /// <summary>
        /// Decompresses a byte array into a string using GZipStream.
        /// </summary>
        /// <param name="compressed">The byte array to be decompressed.</param>
        /// <returns>A string that represents the decompressed byte array.</returns>
        private static string DecompressString(byte[] compressed)
        {
            using var stream = new MemoryStream(compressed);
            using var zip = new GZipStream(stream, CompressionMode.Decompress);
            using var reader = new StreamReader(zip);

            return reader.ReadToEnd();
        }
    }
}