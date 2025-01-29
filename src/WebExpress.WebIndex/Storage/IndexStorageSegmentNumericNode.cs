﻿using System;
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
        public uint Height { get; private set; } = 1;

        /// <summary>
        /// Returns the balance factor of the tree.
        /// </summary>
        private int BalanceFactor => (int)(GetHeight(Left) - GetHeight(Right));

        /// <summary>
        /// Returns the height of the given node.
        /// </summary>
        /// <param name="node">The node whose height is to be determined.</param>
        /// <returns>The height of the node.</returns>
        private static uint GetHeight(IndexStorageSegmentNumericNode node)
        {
            return node?.Height ?? 0;
        }

        /// <summary>
        /// Updates the height of the current node based on the heights of its children.
        /// </summary>
        private void UpdateHeight()
        {
            Height = (uint)(Math.Max(GetHeight(Left), GetHeight(Right)) + 1);
        }

        /// <summary>
        /// Passes through the tree in post order.
        /// </summary>
        /// <returns>The tree as a list.</returns>
        public IEnumerable<IndexStorageSegmentNumericNode> PostOrder
        {
            get
            {
                yield return this;

                // recurse on the left subtree
                foreach (var n in Left?.PostOrder ?? [])
                {
                    yield return n;
                }

                // recurse on the right subtree
                foreach (var n in Right?.PostOrder ?? [])
                {
                    yield return n;
                }
            }
        }

        /// <summary>
        /// Returns all document ids.
        /// </summary>
        public IEnumerable<Guid> All => PostOrder
            .SelectMany(x => x.Posting?.All);

        /// <summary>
        /// Returns the root element of the posting tree.
        /// </summary>
        public IndexStorageSegmentNumericPostingNode Posting
        {
            get
            {
                if (PostingAddr != 0)
                {
                    var item = Context.IndexFile.Read<IndexStorageSegmentNumericPostingNode>(PostingAddr, Context);
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
        /// Adds a new node with the specified value and balances the tree.
        /// </summary>
        /// <param name="id">The unique identifier of the document.</param>
        /// <param name="value">The numeric value to be added to the tree.</param>
        /// <returns>The node where the value was added.</returns>
        public IndexStorageSegmentNumericNode AddAndBalance(Guid id, decimal value)
        {
            var node = Add(id, value);
            Balance();

            return node;
        }

        /// <summary>
        /// Add a posting segments.
        /// </summary>
        /// <param name="id">The document id.</params>
        /// <returns>The posting node segment.</returns>
        public IndexStorageSegmentNumericPostingNode AddPosting(Guid id)
        {
            var item = default(IndexStorageSegmentNumericPostingNode);

            lock (_guard)
            {
                if (PostingAddr == 0)
                {
                    PostingAddr = Context.Allocator.Alloc(IndexStorageSegmentNumericPostingNode.SegmentSize);
                    item = new IndexStorageSegmentNumericPostingNode(Context, PostingAddr)
                    {
                        DocumentID = id
                    };

                    Fequency++;

                    Context.IndexFile.Write(this);
                    Context.IndexFile.Write(item);
                }
                else
                {
                    if (Posting.Insert(id, out IndexStorageSegmentNumericPostingNode node))
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

                        Context.Allocator.Free(root);

                        Fequency--;

                        Context.IndexFile.Write(this);

                        return true;
                    }
                    else if (root.RightAddr == 0)
                    {
                        PostingAddr = root.LeftAddr;

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

                    if (Left != null && search < Value)
                    {
                        // recurse on the left subtree
                        foreach (var value in Left.Retrieve(search, options))
                        {
                            yield return value;
                        }
                    }

                    if (Right != null && search > Value)
                    {
                        // recurse on the right subtree
                        foreach (var value in Right.Retrieve(search, options))
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

                    if (Left != null)
                    {
                        foreach (var value in Left.Retrieve(search, options))
                        {
                            yield return value;
                        }
                    }

                    if (Right != null)
                    {
                        foreach (var value in Right.Retrieve(search, options))
                        {
                            yield return value;
                        }
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

                    if (Left != null)
                    {
                        foreach (var value in Left.Retrieve(search, options))
                        {
                            yield return value;
                        }
                    }

                    if (Right != null)
                    {
                        foreach (var value in Right.Retrieve(search, options))
                        {
                            yield return value;
                        }
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

                    if (Left != null)
                    {
                        foreach (var value in Left.Retrieve(search, options))
                        {
                            yield return value;
                        }
                    }

                    if (Right != null && search > Value)
                    {
                        foreach (var value in Right.Retrieve(search, options))
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

                    if (Left != null)
                    {
                        foreach (var value in Left.Retrieve(search, options))
                        {
                            yield return value;
                        }
                    }

                    if (Right != null && search >= Value)
                    {
                        foreach (var value in Right.Retrieve(search, options))
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
        internal virtual IEnumerable<IndexStorageSegmentNumericPostingNode> GetPostings(decimal value)
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
        /// Converts the order expression to a string.
        /// </summary>
        /// <returns>The order expression as a string.</returns>
        public override string ToString()
        {
            return $"{Value}";
        }

        /// <summary>
        /// Performs a right rotation on the current node.
        /// </summary>
        /// <returns>The new root after the rotation.</returns>
        private void RotateRight()
        {
            var value = Value;
            var rightAddr = RightAddr;
            var newRight = Left;
            var rightAddr1 = Left.RightAddr;
            var postingAddr = PostingAddr;

            Value = newRight.Value;
            PostingAddr = newRight.PostingAddr;
            LeftAddr = newRight.LeftAddr;
            RightAddr = newRight.Addr;
            newRight.Value = value;
            newRight.PostingAddr = postingAddr;
            newRight.LeftAddr = rightAddr1;
            newRight.RightAddr = rightAddr;

            Context.IndexFile.Write(this);
            Context.IndexFile.Write(newRight);
        }

        /// <summary>
        /// Performs a left rotation on the current node.
        /// </summary>
        private void RotateLeft()
        {
            var value = Value;
            var leftAddr = LeftAddr;
            var newLeft = Right;
            var leftAddr1 = newLeft.LeftAddr;
            var postingAddr = PostingAddr;

            Value = newLeft.Value;
            PostingAddr = newLeft.PostingAddr;
            LeftAddr = newLeft.Addr;
            RightAddr = newLeft.RightAddr;
            newLeft.Value = value;
            newLeft.PostingAddr = postingAddr;
            newLeft.LeftAddr = leftAddr;
            newLeft.RightAddr = leftAddr1;

            Context.IndexFile.Write(this);
            Context.IndexFile.Write(newLeft);
        }

        /// <summary>
        /// Balances the tree node by performing rotations if necessary.
        /// </summary>
        private void Balance()
        {
            UpdateHeight();

            if (BalanceFactor > 1)
            {
                if (Left.BalanceFactor < 0)
                {
                    Left.RotateLeft();
                }
                RotateRight();
            }
            else if (BalanceFactor < -1)
            {
                if (Right.BalanceFactor > 0)
                {
                    Right.RotateRight();
                }
                RotateLeft();
            }
        }

        /// <summary>
        /// Adds a new value to the binary tree. If the value is less than the current node's value, it is added to the left subtree.
        /// If the value is greater than the current node's value, it is added to the right subtree.
        /// </summary>
        /// <remarks>
        /// Works recursively and inserts the value into the tree.
        /// </remarks>
        /// <param name="id">The document id.</params>
        /// <param name="value">The value to be added to the tree.</param>
        /// <returns>The node where the value was added.</returns>
        private IndexStorageSegmentNumericNode Add(Guid id, decimal value)
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
                    item.AddPosting(id);

                    Context.IndexFile.Write(this);
                    Context.IndexFile.Write(item);

                    return item;
                }
                else
                {
                    return Left.AddAndBalance(id, value);
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

                    item.AddPosting(id);

                    Context.IndexFile.Write(this);
                    Context.IndexFile.Write(item);

                    return item;
                }
                else
                {
                    return Right.AddAndBalance(id, value);
                }
            }

            Value = value;

            AddPosting(id);

            Context.IndexFile.Write(this);

            return this;
        }
    }
}