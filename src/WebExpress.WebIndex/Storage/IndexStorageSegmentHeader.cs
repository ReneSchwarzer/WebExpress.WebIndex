using System.IO;

namespace WebExpress.WebIndex.Storage
{
    /// <summary>
    /// Represents the header segment of the index storage.
    /// </summary>
    public class IndexStorageSegmentHeader : IndexStorageSegment
    {
        /// <summary>
        /// Returns the amount of space required on the storage device.
        /// </summary>
        public const uint SegmentSize = 3 + sizeof(byte);

        /// <summary>
        /// Returns or sets the file identifire.
        /// </summary>
        public string Identifier { get; internal set; }

        /// <summary>
        /// Returns or sets the file version.
        /// </summary>
        public byte Version { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="context">The reference to the context of the index.</param>
        public IndexStorageSegmentHeader(IndexStorageContext context)
            : base(context, context.IndexFile.Alloc(SegmentSize))
        {
        }

        /// <summary>
        /// Initialization method for the header segment.
        /// </summary>
        /// <param name="initializationFromFile">If true, initializes from file. Otherwise, initializes and writes to file.</param>
        public virtual void Initialization(bool initializationFromFile)
        {
            if (initializationFromFile)
            {
                Context.IndexFile.Read(this);
            }
            else
            {
                Context.IndexFile.Write(this);
            }
        }

        /// <summary>
        /// Reads the record from the storage medium.
        /// </summary>
        /// <param name="reader">The reader for i/o operations.</param>
        public override void Read(BinaryReader reader)
        {
            var identifier = new string(reader.ReadChars(3));
            var version = reader.ReadByte();

            if (!Identifier.Equals(identifier))
            {
                throw new IOException($"A file with the identifier '{Identifier}' is expected. However, '{identifier}' was read.");
            }

            if (!Version.Equals(version))
            {
                throw new IOException($"The expected file version is '{Version}', but version '{version}' was read.");
            }
        }

        /// <summary>
        /// Writes the record to the storage medium.
        /// </summary>
        /// <param name="writer">The writer for i/o operations.</param>
        public override void Write(BinaryWriter writer)
        {
            writer.Write((byte)Identifier[0]);
            writer.Write((byte)Identifier[1]);
            writer.Write((byte)Identifier[2]);
            writer.Write(Version);
        }
    }
}