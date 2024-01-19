using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WebExpress.WebIndex.Storage
{
    /// <summary>
    /// The term tree.
    /// </summary>
    /// <typeparam name="T">The data type. This must have the IIndexData interface.</typeparam>
    public class IndexStorageSegmentTreeNode : IndexStorageSegment, IIndexStorageSegmentListItem
    {
        /// <summary>
        /// Returns or sets the address of the siblings.
        /// </summary>
        public ulong SuccessorAddr { get; set; }

        /// <summary>
        /// The character of the node.
        /// </summary>
        public char Character { get; set; }

        /// <summary>
        /// The child nodes of the tree.
        /// </summary>
        public IndexStorageSegmentList<IndexStorageSegmentTreeNode> Children { get; private set; }

        /// <summary>
        /// The item. This is always on the leaf of an term.
        /// </summary>
        public IndexStorageSegmentTerm Term { get; private set; }

        /// <summary>
        /// Checks whether the current node is the root.
        /// </summary>
        public bool IsRoot => Character == 0;

        /// <summary>
        /// Returns the amount of space required on the storage device.
        /// SuccessorAddr + Character + TermAddr + Children
        /// </summary>
        public override uint Size => sizeof(ulong) + sizeof(char) + sizeof(ulong) + sizeof(ulong);

        /// <summary>
        /// Passes through the tree in pre order.
        /// </summary>
        /// <returns>The tree as a list.</returns>
        public IEnumerable<IndexStorageSegmentTreeNode> PreOrder
        {
            get
            {
                var list = new List<IndexStorageSegmentTreeNode>
                {
                    this
                };

                foreach (var child in Children ?? Enumerable.Empty<IndexStorageSegmentTreeNode>())
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

                foreach (var child in Children ?? Enumerable.Empty<IndexStorageSegmentTreeNode>())
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

                return [(Character.ToString(), Term?.Postings?.All?.Select(x => x.DocumentID).ToArray())];
            }
        }

        /// <summary>
        /// Returns all items.
        /// </summary>
        public IEnumerable<Guid> All => Terms
            .SelectMany(x => x.Item2);

        /// <summary>
        /// Returns the term in the tree.
        /// </summary>
        /// <param name="subterm">A subterm that is shortened by the first character at each tree level.</param>
        /// <returns>The leaf of the term.</returns>
        public IndexStorageSegmentTreeNode this[string subterm]
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
        public IndexStorageSegmentTreeNode(IndexStorageContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Create tree from term and save item in leaf.
        /// </summary>
        /// <param name="subterm">A subterm that is shortened by the first character at each tree level.</param>
        public IndexStorageSegmentTreeNode Add(string subterm)
        {
            if (subterm == null)
            {
                if (Term == null)
                {
                    Term = new IndexStorageSegmentTerm(Context);
                    Context.Allocator.Alloc(Term);
                }

                Context.IndexFile.Write(this);

                return this;
            }

            var first = subterm.FirstOrDefault();
            var next = subterm.Length > 1 ? subterm[1..] : null;

            if (Children == null)
            {
                Children = new IndexStorageSegmentList<IndexStorageSegmentTreeNode>(Context, System.ComponentModel.ListSortDirection.Ascending);
                Context.Allocator.Alloc(Children);
                Context.IndexFile.Write(this);
            }

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
            var node = new IndexStorageSegmentTreeNode(Context)
            {
                Character = first
            };

            Children.Add(node);

            return node.Add(next);
        }

        /// <summary>
        /// Assigns an address to the segment.
        /// </summary>
        /// <param name="addr">The address of the segment.</param>
        public override void OnAllocated(ulong addr)
        {
            base.OnAllocated(addr);
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

            Character = reader.ReadChar();
            SuccessorAddr = reader.ReadUInt64();
            var childrenAddr = reader.ReadUInt64();
            var termAddr = reader.ReadUInt64();

            if (childrenAddr != 0)
            {
                Children = new IndexStorageSegmentList<IndexStorageSegmentTreeNode>(Context, System.ComponentModel.ListSortDirection.Ascending);
                Children.Read(reader, childrenAddr);
            }

            if (termAddr != 0)
            {
                Term = new IndexStorageSegmentTerm(Context);
                Term.Read(reader, termAddr);
            }
        }

        /// <summary>
        /// Writes the record to the storage medium.
        /// </summary>
        /// <param name="writer">The writer for i/o operations.</param>
        public override void Write(BinaryWriter writer)
        {
            writer.BaseStream.Seek((long)Addr, SeekOrigin.Begin);

            writer.Write(Character);
            writer.Write(SuccessorAddr);
            writer.Write(Children?.Addr ?? 0ul);
            writer.Write(Term?.Addr ?? 0ul);

            if (Children != null)
            {
                Children.Write(writer);
            }

            if (Term != null)
            {
                Term.Write(writer);
            }
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
            if (obj is IndexStorageSegmentTreeNode item)
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

            return $"{Character} → {Term?.ToString() ?? "null"}";
        }
    }
}