using System.IO;

namespace WebExpress.WebIndex.Storage
{
    public abstract class IndexStorageSegment : IIndexStorageSegment
    {
        /// <summary>
        /// Returns the address of the segment.
        /// </summary>
        public virtual ulong Addr { get; private set; }

        /// <summary>
        /// Returns the the context of the index.
        /// </summary>
        public IndexStorageContext Context { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">The reference to the context of the index.</param>
        /// <param name="addr">The address of the segment.</param>
        public IndexStorageSegment(IndexStorageContext context, ulong addr)
        {
            Context = context;
            Addr = addr;
        }

        /// <summary>
        /// Reads the record from the storage medium.
        /// </summary>
        /// <param name="reader">The reader for i/o operations.</param>
        public abstract void Read(BinaryReader reader);

        /// <summary>
        /// Writes the record to the storage medium.
        /// </summary>
        /// <param name="writer">The writer for i/o operations.</param>
        public abstract void Write(BinaryWriter writer);
    }
}