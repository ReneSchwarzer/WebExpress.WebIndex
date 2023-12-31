using System;
using System.ComponentModel;
using System.IO;

namespace WebExpress.WebIndex.Storage
{
    public class WebIndexStorageSegmentPosting : WebIndexStorageSegment, IWebIndexStorageSegmentListItem
    {
        /// <summary>
        /// Returns or sets the address of the following posting.
        /// </summary>
        public ulong SuccessorAddr { get; set; }

        /// <summary>
        /// Returns or sets the document id.
        /// </summary>
        public Guid DocumentID { get; set; }

        /// <summary>
        /// Returns the position list.
        /// </summary>
        public WebIndexStorageSegmentList<WebIndexStorageSegmentPosition> Positions { get; private set; }

        /// <summary>
        /// Returns the amount of space required on the storage device.
        /// SuccessorAddr + Id
        /// </summary>
        public override uint Size => sizeof(ulong) + 16 + WebIndexStorageSegmentList<WebIndexStorageSegmentPosition>.SegmentSize;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">The reference to the context of the index.</param>
        public WebIndexStorageSegmentPosting(WebIndexStorageContext context)
            : base(context)
        {
            Positions = new WebIndexStorageSegmentList<WebIndexStorageSegmentPosition>(context, ListSortDirection.Descending);
        }

        /// <summary>
        /// Assigns an address to the segment.
        /// </summary>
        /// <param name="addr">The address of the segment.</param>
        public override void OnAllocated(ulong addr)
        {
            base.OnAllocated(addr);

            Positions.OnAllocated(addr + Size - Positions.Size);
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
            DocumentID = new Guid(reader.ReadBytes(16));

            Positions.Read(reader, addr + Size - Positions.Size);
        }

        /// <summary>
        /// Writes the record to the storage medium.
        /// </summary>
        /// <param name="writer">The writer for i/o operations.</param>
        public override void Write(BinaryWriter writer)
        {
            writer.BaseStream.Seek((long)Addr, SeekOrigin.Begin);

            writer.Write(SuccessorAddr);
            writer.Write(DocumentID.ToByteArray());

            Positions.Write(writer);
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
            if (obj is WebIndexStorageSegmentPosting posting)
            {
                return DocumentID.CompareTo(posting.DocumentID);
            }

            throw new System.ArgumentException();
        }
    }
}