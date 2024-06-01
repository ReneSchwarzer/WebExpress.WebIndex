using System;
using System.Collections.Generic;
using System.IO;
using WebExpress.WebIndex.Utility;

namespace WebExpress.WebIndex.Storage
{
    public class IndexStorageSegmentHashMap : IndexStorageSegment
    {
        /// <summary>
        /// Returns the amount of space required on the storage device.
        /// </summary>
        public static uint SegmentSize => sizeof(uint);

        /// <summary>
        /// A hash bucket is a range of memory in a hash table that is associated with a 
        /// specific hash value. A bucket provides a concatenated list by recording the 
        /// collisions (different keys with the same hash value).
        /// </summary>
        private ulong[] Buckets { get; set; }

        /// <summary>
        /// The number of fields (buckets) of the hash map. This should be a 
        /// prime number so that there are fewer collisions.
        /// </summary>
        public uint BucketCount { get; private set; }

        /// <summary>
        /// Returns a guard to protect against concurrent access.
        /// </summary>
        private object Guard { get; } = new object();

        /// <summary>
        /// Returns all items.
        /// </summary>
        public IEnumerable<IndexStorageSegmentItem> All
        {
            get
            {        
                #if DEBUG 
                using var profiling = Profiling.Diagnostic(); 
                #endif

                foreach (var bucket in Buckets)
                {
                    var addr = bucket;

                    while (addr != 0)
                    {
                        var item = Context.IndexFile.Read<IndexStorageSegmentItem>(addr, Context);
                        yield return item;

                        addr = item.SuccessorAddr;
                    }
                }
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">The reference to the context of the index.</param>
        /// <param name="capacity">The number of elements to be stored in the hash map.</param>
        public IndexStorageSegmentHashMap(IndexStorageContext context, uint capacity = ushort.MaxValue)
            : base(context, context.IndexFile.Alloc(SegmentSize))
        {
            #if DEBUG 
            using var profiling = Profiling.Diagnostic(); 
            #endif

            BucketCount = DeterminePrimeNumber(capacity);
            Buckets = new ulong[BucketCount];

            Context.IndexFile.Alloc(sizeof(ulong) * BucketCount);
        }

        /// <summary>
        /// Add an item.
        /// </summary>
        /// <param name="segment">The segment.</param>
        public IndexStorageSegmentItem Add(IndexStorageSegmentItem segment)
        {
            #if DEBUG 
            using var profiling = Profiling.Diagnostic(); 
            #endif

            var hash = segment.Id.GetHashCode();
            var index = (uint)hash % BucketCount;

            lock (Guard)
            {
                if (Buckets[index] == 0)
                {
                    Buckets[index] = segment.Addr;

                    Context.IndexFile.Write(this);
                    Context.IndexFile.Write(segment);
                }
                else
                {
                    // check whether it exists
                    var last = default(IndexStorageSegmentItem);
                    var count = 0U;

                    foreach (var i in GetBucket(segment.Id))
                    {
                        var compare = i.CompareTo(segment);

                        if (compare > 0)
                        {
                            break;
                        }
                        else if (compare == 0)
                        {
                            return i;
                        }

                        last = i;

                        count++;
                    }

                    if (last == null)
                    {
                        // insert at the beginning
                        var tempAddr = Buckets[index];
                        Buckets[index] = segment.Addr;
                        segment.SuccessorAddr = tempAddr;

                        Context.IndexFile.Write(this);
                        Context.IndexFile.Write(segment);
                    }
                    else
                    {
                        // insert in the correct place
                        var tempAddr = last.SuccessorAddr;
                        last.SuccessorAddr = segment.Addr;
                        segment.SuccessorAddr = tempAddr;

                        Context.IndexFile.Write(last);
                        Context.IndexFile.Write(segment);
                    }
                }
            }

            return segment;
        }

        /// <summary>
        /// Returns all items in a bucket.
        /// </summary>
        /// <param name="id">The id of the item.</param>
        /// <returns>The items in the buckets.</returns>
        public IEnumerable<IndexStorageSegmentItem> GetBucket(Guid id)
        {
            #if DEBUG 
            using var profiling = Profiling.Diagnostic(); 
            #endif

            var hash = id.GetHashCode();
            var index = (uint)hash % BucketCount;

            if (Buckets[index] == 0)
            {
                yield break;
            }

            var addr = Buckets[index];

            while (addr != 0)
            {
                var item = Context.IndexFile.Read<IndexStorageSegmentItem>(addr, Context);
                yield return item;

                addr = item.SuccessorAddr;
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the list.
        /// </summary>
        /// <param name="segment">The object to remove from the list.</param>
        /// <returns>True if item was successfully removed from the list, 
        /// otherwise false. This method also returns false if item is not 
        /// found in the list.</returns>
        public bool Remove(IndexStorageSegmentItem segment)
        {
            #if DEBUG 
            using var profiling = Profiling.Diagnostic(); 
            #endif

            var hash = segment.Id.GetHashCode();
            var index = (uint)hash % BucketCount;

            lock (Guard)
            {
                var predecessor = GetPredecessor(segment, out _);

                if (predecessor == null)
                {
                    Buckets[index] = segment.SuccessorAddr;

                    Context.IndexFile.Write(this);
                    Context.IndexFile.Write(segment);
                }
                else
                {
                    predecessor.SuccessorAddr = segment.SuccessorAddr;
                    Context.IndexFile.Write(predecessor);
                    segment.SuccessorAddr = 0;
                }

                Context.Allocator.Free(segment);
            }

            return true;
        }

        /// <summary>
        /// Returns the predecessor.
        /// </summary>
        /// <param name="item">The segment whose predecessor is to be determined.</param>
        /// <param name="index">The index.</param>
        /// <returns>The predecessor or null if there is no predecessor.</returns>
        private IndexStorageSegmentItem GetPredecessor(IndexStorageSegmentItem item, out uint index)
        {
            #if DEBUG 
            using var profiling = Profiling.Diagnostic(); 
            #endif

            var last = default(IndexStorageSegmentItem);
            index = 0u;

            foreach (var i in GetBucket(item.Id))
            {
                var compare = i.CompareTo(item);

                if (compare > 0)
                {
                    break;
                }
                else if (compare == 0)
                {
                    return last;
                }

                last = i;
                index++;
            }

            return last;
        }

        /// <summary>
        /// Reads the record from the storage medium.
        /// </summary>
        public override void Read(BinaryReader reader)
        {
            #if DEBUG 
            using var profiling = Profiling.Diagnostic(); 
            #endif

            BucketCount = reader.ReadUInt32();
            Buckets = new ulong[BucketCount];

            for (uint i = 0; i < BucketCount; i++)
            {
                Buckets[i] = reader.ReadUInt64();
            }
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

            writer.Write(BucketCount);

            for (int i = 0; i < BucketCount; i++)
            {
                writer.Write(Buckets[i]);
            }
        }

        /// <summary>
        /// Calculates the next prime number.
        /// </summary>
        /// <param name="capacity"></param>
        /// <returns>The next prime number.</returns>
        private static uint DeterminePrimeNumber(uint capacity)
        {
            #if DEBUG 
            using var profiling = Profiling.Diagnostic(); 
            #endif
            
            for (uint i = capacity; i <= uint.MaxValue; i++)
            {
                if (i < 2)
                {
                    return 2;
                }

                var isPrimeNumber = true;

                for (int j = 2; j <= Math.Sqrt(i); j++)
                {
                    if (i % j == 0)
                    {
                        isPrimeNumber = false;
                    }
                }

                if (isPrimeNumber)
                {
                    return i;
                }
            }

            return 65537;
        }
    }
}