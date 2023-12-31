using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WebExpress.WebIndex.Storage
{
    public class WebIndexStorageSegmentHashMap<T> : WebIndexStorageSegment
        where T : IWebIndexStorageSegmentListItem
    {
        /// <summary>
        /// A hash bucket is a range of memory in a hash table that is associated with a 
        /// specific hash value. A bucket provides a concatenated list by recording the 
        /// collisions (different keys with the same hash value).
        /// </summary>
        private readonly WebIndexStorageSegmentList<T>[] Buckets;

        /// <summary>
        /// The number of fields (buckets) of the hash map. This should be a 
        /// prime number so that there are fewer collisions.
        /// </summary>
        public uint BucketCount { get; private set; }

        /// <summary>
        /// Returns the amount of space required on the storage device.
        /// </summary>
        public override uint Size => sizeof(uint) + (WebIndexStorageSegmentList<T>.SegmentSize * BucketCount);

        /// <summary>
        /// Returns all items.
        /// </summary>
        public IEnumerable<T> All => Buckets.SelectMany(x => x);

        /// <summary>
        /// Returns or sets the address of the first term in a bucket in the hash map.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <returns>The address in the bucket at the index.</returns>
        public WebIndexStorageSegmentList<T> this[object term]
        {
            get
            {
                var index = (uint)term.GetHashCode() % BucketCount;
                return Buckets[index];
            }
            set { Buckets[term.GetHashCode() % BucketCount] = value; }
        }

        /// <summary>
        /// Returns or sets the address of the first term in a bucket in the hash map.
        /// </summary>
        /// <param name="term">The id.</param>
        /// <returns>The address in the bucket at the index.</returns>
        public WebIndexStorageSegmentList<T> this[int id]
        {
            get
            {
                var index = (uint)id % BucketCount;
                return Buckets[index];
            }
            set { Buckets[id.GetHashCode() % BucketCount] = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">The reference to the context of the index.</param>
        /// <param name="capacity">The number of elements to be stored in the hash map.</param>
        public WebIndexStorageSegmentHashMap(WebIndexStorageContext context, uint capacity)
            : base(context)
        {
            BucketCount = DeterminePrimeNumber(capacity);
            Buckets = new WebIndexStorageSegmentList<T>[BucketCount];
        }

        /// <summary>
        /// Assigns an address to the segment.
        /// </summary>
        /// <param name="addr">The address of the segment.</param>
        public override void OnAllocated(ulong addr)
        {
            base.OnAllocated(addr);

            var offset = addr + sizeof(uint); // BucketCount

            for (uint i = 0; i < BucketCount; i++)
            {
                Buckets[i] = new WebIndexStorageSegmentList<T>(Context);
                Buckets[i].OnAllocated(offset);

                offset += WebIndexStorageSegmentList<T>.SegmentSize;
            }
        }

        /// <summary>
        /// Reads the record from the storage medium.
        /// </summary>
        /// <param name="reader">The reader for i/o operations.</param>
        /// <param name="addr">The address of the segment.</param>
        public override void Read(BinaryReader reader, ulong addr)
        {
            Addr = addr;
            reader.BaseStream.Seek((long)Addr, SeekOrigin.Begin);

            BucketCount = reader.ReadUInt32();

            var offset = addr + sizeof(uint); // BucketCount

            for (uint i = 0; i < BucketCount; i++)
            {
                Buckets[i] = Context.IndexFile.Read<WebIndexStorageSegmentList<T>>(offset, Context);

                offset += WebIndexStorageSegmentList<T>.SegmentSize;
            }
        }

        /// <summary>
        /// Writes the record to the storage medium.
        /// </summary>
        /// <param name="writer">The writer for i/o operations.</param>
        public override void Write(BinaryWriter writer)
        {
            writer.BaseStream.Seek((long)Addr, SeekOrigin.Begin);

            writer.Write(BucketCount);

            for (int i = 0; i < BucketCount; i++)
            {
                Context.IndexFile.Write(Buckets[i]);
            }
        }

        /// <summary>
        /// Calculates the next prime number.
        /// </summary>
        /// <param name="capacity"></param>
        /// <returns>The next prime number.</returns>
        private static uint DeterminePrimeNumber(uint capacity)
        {
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