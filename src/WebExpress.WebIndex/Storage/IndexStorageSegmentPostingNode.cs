using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WebExpress.WebIndex.WebAttribute;

namespace WebExpress.WebIndex.Storage
{
    /// <summary>
    /// Each document stored as separate nodes in a binary search tree.
    /// </summary>
    /// <typeparam name="T">The data type. This must have the IIndexData interface.</typeparam>
    /// <param name="context">The reference to the context of the index.</param>
    /// <param name="addr">The adress of the segment.</param>
    [SegmentCached]
    public class IndexStorageSegmentPostingNode(IndexStorageContext context, ulong addr) : IndexStorageSegment(context, addr)
    {
        /// <summary>
        /// Returns the amount of space required on the storage device.
        /// </summary>
        public static uint SegmentSize => 16 + sizeof(uint) + sizeof(ulong) + sizeof(ulong);

        /// <summary>
        /// Returns or sets the document id.
        /// </summary>
        public Guid DocumentID { get; set; }

        /// <summary>
        /// Returns or sets the address of the left child.
        /// </summary>
        public ulong LeftAddr { get; set; }

        /// <summary>
        /// Returns or sets the address of the right child.
        /// </summary>
        public ulong RightAddr { get; set; }

        /// <summary>
        /// Returns the adress of the first position element of a sorted list or 0 if there is no element.
        /// </summary>
        public ulong PositionAddr { get; private set; }

        /// <summary>
        /// Returns a guard to protect against concurrent access.
        /// </summary>
        private object Guard { get; } = new object();

        /// <summary>
        /// Returns the left child of the node.
        /// </summary>
        public IndexStorageSegmentPostingNode Left
        {
            get
            {
                if (LeftAddr != 0)
                {
                    var item = Context.IndexFile.Read<IndexStorageSegmentPostingNode>(LeftAddr, Context);
                    return item;
                }

                return null;
            }
        }

        /// <summary>
        /// Returns the right child of the node.
        /// </summary>
        public IndexStorageSegmentPostingNode Right
        {
            get
            {
                if (RightAddr != 0)
                {
                    var item = Context.IndexFile.Read<IndexStorageSegmentPostingNode>(RightAddr, Context);
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
        /// Returns the node with the minimum id.
        /// </summary>
        public IndexStorageSegmentPostingNode MinNode
        {
            get
            {
                var node = Right;

                while (node?.Left != null)
                {
                    node = node.Left;
                }

                return node;
            }
        }

        /// <summary>
        /// Passes through the tree in pre order.
        /// </summary>
        /// <returns>The tree as a list.</returns>
        public IEnumerable<IndexStorageSegmentPostingNode> PreOrder
        {
            get
            {
                yield return this;

                // recurse on the left subtree
                foreach (var n in Left?.PreOrder ?? Enumerable.Empty<IndexStorageSegmentPostingNode>())
                {
                    yield return n;
                }

                // recurse on the right subtree
                foreach (var n in Right?.PreOrder ?? Enumerable.Empty<IndexStorageSegmentPostingNode>())
                {
                    yield return n;
                }
            }
        }

        /// <summary>
        /// Returns all documents.
        /// </summary>
        public IEnumerable<(Guid, IEnumerable<IndexStorageSegmentPosition>)> Documents
        {
            get
            {
                foreach (var node in PreOrder)
                {
                    yield return (node.DocumentID, node.Positions);
                }
            }
        }

        /// <summary>
        /// Returns all document ids.
        /// </summary>
        public IEnumerable<Guid> All => PreOrder
            .Select(x => x.DocumentID);

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
        /// Inserts a new node with the given document id into the binary tree.
        /// </summary>
        /// <param name="id">The document id.</params>
        /// <param name="insert">The posting node segment.</param>
        /// <returns>Ture if a new node has been inserted, otherwise false.</returns>
        public bool Insert(Guid id, out IndexStorageSegmentPostingNode insert)
        {
            if (id.CompareTo(DocumentID) < 0)
            {
                if (LeftAddr == 0)
                {
                    LeftAddr = Context.Allocator.Alloc(SegmentSize);
                    var item = new IndexStorageSegmentPostingNode(Context, LeftAddr)
                    {
                        DocumentID = id
                    };

                    Context.IndexFile.Write(this);

                    insert = item;

                    return true;
                }
                else
                {
                    return Left.Insert(id, out insert);
                }
            }
            else if (id.CompareTo(DocumentID) > 0)
            {
                if (RightAddr == 0)
                {
                    RightAddr = Context.Allocator.Alloc(SegmentSize);
                    var item = new IndexStorageSegmentPostingNode(Context, RightAddr)
                    {
                        DocumentID = id
                    };

                    Context.IndexFile.Write(this);

                    insert = item;

                    return true;
                }
                else
                {
                    return Right.Insert(id, out insert);
                }
            }

            insert = this;

            return false;
        }

        /// <summary>
        /// Removes a node with the given data from the binary tree.
        /// </summary>
        /// <param name="id">The document id.</params>
        /// <returns>Ture if a node has been removed, otherwise false.</returns>
        public bool Remove(Guid id)
        {
            // Otherwise, recur down the tree
            if (id.CompareTo(DocumentID) < 0)
            {
                return Left.Remove(id);
            }
            else if (id.CompareTo(DocumentID) > 0)
            {
                return Right.Remove(id);
            }


            //// node with only one child or no child
            //if (LeftAddr == 0)
            //{
            //    LeftAddr = RightAddr;
            //}
            //else if (RightAddr == 0)
            //{
            //    RightAddr = LeftAddr;
            //}

            // node with two children: Get the inorder successor (smallest in the right subtree)


            //root.data = MinValue(root.right);

            //// Delete the inorder successor
            //root.right = Remove(root.right, root.data);

            return true;
        }

        /// <summary>
        /// Add a position segments.
        /// </summary>
        /// <param name="pos">The position of the term.</params>
        /// <returns>The position segment.</returns>
        public IndexStorageSegmentPosition AddPosition(uint pos)
        {
            var item = default(IndexStorageSegmentPosition);

            lock (Guard)
            {
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

                        Context.IndexFile.Write(last);
                        Context.IndexFile.Write(item);
                    }
                }
            }

            return item;
        }

        /// <summary>
        /// Remove all position segments.
        /// </summary>
        public void RemovePositions()
        {
            if (PositionAddr == 0)
            {
                return;
            }

            lock (Guard)
            {
                foreach (var position in Positions)
                {
                    // remove
                    Context.Allocator.Free(position);
                }
            }
        }

        /// <summary>
        /// Reads the record from the storage medium.
        /// </summary>
        /// <param name="reader">The reader for i/o operations.</param>
        public override void Read(BinaryReader reader)
        {
            DocumentID = new Guid(reader.ReadBytes(16));
            LeftAddr = reader.ReadUInt64();
            RightAddr = reader.ReadUInt64();
            PositionAddr = reader.ReadUInt64();
        }

        /// <summary>
        /// Writes the record to the storage medium.
        /// </summary>
        /// <param name="writer">The writer for i/o operations.</param>
        public override void Write(BinaryWriter writer)
        {
            writer.Write(DocumentID.ToByteArray());
            writer.Write(LeftAddr);
            writer.Write(RightAddr);
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
            if (obj is IndexStorageSegmentPostingNode posting)
            {
                return DocumentID.CompareTo(posting.DocumentID);
            }

            throw new ArgumentException();
        }

        /// <summary>
        /// Converts the order expression to a string.
        /// </summary>
        /// <returns>The order expression as a string.</returns>
        public override string ToString()
        {
            return $"{DocumentID}";
        }
    }
}