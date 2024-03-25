using System.Collections.Generic;
using System.IO;

namespace WebExpress.WebIndex.Storage
{
    /// <summary>
    /// The Allocator is a mechanism for reserving and freeing up space. 
    /// </summary>
    public class IndexStorageSegmentAllocator : IndexStorageSegment
    {
        /// <summary>
        /// Returns or sets the next free address.
        /// </summary>
        private ulong NextFreeAddr { get; set; } = 0ul;

        /// <summary>
        /// Returns or sets the adress pointer to the free list.
        /// </summary>
        public ulong FreeListAddr { get; set; }

        /// <summary>
        /// Returns the amount of space required on the storage device.
        /// </summary>
        public static uint SegmentSize => sizeof(ulong) + sizeof(ulong);

        /// <summary>
        /// Returns the a sorted list of the free segments.
        /// </summary>
        public IEnumerable<IndexStorageSegmentFree> FreeSegments
        {
            get
            {
                if (FreeListAddr == 0)
                {
                    yield break;
                }

                var addr = FreeListAddr;

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
        public IndexStorageSegmentAllocator(IndexStorageContext context)
            : base(context, context.IndexFile.Alloc(SegmentSize))
        {
        }

        /// <summary>
        /// Initialization
        /// </summary>
        public void Initialization()
        {
            NextFreeAddr = Context.IndexFile.NextFreeAddr;
        }

        /// <summary>
        /// Allocate the memory.
        /// </summary>
        /// <param name="segment">The segment determines how much memory should be reserved.</param>
        /// <param name="size"><param>
        /// <returns>The start address of the reserved storage area.</returns>
        public ulong Alloc(uint size)
        {
            var last = default(IndexStorageSegmentFree);

            // find free space
            foreach (var item in FreeSegments)
            {
                if (item.Lenght == size)
                {
                    var freeAddr = Split(last, item, IndexStorageSegmentFree.SegmentSize);
                    return freeAddr;
                }

                last = item;
            }

            var addr = NextFreeAddr;
            NextFreeAddr += size;

            Context.IndexFile.Write(this);

            return addr;
        }

        /// <summary>
        /// Allocate the memory.
        /// </summary>
        /// <param name="segment">The segment determines how much memory should be reserved.</param>
        public void Free(IIndexStorageSegment segment)
        {
            var item = new IndexStorageSegmentFree(Context, segment.Addr);

            if (FreeListAddr == 0)
            {
                FreeListAddr = segment.Addr;
                Context.IndexFile.Write(this);
            }
            else
            {
                // check whether it exists
                var last = default(IndexStorageSegmentFree);
                var count = 0U;

                foreach (var i in FreeSegments)
                {
                    var compare = i.Addr.CompareTo(item.Addr);

                    if (compare > 0)
                    {
                        break;
                    }
                    else if (compare == 0)
                    {
                        return;
                    }

                    last = i;

                    count++;
                }

                if (last == null)
                {
                    // insert at the beginning
                    var tempAddr = FreeListAddr;
                    FreeListAddr = item.Addr;
                    item.SuccessorAddr = tempAddr;

                    Context.IndexFile.Write(this);
                    Context.IndexFile.Write(item);
                }
                else
                {
                    // insert in the correct place
                    var tempAddr = last.SuccessorAddr;
                    last.SuccessorAddr = item.Addr;
                    item.SuccessorAddr = tempAddr;

                    Context.IndexFile.Write(this);
                    Context.IndexFile.Write(last);
                    Context.IndexFile.Write(item);
                }
            }
        }

        /// <summary>
        /// Split a free segment.
        /// </summary>
        /// <param name="predecessor">The predecessor segment or null.</param>
        /// <param name="segment">The segment to be split.</param>
        /// <returns>The free address with the guaranteed memory.</returns>
        private ulong Split(IndexStorageSegmentFree predecessor, IndexStorageSegmentFree segment, uint size)
        {
            var free = new IndexStorageSegmentFree(Context, segment.Addr + size)
            {
                SuccessorAddr = segment.SuccessorAddr,
                Lenght = uint.MaxValue
            };

            if (predecessor != null)
            {
                predecessor.SuccessorAddr = free.Addr;
                Context.IndexFile.Write(predecessor);
            }
            else
            {
                FreeListAddr = free.Addr;
                Context.IndexFile.Write(this);
            }

            Context.IndexFile.Write(free);

            return segment.Addr;
        }

        /// <summary>
        /// Reads the record from the storage medium.
        /// </summary>
        /// <param name="reader">The reader for i/o operations.</param>
        public override void Read(BinaryReader reader)
        {
            NextFreeAddr = reader.ReadUInt64();
            FreeListAddr = reader.ReadUInt64();
        }

        /// <summary>
        /// Writes the record to the storage medium.
        /// </summary>
        /// <param name="writer">The writer for i/o operations.</param>
        public override void Write(BinaryWriter writer)
        {
            writer.Write(NextFreeAddr);
            writer.Write(FreeListAddr);
        }

        /// <summary>
        /// Converts the current object to a string.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"[{string.Join(", ", FreeSegments)}]";
        }
    }
}