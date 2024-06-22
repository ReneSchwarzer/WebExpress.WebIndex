using System.Collections.Generic;
using System.IO;

namespace WebExpress.WebIndex.Storage
{
    /// <summary>
    /// The Allocator is a mechanism for reserving and freeing up space. 
    /// </summary>
    public class IndexStorageSegmentAllocatorDocumentStore : IndexStorageSegmentAllocator
    {
        /// <summary>
        /// Returns the amount of space required on the storage device.
        /// </summary>
        public new const uint SegmentSize = IndexStorageSegmentAllocator.SegmentSize + sizeof(ulong) + sizeof(ulong);

        /// <summary>
        /// Returns or sets the adress pointer to the free item list.
        /// </summary>
        public ulong FreeItemAddr { get; set; }

        /// <summary>
        /// Returns or sets the adress pointer to the free chunk list.
        /// </summary>
        public ulong FreeChunkAddr { get; set; }

        /// <summary>
        /// Returns a guard to protect against concurrent access.
        /// </summary>
        private object Guard { get; } = new object();

        /// <summary>
        /// Returns the a sorted list of the free items segments.
        /// </summary>
        public IEnumerable<IndexStorageSegmentFree> FreeItemSegments
        {
            get
            {
                if (FreeItemAddr == 0)
                {
                    yield break;
                }

                var addr = FreeItemAddr;

                while (addr != 0)
                {
                    var item = Context.IndexFile.Read<IndexStorageSegmentFree>(addr, Context);
                    yield return item;

                    addr = item.SuccessorAddr;
                }
            }
        }

        /// <summary>
        /// Returns the a sorted list of the free chunk segments.
        /// </summary>
        public IEnumerable<IndexStorageSegmentFree> FreeChunkSegments
        {
            get
            {
                if (FreeChunkAddr == 0)
                {
                    yield break;
                }

                var addr = FreeChunkAddr;

                while (addr != 0)
                {
                    var item = Context.IndexFile.Read<IndexStorageSegmentFree>(addr, Context);
                    yield return item;

                    addr = item.SuccessorAddr;
                }
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">The reference to the context of the index.</param>
        public IndexStorageSegmentAllocatorDocumentStore(IndexStorageContext context)
            : base(context, context.IndexFile.Alloc(SegmentSize))
        {
        }

        /// <summary>
        /// Initialization
        /// </summary>
        public override void Initialization()
        {
            base.Initialization();
        }

        /// <summary>
        /// Allocate the memory.
        /// </summary>
        /// <param name="size">The size determines how much memory should be reserved.<param>
        /// <returns>The start address of the reserved storage area.</returns>
        public override ulong Alloc(uint size)
        {
            lock (Guard)
            {
                switch (size)
                {
                    case IndexStorageSegmentItem.SegmentSize:
                        if (FreeItemAddr != 0)
                        {
                            var item = Context.IndexFile.Read<IndexStorageSegmentFree>(FreeItemAddr, Context);

                            FreeItemAddr = item.SuccessorAddr;
                            Context.IndexFile.Write(this);

                            return item.Addr;
                        }
                        break;
                    case IndexStorageSegmentChunk.SegmentSize:
                        if (FreeChunkAddr != 0)
                        {
                            var item = Context.IndexFile.Read<IndexStorageSegmentFree>(FreeChunkAddr, Context);

                            FreeChunkAddr = item.SuccessorAddr;
                            Context.IndexFile.Write(this);

                            return item.Addr;
                        }
                        break;
                }

                var addr = NextFreeAddr;
                NextFreeAddr += size;

                Context.IndexFile.Write(this);

                return addr;
            }
        }

        /// <summary>
        /// Allocate the memory.
        /// </summary>
        /// <param name="segment">The segment determines how much memory should be reserved.</param>
        public override void Free(IIndexStorageSegment segment)
        {
            var item = new IndexStorageSegmentFree(Context, segment.Addr);

            Context.IndexFile.Invalidation(segment);

            lock (Guard)
            {
                if (segment is IndexStorageSegmentItem)
                {
                    var addr = FreeItemAddr;
                    FreeItemAddr = item.Addr;
                    item.SuccessorAddr = addr;

                    Context.IndexFile.Write(this);
                    Context.IndexFile.Write(item);
                }
                else if (segment is IndexStorageSegmentChunk)
                {
                    var addr = FreeChunkAddr;
                    FreeChunkAddr = item.Addr;
                    item.SuccessorAddr = addr;

                    Context.IndexFile.Write(this);
                    Context.IndexFile.Write(item);
                }
            }
        }

        /// <summary>
        /// Reads the record from the storage medium.
        /// </summary>
        /// <param name="reader">The reader for i/o operations.</param>
        public override void Read(BinaryReader reader)
        {
            base.Read(reader);

            FreeItemAddr = reader.ReadUInt64();
            FreeItemAddr = reader.ReadUInt64();
        }

        /// <summary>
        /// Writes the record to the storage medium.
        /// </summary>
        /// <param name="writer">The writer for i/o operations.</param>
        public override void Write(BinaryWriter writer)
        {
            base.Write(writer);

            writer.Write(FreeItemAddr);
            writer.Write(FreeItemAddr);
        }

        /// <summary>
        /// Converts the current object to a string.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"[{string.Join(", ", FreeItemSegments)}];[{string.Join(", ", FreeChunkSegments)}]";
        }
    }
}