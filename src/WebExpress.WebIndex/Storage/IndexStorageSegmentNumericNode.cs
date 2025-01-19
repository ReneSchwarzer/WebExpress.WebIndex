using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using WebExpress.WebIndex.WebAttribute;

namespace WebExpress.WebIndex.Storage
{
    /// <summary>
    /// Each node is a numeric value stored in a binary tree. Each node has additional 
    /// information about the value, such as its frequency, position in the document, and other relevant information that can be
    /// useful in search queries.
    /// </summary>
    /// <remarks> 
    /// TODO: Implement balanced tree algorithm for optimal performance. 
    /// </remarks>
    /// <param name="context">The reference to the context of the index.</param>
    /// <param name="addr">The adress of the segment.</param>
    [SegmentCached]
    public class IndexStorageSegmentNumericNode(IndexStorageContext context, ulong addr) : IndexStorageSegment(context, addr)
    {
        private readonly Lock _guard = new();

        /// <summary>
        /// Returns the amount of space required on the storage device.
        /// </summary>
        public const uint SegmentSize = sizeof(decimal) + sizeof(ulong) + sizeof(ulong) + sizeof(uint) + sizeof(ulong);

        /// <summary>
        /// Returns or sets the value of the node.
        /// </summary>
        public decimal Value { get; set; }

        /// <summary>
        /// Returns or sets the address of the left child.
        /// </summary>
        public ulong LeftAddr { get; set; }

        /// <summary>
        /// Returns or sets the address of the right child.
        /// </summary>
        public ulong RightAddr { get; set; }

        /// <summary>
        /// Returns or sets the number of times the term is used (postings).
        /// </summary>
        public uint Fequency { get; set; }

        /// <summary>
        /// Returns the adress of the first posting element of a sorted list or 0 if there is no element.
        /// </summary>
        public ulong PostingAddr { get; private set; }

        /// <summary>
        /// Returns the left child of the node.
        /// </summary>
        public IndexStorageSegmentNumericNode Left
        {
            get
            {
                if (LeftAddr != 0)
                {
                    var item = Context.IndexFile.Read<IndexStorageSegmentNumericNode>(LeftAddr, Context);
                    return item;
                }

                return null;
            }
        }

        /// <summary>
        /// Returns the right child of the node.
        /// </summary>
        public IndexStorageSegmentNumericNode Right
        {
            get
            {
                if (RightAddr != 0)
                {
                    var item = Context.IndexFile.Read<IndexStorageSegmentNumericNode>(RightAddr, Context);
                    return item;
                }

                return null;
            }
        }

        /// <summary>
        /// Returns the height of the tree.
        /// </summary>
        public uint Height
        {
            get
            {
                var leftHeight = Left?.Height ?? 0;
                var rightHeight = Right?.Height ?? 0;

                return Math.Max(leftHeight, rightHeight) + 1;
            }
        }

        /// <summary>
        /// Returns the balance factor of the tree.
        /// </summary>
        public uint Balance
        {
            get
            {
                var leftHeight = Left?.Height ?? 0;
                var rightHeight = Right?.Height ?? 0;

                return leftHeight > rightHeight ? leftHeight - rightHeight : rightHeight - leftHeight;
            }
        }

        /// <summary>
        /// Passes through the tree in pre order.
        /// </summary>
        /// <returns>The tree as a list.</returns>
        public IEnumerable<IndexStorageSegmentNumericNode> PreOrder
        {
            get
            {
                yield return this;

                // recurse on the left subtree
                foreach (var n in Left?.PreOrder ?? [])
                {
                    yield return n;
                }

                // recurse on the right subtree
                foreach (var n in Right?.PreOrder ?? [])
                {
                    yield return n;
                }
            }
        }

        /// <summary>
        /// Returns all values.
        /// </summary>
        public IEnumerable<(decimal, IndexStorageSegmentNumericNode)> Values
        {
            get
            {
                foreach (var node in PreOrder)
                {
                    yield return (node.Value, node);
                }
            }
        }

        /// <summary>
        /// Returns all document ids.
        /// </summary>
        public IEnumerable<Guid> All => Values
            .SelectMany(x => x.Item2.Posting?.All);

        /// <summary>
        /// Returns the root element of the posting tree.
        /// </summary>
        public IndexStorageSegmentPostingNode Posting
        {
            get
            {
                if (PostingAddr != 0)
                {
                    var item = Context.IndexFile.Read<IndexStorageSegmentPostingNode>(PostingAddr, Context);
                    return item;
                }

                return null;
            }
        }

        /// <summary>
        /// Returns the value in the tree.
        /// </summary>
        /// <param name="subterm">A subterm that is shortened by the first character at each tree level.</param>
        /// <returns>The leaf of the term.</returns>
        public IndexStorageSegmentNumericNode this[decimal value]
        {
            get
            {
                if (value.CompareTo(Value) < 0)
                {
                    return Left?[value];
                }
                else if (value.CompareTo(Value) > 0)
                {
                    return Right?[value];
                }

                return this;
            }
        }

        /// <summary>
        /// Adds a new value to the binary tree. If the value is less than the current node's value, it is added to the left subtree.
        /// If the value is greater than the current node's value, it is added to the right subtree.
        /// </summary>
        /// <remarks>
        /// Works recursively and inserts the value into the tree.
        /// </remarks>
        /// <param name="value">The value to be added to the tree.</param>
        /// <returns>The node where the value was added.</returns>
        public IndexStorageSegmentNumericNode Add(decimal value)
        {
            if (value.CompareTo(Value) < 0)
            {
                if (LeftAddr == 0)
                {
                    LeftAddr = Context.Allocator.Alloc(SegmentSize);
                    var item = new IndexStorageSegmentNumericNode(Context, LeftAddr)
                    {
                        Value = value
                    };

                    Context.IndexFile.Write(this);

                    return item;
                }
                else
                {
                    return Left.Add(value);
                }
            }
            else if (value.CompareTo(Value) > 0)
            {
                if (RightAddr == 0)
                {
                    RightAddr = Context.Allocator.Alloc(SegmentSize);
                    var item = new IndexStorageSegmentNumericNode(Context, RightAddr)
                    {
                        Value = value
                    };

                    Context.IndexFile.Write(this);

                    return item;
                }
                else
                {
                    return Right.Add(value);
                }
            }

            Value = value;

            return this;
        }

        /// <summary>
        /// Add a posting segments.
        /// </summary>
        /// <param name="id">The document id.</params>
        /// <returns>The posting node segment.</returns>
        public IndexStorageSegmentPostingNode AddPosting(Guid id)
        {
            var item = default(IndexStorageSegmentPostingNode);

            lock (_guard)
            {
                if (PostingAddr == 0)
                {
                    PostingAddr = Context.Allocator.Alloc(IndexStorageSegmentPostingNode.SegmentSize);
                    item = new IndexStorageSegmentPostingNode(Context, PostingAddr)
                    {
                        DocumentID = id
                    };

                    Fequency++;

                    Context.IndexFile.Write(this);
                    Context.IndexFile.Write(item);
                }
                else
                {
                    if (Posting.Insert(id, out IndexStorageSegmentPostingNode node))
                    {
                        Fequency++;

                        Context.IndexFile.Write(this);
                    }

                    item = node;
                }
            }

            return item;
        }

        /// <summary>
        /// Remove a posting segments.
        /// </summary>
        /// <param name="id">The document id.</params>
        /// <returns>The posting segment.</returns>
        public bool RemovePosting(Guid id)
        {
            if (PostingAddr == 0)
            {
                return false;
            }

            lock (_guard)
            {
                if (PostingAddr != 0)
                {
                    var root = Posting;

                    if (id.CompareTo(root.DocumentID) < 0)
                    {
                        if (root.Left?.Remove(id, root, IndexStorageBinaryTreeDirection.Left) ?? false)
                        {
                            Fequency--;

                            Context.IndexFile.Write(this);

                            return true;
                        }

                        return false;
                    }
                    else if (id.CompareTo(root.DocumentID) > 0)
                    {
                        if (root.Right?.Remove(id, root, IndexStorageBinaryTreeDirection.Right) ?? false)
                        {
                            Fequency--;

                            Context.IndexFile.Write(this);

                            return true;
                        }

                        return false;
                    }

                    // node with only one child or no child
                    if (root.LeftAddr == 0)
                    {
                        PostingAddr = root.RightAddr;

                        root.RemovePositions();
                        Context.Allocator.Free(root);

                        Fequency--;

                        Context.IndexFile.Write(this);

                        return true;
                    }
                    else if (root.RightAddr == 0)
                    {
                        PostingAddr = root.LeftAddr;

                        root.RemovePositions();
                        Context.Allocator.Free(root);

                        Fequency--;

                        Context.IndexFile.Write(this);

                        return true;
                    }

                    // node with two children: get the inorder successor (most left child in the right subtree)
                    var leftmostChild = root.Right.LeftmostChild;
                    var inorderSuccessor = leftmostChild?.Leftmost;
                    var inorderSuccessorParent = leftmostChild?.Parent;

                    inorderSuccessor.LeftAddr = root.LeftAddr;
                    inorderSuccessor.RightAddr = inorderSuccessorParent?.Addr ?? 0ul;
                    Context.IndexFile.Write(inorderSuccessor);

                    if (inorderSuccessorParent != null)
                    {
                        inorderSuccessorParent.LeftAddr = 0ul;
                        Context.IndexFile.Write(inorderSuccessorParent);
                    }

                    PostingAddr = inorderSuccessor?.Addr ?? 0ul;
                    root.RemovePositions();
                    Context.Allocator.Free(root);

                    Fequency--;

                    Context.IndexFile.Write(this);

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Return all document ids for a given numeric value.
        /// </summary>
        /// <param name="search">The value.</param>
        /// <param name="options">The retrieve options.</param>
        /// <returns>An enumeration of data ids of the terms.</returns>
        public virtual IEnumerable<Guid> Retrieve(decimal search, IndexRetrieveOptions options)
        {
            var left = Left;
            var right = Right;

            switch (options.Method)
            {
                case IndexRetrieveMethod.Phrase:
                    // searches the binary tree for the value that is equals with the specified value
                    if (Value == search)
                    {
                        foreach (var documentId in Posting?.All ?? [])
                        {
                            yield return documentId;
                        }
                    }

                    if (left?.Value >= search)
                    {
                        // recurse on the left subtree
                        foreach (var value in left?.Retrieve(search, options) ?? [])
                        {
                            yield return value;
                        }
                    }

                    if (right?.Value <= search)
                    {
                        // recurse on the right subtree
                        foreach (var value in right?.Retrieve(search, options) ?? [])
                        {
                            yield return value;
                        }
                    }

                    break;
                case IndexRetrieveMethod.GratherThan:
                    if (Value > search)
                    {
                        foreach (var documentId in Posting?.All ?? [])
                        {
                            yield return documentId;
                        }
                    }

                    if (left?.Value >= search)
                    {
                        // recurse on the left subtree
                        foreach (var value in left?.Retrieve(search, options) ?? [])
                        {
                            yield return value;
                        }
                    }

                    // recurse on the right subtree
                    foreach (var value in right?.Retrieve(search, options) ?? [])
                    {
                        yield return value;
                    }

                    break;
                case IndexRetrieveMethod.GratherThanOrEqual:
                    // searches the binary tree for the largest value that is less or equals than the specified value
                    if (Value >= search)
                    {
                        foreach (var documentId in Posting?.All ?? [])
                        {
                            yield return documentId;
                        }
                    }

                    if (left?.Value >= search)
                    {
                        // recurse on the left subtree
                        foreach (var value in left?.Retrieve(search, options) ?? [])
                        {
                            yield return value;
                        }
                    }

                    // recurse on the right subtree
                    foreach (var value in right?.Retrieve(search, options) ?? [])
                    {
                        yield return value;
                    }

                    break;
                case IndexRetrieveMethod.LessThan:
                    // searches the binary tree for the smallest value that is greater than the specified value
                    if (Value < search)
                    {
                        foreach (var documentId in Posting?.All ?? [])
                        {
                            yield return documentId;
                        }
                    }

                    // recurse on the left subtree
                    foreach (var value in left?.Retrieve(search, options) ?? [])
                    {
                        yield return value;
                    }

                    if (right?.Value <= search)
                    {
                        // recurse on the right subtree
                        foreach (var value in right?.Retrieve(search, options) ?? [])
                        {
                            yield return value;
                        }
                    }

                    break;
                case IndexRetrieveMethod.LessThanOrEqual:
                    // searches the binary tree for the smallest value that is greater or equals than the specified value
                    if (Value <= search)
                    {
                        foreach (var documentId in Posting?.All ?? [])
                        {
                            yield return documentId;
                        }
                    }

                    // recurse on the left subtree
                    foreach (var value in left?.Retrieve(search, options) ?? [])
                    {
                        yield return value;
                    }

                    if (right?.Value <= search)
                    {
                        // recurse on the right subtree
                        foreach (var value in right?.Retrieve(search, options) ?? [])
                        {
                            yield return value;
                        }
                    }

                    break;
                default:
                    yield break;
            }
        }

        /// <summary>
        /// Return all term posting items for a given value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>An enumeration of posting items.</returns>
        internal virtual IEnumerable<IndexStorageSegmentPostingNode> GetPostings(decimal value)
        {
            var node = this[value];

            foreach (var posting in node.Posting?.PreOrder ?? [])
            {
                yield return posting;
            }
        }

        /// <summary>
        /// Reads the record from the storage medium.
        /// </summary>
        /// <param name="reader">The reader for i/o operations.</param>
        public override void Read(BinaryReader reader)
        {
            Value = reader.ReadDecimal();
            LeftAddr = reader.ReadUInt64();
            RightAddr = reader.ReadUInt64();
            Fequency = reader.ReadUInt32();
            PostingAddr = reader.ReadUInt64();
        }

        /// <summary>
        /// Writes the record to the storage medium.
        /// </summary>
        /// <param name="writer">The writer for i/o operations.</param>
        public override void Write(BinaryWriter writer)
        {
            writer.Write(Value);
            writer.Write(LeftAddr);
            writer.Write(RightAddr);
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
                return Value == item.Character ? 0 : -1;
            }

            throw new ArgumentException("Object is not the same type as this instance.", nameof(obj));
        }

        /// <summary>
        /// Converts the order expression to a string.
        /// </summary>
        /// <returns>The order expression as a string.</returns>
        public override string ToString()
        {
            return $"{Value}";
        }
    }
}