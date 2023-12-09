using System.IO;
using System.Linq;

namespace WebExpress.WebIndex.Storage
{
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
        /// Returns or sets the number of times the term is used (postings).
        /// </summary>
        public uint Fequency { get; set; }

        /// <summary>
        /// Returns or sets the id of the item.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Returns or sets the item data.
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// Returns the amount of space required on the storage device.
        /// </summary>
        public override uint Size => sizeof(ulong) + sizeof(uint) + sizeof(uint) + sizeof(int) + Length;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">The reference to the context of the index.</param>
        public IndexStorageSegmentItem(IndexStorageContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Assigns an address to the segment.
        /// </summary>
        /// <param name="addr">The address of the segment.</param>
        public override void OnAllocated(ulong addr)
        {
            base.OnAllocated(addr);
        }

        /// <summary>
        /// Reads the record from the storage medium.
        /// </summary>
        /// <param name="reader">The reader for i/o operations.</param>
        /// <param name="addr">The address of the segment.</param>
        public override void Read(BinaryReader reader, ulong addr)
        {
            Addr = addr;
            reader.BaseStream.Seek((long)Addr, SeekOrigin.Begin);

            SuccessorAddr = reader.ReadUInt64();
            Fequency = reader.ReadUInt32();
            Id = reader.ReadInt32();
            var length = reader.ReadUInt32();
            Data = reader.ReadBytes((int)length);
        }

        /// <summary>
        /// Writes the record to the storage medium.
        /// </summary>
        /// <param name="writer">The writer for i/o operations.</param>
        public override void Write(BinaryWriter writer)
        {
            writer.BaseStream.Seek((long)Addr, SeekOrigin.Begin);

            writer.Write(SuccessorAddr);
            writer.Write(Fequency);
            writer.Write(Id);
            writer.Write(Length);
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
            if (obj is IndexStorageSegmentItem item)
            {
                return Data.SequenceEqual(item.Data) ? 0 : -1;
            }

            throw new System.ArgumentException();
        }
    }
}