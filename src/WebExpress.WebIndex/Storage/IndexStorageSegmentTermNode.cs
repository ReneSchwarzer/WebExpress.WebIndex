﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WebExpress.WebIndex.WebAttribute;

namespace WebExpress.WebIndex.Storage
{
    /// <summary>
    /// Each term is broken down into individual characters and stored as separate nodes in a search tree. With the exception 
    /// of the root node, the TreeNode segments, which have a constant length, are stored in the data area of the reverse 
    /// index. The path determines the term. Each node, which is a complete term, points to a term segment, which has additional 
    /// information about the term, such as its frequency, position in the document, and other relevant information that can be
    /// useful in search queries.
    /// </summary>
    /// <typeparam name="T">The data type. This must have the IIndexData interface.</typeparam>
    [SegmentCached]
    public class IndexStorageSegmentTermNode : IndexStorageSegment
    {
        /// <summary>
        /// Returns the amount of space required on the storage device.
        /// </summary>
        public static uint SegmentSize => sizeof(char) + sizeof(ulong) + sizeof(ulong) + sizeof(uint) + sizeof(ulong);

        /// <summary>
        /// Returns or sets the character of the node.
        /// </summary>
        public char Character { get; set; }

        /// <summary>
        /// Returns or sets the address of the siblings.
        /// </summary>
        public ulong SiblingAddr { get; set; }

        /// <summary>
        /// Returns the child address of the tree.
        /// </summary>
        public ulong ChildAddr { get; set; }

        /// <summary>
        /// Returns or sets the number of times the term is used (postings).
        /// </summary>
        public uint Fequency { get; set; }

        /// <summary>
        /// Returns the adress of the first posting element of a sorted list or 0 if there is no element.
        /// </summary>
        public ulong PostingAddr { get; private set; }

        /// <summary>
        /// Returns the sibling list.
        /// </summary>
        public IEnumerable<IndexStorageSegmentTermNode> Siblings
        {
            get
            {
                if (SiblingAddr == 0)
                {
                    yield break;
                }

                var addr = SiblingAddr;

                while (addr != 0)
                {
                    var item = Context.IndexFile.Read<IndexStorageSegmentTermNode>(addr, Context);
                    yield return item;

                    addr = item.SiblingAddr;
                }
            }
        }

        /// <summary>
        /// Returns the children list.
        /// </summary>
        public IEnumerable<IndexStorageSegmentTermNode> Children
        {
            get
            {
                if (ChildAddr == 0)
                {
                    yield break;
                }

                var addr = ChildAddr;

                while (addr != 0)
                {
                    var item = Context.IndexFile.Read<IndexStorageSegmentTermNode>(addr, Context);
                    yield return item;

                    addr = item.SiblingAddr;
                }
            }
        }

        /// <summary>
        /// Checks whether the current node is the root.
        /// </summary>
        public bool IsRoot => Character == 0;

        /// <summary>
        /// Passes through the tree in pre order.
        /// </summary>
        /// <returns>The tree as a list.</returns>
        public IEnumerable<IndexStorageSegmentTermNode> PreOrder
        {
            get
            {
                var list = new List<IndexStorageSegmentTermNode>
                {
                    this
                };

                foreach (var child in Children)
                {
                    list.AddRange(child.PreOrder);
                }

                return list;
            }
        }

        /// <summary>
        /// Returns all terms.
        /// </summary>
        public IEnumerable<(string, Guid[])> Terms
        {
            get
            {
                var list = new List<(string, Guid[])>();

                foreach (var child in Children)
                {
                    list.AddRange(child.Terms);
                }

                if (IsRoot)
                {
                    return list;
                }

                if (list.Count > 0)
                {
                    return list.Select(x => (Character + x.Item1, x.Item2));
                }

                return [(Character.ToString(), Postings?.Select(x => x.DocumentID).ToArray())];
            }
        }

        /// <summary>
        /// Returns all items.
        /// </summary>
        public IEnumerable<Guid> All => Terms
            .SelectMany(x => x.Item2);

        /// <summary>
        /// Returns the a sorted list of the postings or no element.
        /// </summary>
        public IEnumerable<IndexStorageSegmentPosting> Postings
        {
            get
            {
                if (PostingAddr == 0)
                {
                    yield break;
                }

                var addr = PostingAddr;

                while (addr != 0)
                {
                    var item = Context.IndexFile.Read<IndexStorageSegmentPosting>(addr, Context);
                    yield return item;

                    addr = item.SuccessorAddr;
                }
            }
        }

        /// <summary>
        /// Returns the term in the tree.
        /// </summary>
        /// <param name="subterm">A subterm that is shortened by the first character at each tree level.</param>
        /// <returns>The leaf of the term.</returns>
        public IndexStorageSegmentTermNode this[string subterm]
        {
            get
            {
                if (subterm == null)
                {
                    return this;
                }

                var first = subterm.FirstOrDefault();
                var next = subterm.Length > 1 ? subterm.Substring(1) : null;

                // find nodes
                foreach (var child in Children)
                {
                    if (first == child.Character)
                    {
                        // recursive descent
                        return child[next];
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">The reference to the context of the index.</param>
        /// <param name="addr">The adress of the segment.</param>
        public IndexStorageSegmentTermNode(IndexStorageContext context, ulong addr)
            : base(context, addr)
        {
        }

        /// <summary>
        /// Create tree from term and save item in leaf.
        /// </summary>
        /// <param name="subterm">A subterm that is shortened by the first character at each tree level.</param>
        public IndexStorageSegmentTermNode Add(string subterm)
        {
            if (subterm == null)
            {
                return this;
            }

            var first = subterm.FirstOrDefault();
            var next = subterm.Length > 1 ? subterm[1..] : null;

            // find existing nodes
            foreach (var child in Children)
            {
                if (first == child.Character)
                {
                    // recursive descent
                    return child.Add(next);
                }
            }

            // add new node
            var node = new IndexStorageSegmentTermNode(Context, Context.Allocator.Alloc(SegmentSize))
            {
                Character = first
            };

            AddChild(node);

            return node.Add(next);
        }

        /// <summary>
        /// Add a posting segments.
        /// </summary>
        /// <param name="id">The document id.</params>
        /// <returns>The posting segment.</returns>
        public IndexStorageSegmentPosting AddPosting(Guid id)
        {
            var item = default(IndexStorageSegmentPosting);

            if (PostingAddr == 0)
            {
                PostingAddr = Context.Allocator.Alloc(IndexStorageSegmentPosting.SegmentSize);
                item = new IndexStorageSegmentPosting(Context, PostingAddr)
                {
                    DocumentID = id
                };

                Context.IndexFile.Write(this);
                Context.IndexFile.Write(item);
            }
            else
            {
                // check whether it exists
                var last = default(IndexStorageSegmentPosting);
                var count = 0U;

                foreach (var i in Postings)
                {
                    var compare = i.DocumentID.CompareTo(id);

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

                item = new IndexStorageSegmentPosting(Context, Context.Allocator.Alloc(IndexStorageSegmentPosting.SegmentSize))
                {
                    DocumentID = id
                };

                if (last == null)
                {
                    // insert at the beginning
                    var tempAddr = PostingAddr;
                    PostingAddr = item.Addr;
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

                    Context.IndexFile.Write(last);
                    Context.IndexFile.Write(item);
                }
            }

            return item;
        }

        /// <summary>
        /// Remove a posting segments.
        /// </summary>
        /// <param name="id">The document id.</params>
        /// <returns>The posting segment.</returns>
        public IndexStorageSegmentPosting RemovePosting(Guid id)
        {
            if (PostingAddr == 0)
            {
                return null;
            }

            // check whether it exists
            var last = default(IndexStorageSegmentPosting);
            var posting = default(IndexStorageSegmentPosting);
            var count = 0U;

            foreach (var i in Postings)
            {
                var compare = i.DocumentID.CompareTo(id);

                if (compare > 0)
                {
                    break;
                }
                else if (compare == 0)
                {
                    posting = i;

                    break;
                }

                last = i;

                count++;
            }

            if (posting != null && last == null)
            {
                // remove at the beginning
                PostingAddr = posting.SuccessorAddr;

                Context.IndexFile.Write(this);
                Context.Allocator.Free(posting);

                return posting;
            }
            else if (posting != null && last != null)
            {
                // remove in place
                last.SuccessorAddr = posting.SuccessorAddr;

                Context.IndexFile.Write(this);
                Context.IndexFile.Write(last);
                Context.Allocator.Free(posting);

                return posting;
            }

            return null;
        }

        /// <summary>
        /// Add a child node.
        /// </summary>
        /// <returns>The child node.<returns>
        private IndexStorageSegmentTermNode AddChild(IndexStorageSegmentTermNode node)
        {
            if (ChildAddr == 0)
            {
                ChildAddr = node.Addr;

                Context.IndexFile.Write(this);
            }
            else
            {
                // check whether it exists
                var last = default(IndexStorageSegmentTermNode);
                var count = 0U;

                foreach (var i in Children)
                {
                    var compare = i.Character.CompareTo(node.Character);

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
                    var tempAddr = ChildAddr;
                    ChildAddr = node.Addr;
                    node.SiblingAddr = tempAddr;

                    Context.IndexFile.Write(this);
                    Context.IndexFile.Write(node);
                }
                else
                {
                    // insert in the correct place
                    var tempAddr = last.SiblingAddr;
                    last.SiblingAddr = node.Addr;
                    node.SiblingAddr = tempAddr;

                    Context.IndexFile.Write(this);
                    Context.IndexFile.Write(last);
                    Context.IndexFile.Write(node);
                }
            }

            return node;
        }

        /// <summary>
        /// Reads the record from the storage medium.
        /// </summary>
        /// <param name="reader">The reader for i/o operations.</param>
        public override void Read(BinaryReader reader)
        {
            Character = reader.ReadChar();
            SiblingAddr = reader.ReadUInt64();
            ChildAddr = reader.ReadUInt64();
            Fequency = reader.ReadUInt32();
            PostingAddr = reader.ReadUInt64();
        }

        /// <summary>
        /// Writes the record to the storage medium.
        /// </summary>
        /// <param name="writer">The writer for i/o operations.</param>
        public override void Write(BinaryWriter writer)
        {
            writer.Write(Character);
            writer.Write(SiblingAddr);
            writer.Write(ChildAddr);
            writer.Write(Fequency);
            writer.Write(PostingAddr);
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
            if (obj is IndexStorageSegmentTermNode item)
            {
                return Character == item.Character ? 0 : -1;
            }

            throw new ArgumentException();
        }

        /// <summary>
        /// Converts the order expression to a string.
        /// </summary>
        /// <returns>The order expression as a string.</returns>
        public override string ToString()
        {
            if (IsRoot)
            {
                return "ROOT";
            }

            return $"{Character}";
        }
    }
}