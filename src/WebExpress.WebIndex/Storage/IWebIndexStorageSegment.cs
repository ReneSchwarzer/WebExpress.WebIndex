using System.IO;

namespace WebExpress.WebIndex.Storage
{
    public interface IWebIndexStorageSegment
    {
        /// <summary>
        /// Returns the address of the segment.
        /// </summary>
        ulong Addr { get; }

        /// <summary>
        /// Returns the the context of the index.
        /// </summary>
        WebIndexStorageContext Context { get; }

        /// <summary>
        /// Returns the amount of space required on the storage device.
        /// </summary>
        uint Size { get; }

        /// <summary>
        /// Assigns an address to the segment.
        /// </summary>
        /// <<param name="addr">The address of the segment.</param>
        void OnAllocated(ulong addr);

        /// <summary>
        /// Reads the record from the storage medium.
        /// </summary>
        /// <param name="reader">The reader for i/o operations.</param>
        /// <param name="addr">The address of the segment.</param>
        void Read(BinaryReader reader, ulong addr);

        /// <summary>
        /// Writes the record to the storage medium.
        /// </summary>
        /// <param name="writer">The writer for i/o operations.</param>
        void Write(BinaryWriter writer);
    }
}