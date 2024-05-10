﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace WebExpress.WebIndex.Storage
{
    /// <summary>
    /// The posting segment is designed as a list and contains the IDs of the documents that belong to a term. For each document, the 
    /// posting segment refers to the position information that indicates where the term is located in the document. The posting 
    /// segment is stored in the variable memory area of the inverted index.
    /// </summary>
    public class IndexStorageSegmentPosting : IndexStorageSegment, IIndexStorageSegmentListItem
    {
        /// <summary>
        /// Returns or sets the document id.
        /// </summary>
        public Guid DocumentID { get; set; }
        /// <summary>
        /// Returns or sets the address of the following posting.
        /// </summary>
        public ulong SuccessorAddr { get; set; }

        /// <summary>
        /// Returns the adress of the first position element of a sorted list or 0 if there is no element.
        /// </summary>
        public ulong PositionAddr { get; private set; }

        /// <summary>
        /// Returns the amount of space required on the storage device.
        /// </summary>
        public static uint SegmentSize =>  16 + sizeof(ulong) + sizeof(ulong);

        /// <summary>
        /// Returns the a sorted list of the positions or no element.
        /// </summary>
        public IEnumerable<IndexStorageSegmentPosition> Positions 
        {
            get 
            {
                if (PositionAddr == 0)
                {
                    yield break;
                }

                var addr = PositionAddr;

                while (addr != 0)
                {
                    var item = Context.IndexFile.Read<IndexStorageSegmentPosition>(addr, Context);
                    yield return item;

                    addr = item.SuccessorAddr;
                }
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">The reference to the context of the index.</param>
        /// <param name="addr">The adress of the segment.</param>
        public IndexStorageSegmentPosting(IndexStorageContext context, ulong addr)
            : base(context, addr)
        {
        }

        /// <summary>
        /// Add a position segments.
        /// </summary>
        /// <param name="pos">The position of the term.</params>
        /// <returns>The position segment.</returns>
        public IndexStorageSegmentPosition AddPosition(uint pos)
        {
            var item = default(IndexStorageSegmentPosition);

            if (PositionAddr == 0)
            {
                PositionAddr = Context.Allocator.Alloc(IndexStorageSegmentPosition.SegmentSize);
                item = new IndexStorageSegmentPosition(Context, PositionAddr)
                {
                    Position = pos
                };

                Context.IndexFile.Write(this);
                Context.IndexFile.Write(item);
            }
            else
            {
                // check whether it exists
                var last = default(IndexStorageSegmentPosition);
                var count = 0U;

                foreach (var i in Positions)
                {
                    var compare = i.Position.CompareTo(pos);

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

                item = new IndexStorageSegmentPosition(Context, Context.Allocator.Alloc(IndexStorageSegmentPosition.SegmentSize))
                {
                    Position = pos
                };

                if (last == null)
                {
                    // insert at the beginning
                    var tempAddr = PositionAddr;
                    PositionAddr = item.Addr;
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

            return item;
        }

        /// <summary>
        /// Remove a position segments.
        /// </summary>
        /// <param name="pos">The position of the term.</param>
        /// <returns>The position segment.</returns>
        public IndexStorageSegmentPosition RemovePosition(uint pos)
        {
            if (PositionAddr == 0)
            {
                return null;
            }
           
            // check whether it exists
            var last = default(IndexStorageSegmentPosition);
            var position = default(IndexStorageSegmentPosition);
            var count = 0U;

            foreach (var i in Positions)
            {
                var compare = i.Position.CompareTo(pos);

                if (compare > 0)
                {
                    break;
                }
                else if (compare == 0)
                {
                    position = i;
                    break;
                }

                last = i;

                count++;
            }

            if (position != null && last == null)
            {
                // remove at the beginning
                PositionAddr = position.SuccessorAddr;

                Context.IndexFile.Write(this);
                Context.Allocator.Free(position);

                return position;
            }
            else if (position != null && last != null)
            {
                // remove in place
                last.SuccessorAddr = position.SuccessorAddr;

                Context.IndexFile.Write(this);
                Context.IndexFile.Write(last);
                Context.Allocator.Free(position);

                return position;
            }

            return null;
        }
        
        /// <summary>
        /// Reads the record from the storage medium.
        /// </summary>
        /// <param name="reader">The reader for i/o operations.</param>
        public override void Read(BinaryReader reader)
        {
            DocumentID = new Guid(reader.ReadBytes(16));
            SuccessorAddr = reader.ReadUInt64();
            PositionAddr = reader.ReadUInt64();
        }

        /// <summary>
        /// Writes the record to the storage medium.
        /// </summary>
        /// <param name="writer">The writer for i/o operations.</param>
        public override void Write(BinaryWriter writer)
        {
            writer.Write(DocumentID.ToByteArray());
            writer.Write(SuccessorAddr);
            writer.Write(PositionAddr);
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns
        ///  an integer that indicates whether the current instance precedes, follows, or
        ///  occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// A signed integer that indicates the relative values of x and y.
        ///     Less than zero -> x is less than y.
        ///     Zero -> x equals y.
        ///     Greater than zero -> x is greater than y.
        /// </returns>
        /// <exception cref="System.ArgumentException">Obj is not the same type as this instance.</exception>
        public int CompareTo(object obj)
        {
            if (obj is IndexStorageSegmentPosting posting)
            {
                return DocumentID.CompareTo(posting.DocumentID);
            }

            throw new System.ArgumentException();
        }
    }
}