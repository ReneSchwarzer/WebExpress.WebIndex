using System.Collections.Generic;
using System.IO;

namespace WebExpress.WebIndex.Storage
{
    /// <summary>
    /// The Allocator for reverse index is a mechanism for reserving and freeing up space. 
    /// </summary>
    public class IndexStorageSegmentAllocatorReverseIndex : IndexStorageSegmentAllocator
    {
        /// <summary>
        /// Returns the amount of space required on the storage device.
        /// </summary>
        public new const uint SegmentSize = IndexStorageSegmentAllocator.SegmentSize + sizeof(ulong) + sizeof(ulong) + sizeof(ulong);

        /// <summary>
        /// Returns or sets the adress pointer to the free list.
        /// </summary>
        public ulong FreeTermAddr { get; set; }

        /// <summary>
        /// Returns or sets the adress pointer to the free list.
        /// </summary>
        public ulong FreePostingAddr { get; set; }

        /// <summary>
        /// Returns or sets the adress pointer to the free list.
        /// </summary>
        public ulong FreePositionAddr { get; set; }

        /// <summary>
        /// Returns a guard to protect against concurrent access.
        /// </summary>
        private object Guard { get; } = new object();

        /// <summary>
        /// Returns the a sorted list of the free term segments.
        /// </summary>
        public IEnumerable<IndexStorageSegmentFree> FreeTerms
        {
            get
            {
                if (FreeTermAddr == 0)
                {
                    yield break;
                }

                var addr = FreeTermAddr;

                while (addr != 0)
                {
                    var item = Context.IndexFile.Read<IndexStorageSegmentFree>(addr, Context);
                    yield return item;

                    addr = item.SuccessorAddr;
                }
            }
        }

        /// <summary>
        /// Returns the a sorted list of the free posting segments.
        /// </summary>
        public IEnumerable<IndexStorageSegmentFree> FreePostings
        {
            get
            {
                if (FreePostingAddr == 0)
                {
                    yield break;
                }

                var addr = FreePostingAddr;

                while (addr != 0)
                {
                    var item = Context.IndexFile.Read<IndexStorageSegmentFree>(addr, Context);
                    yield return item;

                    addr = item.SuccessorAddr;
                }
            }
        }

        /// <summary>
        /// Returns the a sorted list of the free posting segments.
        /// </summary>
        public IEnumerable<IndexStorageSegmentFree> FreePositions
        {
            get
            {
                if (FreePositionAddr == 0)
                {
                    yield break;
                }

                var addr = FreePositionAddr;

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
        public IndexStorageSegmentAllocatorReverseIndex(IndexStorageContext context)
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
        /// <param name="segment">The size determines how much memory should be reserved.</param>
        /// <returns>The start address of the reserved storage area.</returns>
        public override ulong Alloc(uint size)
        {
            lock (Guard)
            {
                switch (size)
                {
                    case IndexStorageSegmentTermNode.SegmentSize:
                        if (FreeTermAddr != 0)
                        {
                            var item = Context.IndexFile.Read<IndexStorageSegmentFree>(FreeTermAddr, Context);

                            FreeTermAddr = item.SuccessorAddr;
                            Context.IndexFile.Write(this);

                            return item.Addr;
                        }
                        break;
                    case IndexStorageSegmentPostingNode.SegmentSize:
                        if (FreePostingAddr != 0)
                        {
                            var item = Context.IndexFile.Read<IndexStorageSegmentFree>(FreePostingAddr, Context);

                            FreePostingAddr = item.SuccessorAddr;
                            Context.IndexFile.Write(this);

                            return item.Addr;
                        }
                        break;
                    case IndexStorageSegmentPosition.SegmentSize:
                        if (FreePositionAddr != 0)
                        {
                            var item = Context.IndexFile.Read<IndexStorageSegmentFree>(FreePositionAddr, Context);

                            FreePositionAddr = item.SuccessorAddr;
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
                if (segment is IndexStorageSegmentTermNode)
                {
                    var addr = FreeTermAddr;
                    FreeTermAddr = item.Addr;
                    item.SuccessorAddr = addr;

                    Context.IndexFile.Write(this);
                    Context.IndexFile.Write(item);
                }
                else if (segment is IndexStorageSegmentPostingNode)
                {
                    var addr = FreePostingAddr;
                    FreePostingAddr = item.Addr;
                    item.SuccessorAddr = addr;

                    Context.IndexFile.Write(this);
                    Context.IndexFile.Write(item);
                }
                else if (segment is IndexStorageSegmentPosition)
                {
                    var addr = FreePositionAddr;
                    FreePositionAddr = item.Addr;
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

            FreeTermAddr = reader.ReadUInt64();
            FreePostingAddr = reader.ReadUInt64();
            FreePositionAddr = reader.ReadUInt64();
        }

        /// <summary>
        /// Writes the record to the storage medium.
        /// </summary>
        /// <param name="writer">The writer for i/o operations.</param>
        public override void Write(BinaryWriter writer)
        {
            base.Write(writer);

            writer.Write(FreeTermAddr);
            writer.Write(FreePostingAddr);
            writer.Write(FreePositionAddr);
        }

        /// <summary>
        /// Converts the current object to a string.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"[{string.Join(", ", FreeTerms)}];[{string.Join(", ", FreePostings)}];[{string.Join(", ", FreePositions)}]";
        }
    }
}