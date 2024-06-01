using System;
using System.IO;
using System.Linq;
using WebExpress.WebIndex.Utility;

namespace WebExpress.WebIndex.Storage
{
    /// <summary>
    /// The items are stored in a segment. The size of the segment is variable and is determined by the size of the compressed 
    /// item instance. The segment are stored in the variable memory area of the IndexDocumentStore.
    /// </summary>
    public class IndexStorageSegmentItem : IndexStorageSegment, IIndexStorageSegmentListItem
    {
        /// <summary>
        /// Returns or sets the address of the following term.
        /// </summary>
        public ulong SuccessorAddr { get; set; }

        /// <summary>
        /// Returns the number of characters in the term.
        /// </summary>
        public uint Length => (uint)Data.Length;

        /// <summary>
        /// Returns or sets the id of the item.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Returns or sets the item data.
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// Returns the amount of space required on the storage device.
        /// </summary>
        public static uint SegmentSize => sizeof(ulong) + sizeof(uint) + sizeof(uint) + 16;

        /// <summary>
        /// Returns the amount of space required on the storage device.
        /// SuccessorAddr + Length + Fequency + DocumentID + Data
        /// </summary>
        public uint Size => SegmentSize + Length;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">The reference to the context of the index.</param>
        /// <param name="addr">The adress of the segment.</param>
        public IndexStorageSegmentItem(IndexStorageContext context, ulong addr)
            : base(context, addr)
        {
        }

        /// <summary>
        /// Reads the record from the storage medium.
        /// </summary>
        /// <param name="reader">The reader for i/o operations.</param>
        public override void Read(BinaryReader reader)
        {
            #if DEBUG 
            using var profiling = Profiling.Diagnostic(); 
            #endif
            
            Id = new Guid(reader.ReadBytes(16));
            var length = reader.ReadUInt32();
            SuccessorAddr = reader.ReadUInt64();
            Data = reader.ReadBytes((int)length);
        }

        /// <summary>
        /// Writes the record to the storage medium.
        /// </summary>
        /// <param name="writer">The writer for i/o operations.</param>
        public override void Write(BinaryWriter writer)
        {
            #if DEBUG 
            using var profiling = Profiling.Diagnostic(); 
            #endif

            writer.Write(Id.ToByteArray());
            writer.Write(Length);
            writer.Write(SuccessorAddr);
            writer.Write(Data);
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
            #if DEBUG 
            using var profiling = Profiling.Diagnostic(); 
            #endif
            
            if (obj is IndexStorageSegmentItem item)
            {
                return Data.SequenceEqual(item.Data) ? 0 : -1;
            }

            throw new System.ArgumentException();
        }
    }
}