using System.IO;

namespace WebExpress.WebIndex.Storage
{
    /// <summary>
    /// Records statistical values that can be help to optimize the index.
    /// </summary>
    public class IndexStorageSegmentStatistic : IndexStorageSegment
    {
        /// <summary>
        /// Returns the amount of space required on the storage device.
        /// </summary>
        public const uint SegmentSize = sizeof(uint);

        /// <summary>
        /// Returns the number of items stored.
        /// </summary>
        public uint Count { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="context">The reference to the context of the index.</param>
        public IndexStorageSegmentStatistic(IndexStorageContext context)
            : base(context, context.IndexFile.Alloc(SegmentSize))
        {
        }

        /// <summary>
        /// Reads the record from the storage medium.
        /// </summary>
        /// <param name="reader">The reader for i/o operations.</param>
        public override void Read(BinaryReader reader)
        {
            Count = reader.ReadUInt32();
        }

        /// <summary>
        /// Writes the record to the storage medium.
        /// </summary>
        /// <param name="writer">The writer for i/o operations.</param>
        public override void Write(BinaryWriter writer)
        {
            writer.Write(Count);
        }
    }
}