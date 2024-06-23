using System;
using System.IO;
using System.Linq;

namespace WebExpress.WebIndex.Storage
{
    /// <summary>
    /// The class represents a segment of an index storage that is divided into chunks. Each chunk contains a 
    /// portion of the data and a reference to the next chunk, creating an ordered list of chunks. 
    /// </summary>
    public class IndexStorageSegmentChunk : IndexStorageSegment, IIndexStorageSegmentChunk
    {
        /// <summary>
        /// Returns the amount of space required on the storage device.
        /// </summary>
        public const uint ChunkSize = 256;

        /// <summary>
        /// Returns the amount of space required on the storage device.
        /// </summary>
        public const uint SegmentSize = sizeof(uint) + ChunkSize + sizeof(ulong);

        /// <summary>
        /// Returns the number of characters in the term.
        /// </summary>
        public uint Length => (uint)DataChunk.Length;

        /// <summary>
        /// Returns or sets the item data. 
        /// </summary>
        public byte[] DataChunk { get; set; }

        /// <summary>
        /// Returns or sets the address of the next chunk element of a list or 0 if there is no element.
        /// </summary>
        public ulong NextChunkAddr { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">The reference to the context of the index.</param>
        /// <param name="addr">The adress of the segment.</param>
        public IndexStorageSegmentChunk(IndexStorageContext context, ulong addr)
            : base(context, addr)
        {
        }

        /// <summary>
        /// Reads the record from the storage medium.
        /// </summary>
        /// <param name="reader">The reader for i/o operations.</param>
        public override void Read(BinaryReader reader)
        {
            var length = reader.ReadUInt32();
            DataChunk = reader.ReadBytes((int)Math.Min(length, ChunkSize));
            NextChunkAddr = reader.ReadUInt64();
        }

        /// <summary>
        /// Writes the record to the storage medium.
        /// </summary>
        /// <param name="writer">The writer for i/o operations.</param>
        public override void Write(BinaryWriter writer)
        {
            writer.Write(Length);
            writer.Write(DataChunk);
            writer.Write(NextChunkAddr);
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns
        ///  an integer that indicates whether the current instance precedes, follows, or
        ///  occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// A signed integer that indicates the relative values of x and y.
        ///     Less than zero -> x is less than y.
        ///     Zero -> x equals y.
        ///     Greater than zero -> x is greater than y.
        /// </returns>
        /// <exception cref="System.ArgumentException">Obj is not the same type as this instance.</exception>
        public int CompareTo(object obj)
        {
            if (obj is IndexStorageSegmentItem item)
            {
                return DataChunk.SequenceEqual(item.DataChunk) ? 0 : -1;
            }

            throw new System.ArgumentException();
        }
    }
}