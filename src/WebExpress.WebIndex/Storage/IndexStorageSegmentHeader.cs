using System.IO;
using System.Reflection.Metadata;

namespace WebExpress.WebIndex.Storage
{
    public class IndexStorageSegmentHeader : IndexStorageSegment
    {
        /// <summary>
        /// Returns or sets the file identifire.
        /// </summary>
        public string Identifier { get; internal set; }

        /// <summary>
        /// Returns the amount of space required on the storage device.
        /// </summary>
        public static uint SegmentSize => 3;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">The reference to the context of the index.</param>
        public IndexStorageSegmentHeader(IndexStorageContext context)
            : base(context, context.IndexFile.Alloc(SegmentSize))
        {

        }

        /// <summary>
        /// Reads the record from the storage medium.
        /// </summary>
        /// <param name="reader">The reader for i/o operations.</param>
        public override void Read(BinaryReader reader)
        {
            Identifier = new string(reader.ReadChars((int)SegmentSize));
        }

        /// <summary>
        /// Writes the record to the storage medium.
        /// </summary>
        /// <param name="writer">The writer for i/o operations.</param>
        public override void Write(BinaryWriter writer)
        {
            writer.Write(Identifier.ToCharArray(0, (int)SegmentSize));
        }
    }
}