using System.IO;

namespace WebExpress.WebIndex.Storage
{
    /// <summary>
    /// The Allocator is a mechanism for reserving and freeing up space. 
    /// </summary>
    public abstract class IndexStorageSegmentAllocator : IndexStorageSegment
    {
        /// <summary>
        /// Returns the amount of space required on the storage device.
        /// </summary>
        public const uint SegmentSize = sizeof(ulong);

        /// <summary>
        /// Returns or sets the next free address.
        /// </summary>
        public ulong NextFreeAddr { get; protected set; } = 0ul;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="context">The reference to the context of the index.</param>
        /// <param name="addr">The address of the segment.</param>
        public IndexStorageSegmentAllocator(IndexStorageContext context, ulong addr)
            : base(context, addr)
        {
        }

        /// <summary>
        /// Initializes the allocator segment.
        /// </summary>
        /// <param name="initializationFromFile">If true, initializes from file. Otherwise, initializes and writes to file.</param>
        public virtual void Initialization(bool initializationFromFile)
        {
            NextFreeAddr = Context.IndexFile.NextFreeAddr;

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
        /// Allocate the memory.
        /// </summary>
        /// <param name="segment">The size determines how much memory should be reserved.</param>
        /// <returns>The start address of the reserved storage area.</returns>
        public abstract ulong Alloc(uint size);

        /// <summary>
        /// Allocate the memory.
        /// </summary>
        /// <param name="segment">The segment determines how much memory should be reserved.</param>
        public abstract void Free(IIndexStorageSegment segment);

        /// <summary>
        /// Reads the record from the storage medium.
        /// </summary>
        /// <param name="reader">The reader for i/o operations.</param>
        public override void Read(BinaryReader reader)
        {
            NextFreeAddr = reader.ReadUInt64();
        }

        /// <summary>
        /// Writes the record to the storage medium.
        /// </summary>
        /// <param name="writer">The writer for i/o operations.</param>
        public override void Write(BinaryWriter writer)
        {
            writer.Write(NextFreeAddr);
        }

        /// <summary>
        /// Converts the current object to a string.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return base.ToString();
        }
    }
}