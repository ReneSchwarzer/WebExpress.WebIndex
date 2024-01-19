using System.IO;

namespace WebExpress.WebIndex.Storage
{
    public class IndexStorageSegmentTerm : IndexStorageSegment
    {
        /// <summary>
        /// Returns or sets the number of times the term is used (postings).
        /// </summary>
        public uint Fequency { get; set; }

        /// <summary>
        /// Returns the postings.
        /// </summary>
        public IndexStorageSegmentHashMap<IndexStorageSegmentPosting> Postings { get; private set; }

        /// <summary>
        /// Returns the amount of space required on the storage device.
        /// </summary>
        public override uint Size => sizeof(uint) + Postings.Size;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">The reference to the context of the index.</param>
        public IndexStorageSegmentTerm(IndexStorageContext context)
            : base(context)
        {
            Postings = new IndexStorageSegmentHashMap<IndexStorageSegmentPosting>(Context, 10);
        }

        /// <summary>
        /// Assigns an address to the segment.
        /// </summary>
        /// <param name="addr">The address of the segment.</param>
        public override void OnAllocated(ulong addr)
        {
            base.OnAllocated(addr);

            Postings.OnAllocated(addr + Size - Postings.Size);
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

            Fequency = reader.ReadUInt32();

            Postings.Read(reader, addr + Size - Postings.Size);
        }

        /// <summary>
        /// Writes the record to the storage medium.
        /// </summary>
        /// <param name="writer">The writer for i/o operations.</param>
        public override void Write(BinaryWriter writer)
        {
            writer.BaseStream.Seek((long)Addr, SeekOrigin.Begin);

            writer.Write(Fequency);

            Postings.Write(writer);
        }
    }
}