﻿using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace WebExpress.WebIndex.Storage
{
    /// <summary>
    /// A simple concatenated sorted list.
    /// </summary>
    /// <typeparam name="T">The list item type.</typeparam>
    public class IndexStorageSegmentList<T> : IndexStorageSegment, IEnumerable<T>
        where T : IIndexStorageSegmentListItem
    {
        /// <summary>
        /// Returns or sets the address of the first item in the list, or null if the 
        /// list is empty.
        /// </summary>
        public ulong HeadAddr { get; set; }

        /// <summary>
        /// Returns the amount of space required on the storage device.
        /// </summary>
        public override uint Size => SegmentSize;

        /// <summary>
        /// Returns the amount of space required on the storage device.
        /// </summary>
        public static uint SegmentSize => sizeof(ulong);

        /// <summary>
        /// Checks if the list is empty.
        /// </summary>
        public bool IsEmpty => HeadAddr == 0;

        /// <summary>
        /// Returns or sets the sort order of the list.
        /// </summary>
        public ListSortDirection SortDirection { get; private set; }

        /// <summary>
        /// Gets the number of elements contained in the list. Determining 
        /// the number of stored items has an effort of o(n).
        /// </summary>
        public int Count
        {
            get
            {
                var index = 0;

                foreach (var i in this)
                {
                    index++;
                }

                return index;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">The reference to the context of the index.</param>
        /// <param name="sortDirection">The sort order of the list.</param>
        public IndexStorageSegmentList(IndexStorageContext context)
            : base(context)
        {
            SortDirection = ListSortDirection.Ascending;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">The reference to the context of the index.</param>
        /// <param name="sortDirection">The sort order of the list.</param>
        public IndexStorageSegmentList(IndexStorageContext context, ListSortDirection sortDirection)
            : base(context)
        {
            SortDirection = sortDirection;
        }

        /// <summary>
        /// Adds an item to the list if it doesn't already exist.
        /// </summary>
        /// <param name="item">The item to be added.</param>
        /// <returns>The new item, if it is not already in the list, otherwise the existing item.</returns>
        public T Add(T item)
        {
            if (HeadAddr == 0)
            {
                HeadAddr = Context.Allocator.Alloc(item);

                Context.IndexFile.Write(this);
                Context.IndexFile.Write(item);
            }
            else
            {
                var last = default(T);
                var count = 0U;

                foreach (var i in this)
                {
                    var compare = i.CompareTo(item);

                    if (compare > 0 && SortDirection == ListSortDirection.Ascending)
                    {
                        break;
                    }
                    else if (compare < 0 && SortDirection == ListSortDirection.Descending)
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
                    var tempAddr = HeadAddr;
                    HeadAddr = Context.Allocator.Alloc(item);
                    item.SuccessorAddr = tempAddr;

                    Context.IndexFile.Write(this);
                    Context.IndexFile.Write(item);
                }
                else
                {
                    var tempAddr = last.SuccessorAddr;
                    last.SuccessorAddr = Context.Allocator.Alloc(item);
                    item.SuccessorAddr = tempAddr;

                    Context.IndexFile.Write(this);
                    Context.IndexFile.Write(last);
                    Context.IndexFile.Write(item);

                    Context.Statistic.AddCollision(item, count);
                }
            }

            return item;
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

            HeadAddr = reader.ReadUInt64();
        }

        /// <summary>
        /// Writes the record to the storage medium.
        /// </summary>
        /// <param name="writer">The writer for i/o operations.</param>
        public override void Write(BinaryWriter writer)
        {
            writer.BaseStream.Seek((long)Addr, SeekOrigin.Begin);

            writer.Write(HeadAddr);
        }

        /// <summary>
        /// Removes all items from the list.
        /// </summary>
        public void Clear()
        {
            foreach (var item in this)
            {
                Context.Allocator.Free(item);
            }

            HeadAddr = 0;
            Context.IndexFile.Write(this);
        }

        /// <summary>
        /// Determines whether the list contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the list..</param>
        /// <returns>True if item is found in the list, otherwise false.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool Contains(T item)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the list.
        /// </summary>
        /// <param name="item">The object to remove from the list.</param>
        /// <returns>True if item was successfully removed from the list, 
        /// otherwise false. This method also returns false if item is not 
        /// found in the list.</returns>
        public bool Remove(T item)
        {
            var predecessor = GetPredecessor(item, out uint count);

            if (predecessor == null)
            {
                HeadAddr = item.SuccessorAddr;

                Context.IndexFile.Write(this);
                Context.IndexFile.Write(item);
            }
            else
            {
                predecessor.SuccessorAddr = item.SuccessorAddr;
                Context.IndexFile.Write(predecessor);
                Context.Allocator.Free(item);

                item.SuccessorAddr = 0;
            }

            return true;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            if (IsEmpty)
            {
                yield break;
            }

            var addr = HeadAddr;
            var item = default(T);

            do
            {
                item = Context.IndexFile.Read<T>(addr, Context);

                yield return item;

            } while ((addr = item.SuccessorAddr) != 0);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An IEnumerator object that can be used to iterate through the list.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Converts the current object to a string.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return Count.ToString();
        }

        /// <summary>
        /// Returns the predecessor.
        /// </summary>
        /// <param name="item">The segment whose predecessor is to be determined.</param>
        /// <param name="index">The index.</param>
        /// <returns>The predecessor or null if there is no predecessor.</returns>
        private T GetPredecessor(T item, out uint index)
        {
            var last = default(T);
            index = 0U;

            foreach (var i in this)
            {
                var compare = i.CompareTo(item);

                if (compare > 0 && SortDirection == ListSortDirection.Ascending)
                {
                    break;
                }
                else if (compare < 0 && SortDirection == ListSortDirection.Descending)
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
    }
}