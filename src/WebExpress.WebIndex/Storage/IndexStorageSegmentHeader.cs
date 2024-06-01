using System.IO;
using WebExpress.WebIndex.Utility;

namespace WebExpress.WebIndex.Storage
{
    public class IndexStorageSegmentHeader : IndexStorageSegment
    {
        /// <summary>
        /// Returns or sets the file identifire.
        /// </summary>
        public string Identifier { get; internal set; }

        /// <summary>
        /// Returns or sets the file version.
        /// </summary>
        public byte Version { get; internal set; }

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
            #if DEBUG 
            using var profiling = Profiling.Diagnostic(); 
            #endif
            
            Identifier = new string(reader.ReadChars((int)SegmentSize));
            Version = reader.ReadByte();
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
            
            writer.Write(Identifier.ToCharArray(0, (int)SegmentSize));
            writer.Write(Version);
        }
    }
}