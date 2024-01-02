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
    /// Implementation of the web forward index, which stores the key-value pairs on disk.
    /// </summary>
    /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
    public class IndexStorageForward<T> : IIndexForward<T>, IIndexStorage where T : IIndexItem
    {
        /// <summary>
        /// Returns the file name for the reverse index.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Returns or sets the reverse index file.
        /// </summary>
        public IndexStorageFile IndexFile { get; private set; }

        /// <summary>
        /// Returns or sets the header.
        /// </summary>
        public IndexStorageSegmentHeader Header { get; private set; }

        /// <summary>
        /// Returns or sets the hash map.
        /// </summary>
        public IndexStorageSegmentHashMap<IndexStorageSegmentItem> HashMap { get; private set; }

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
        /// Returns all items.
        /// </summary>
        public IEnumerable<T> All => HashMap.All.Select(x => GetItem(x));

        /// <summary>
        /// Returns or sets the predicted capacity (number of items to store) of the reverse index.
        /// </summary>
        private uint Capacity { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">The index context.</param>
        /// <param name="capacity">The predicted capacity (number of items to store) of the reverse index.</param>
        public IndexStorageForward(IIndexContext context, uint capacity)
        {
            Context = context;
            Capacity = capacity;
            FileName = Path.Combine(Context.IndexDirectory, $"{typeof(T).Name}.wfi");

            var exists = File.Exists(FileName);
            IndexFile = new IndexStorageFile(FileName);
            Header = new IndexStorageSegmentHeader(new IndexStorageContext(this)) { Identifier = "wfi" };
            Allocator = new IndexStorageSegmentAllocator(new IndexStorageContext(this));
            Statistic = new IndexStorageSegmentStatistic(new IndexStorageContext(this));
            HashMap = new IndexStorageSegmentHashMap<IndexStorageSegmentItem>(new IndexStorageContext(this), Capacity);

            Allocator.Alloc(Statistic);
            Allocator.Alloc(HashMap);

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
            var json = JsonSerializer.Serialize(item);
            var bytes = CompressString(json);
            var segment = new IndexStorageSegmentItem(new IndexStorageContext(this))
            {
                Id = item.Id,
                Data = bytes
            };

            HashMap[item.Id].Add(segment);
        }

        /// <summary>
        /// Removed all data from the index.
        /// </summary>
        public void Clear()
        {
            Header = new IndexStorageSegmentHeader(new IndexStorageContext(this)) { Identifier = "wfi" };
            Allocator = new IndexStorageSegmentAllocator(new IndexStorageContext(this));
            Statistic = new IndexStorageSegmentStatistic(new IndexStorageContext(this));
            HashMap = new IndexStorageSegmentHashMap<IndexStorageSegmentItem>(new IndexStorageContext(this), Capacity);

            Allocator.Alloc(Statistic);
            Allocator.Alloc(HashMap);

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
        public void Remove(T item)
        {
            var list = HashMap[item.Id];
            var segmnt = list.SkipWhile(x => x.Id != item.Id).FirstOrDefault();

            HashMap[item.Id].Remove(segmnt);
        }

        /// <summary>
        /// Returns the item.
        /// </summary>
        /// <param name="id">The id of the item.</param>
        /// <returns>The item.</returns>
        public T GetItem(Guid id)
        {
            return GetItem(HashMap[id].SkipWhile(x => x.Id != id).FirstOrDefault());
        }

        /// <summary>
        /// Returns the item.
        /// </summary>
        /// <param name="id">The segment of the item.</param>
        /// <returns>The item.</returns>
        private static T GetItem(IndexStorageSegmentItem segment)
        {
            var bytes = segment?.Data;

            if (bytes == null)
            {
                return default;
            }

            var json = DecompressString(bytes);
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