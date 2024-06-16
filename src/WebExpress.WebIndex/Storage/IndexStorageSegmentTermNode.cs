using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
    /// <param name="context">The reference to the context of the index.</param>
    /// <param name="addr">The adress of the segment.</param>
    [SegmentCached]
    public class IndexStorageSegmentTermNode(IndexStorageContext context, ulong addr) : IndexStorageSegment(context, addr)
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
        /// Returns a guard to protect against concurrent access.
        /// </summary>
        private object Guard { get; } = new object();

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
                yield return this;

                foreach (var child in Children)
                {
                    foreach (var preOrderChild in child.PreOrder)
                    {
                        yield return preOrderChild;
                    }
                }
            }
        }

        /// <summary>
        /// Returns all terms.
        /// </summary>
        public IEnumerable<(string, IndexStorageSegmentTermNode)> Terms
        {
            get
            {
                foreach (var child in Children)
                {
                    foreach (var term in child.Terms)
                    {
                        if (Character != 0)
                        {
                            yield return (Character + term.Item1, term.Item2);
                        }
                        else
                        {
                            yield return (term.Item1, term.Item2);
                        }
                    }
                }

                if (IsRoot)
                {
                    yield break;
                }

                if (ChildAddr != 0)
                {
                    yield break;
                }

                yield return (Character.ToString(), this);
            }
        }

        /// <summary>
        /// Returns all items.
        /// </summary>
        public IEnumerable<Guid> All => Terms
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
                var next = subterm.Length > 1 ? subterm[1..] : null;

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
        /// <returns>The posting node segment.</returns>
        public IndexStorageSegmentPostingNode AddPosting(Guid id)
        {
            var item = default(IndexStorageSegmentPostingNode);

            lock (Guard)
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
        public IndexStorageSegmentPostingNode RemovePosting(Guid id)
        {
            if (PostingAddr == 0)
            {
                return null;
            }

            lock (Guard)
            {
                //if (PostingAddr != 0)
                //{
                //    var root = Posting;

                //    if (id.CompareTo(root.DocumentID) < 0)
                //    {
                //        if (root.Left.Remove(id) != null)
                //        {
                //            Fequency--;

                //            Context.IndexFile.Write(this);
                //        }
                //    }
                //    else if (id.CompareTo(root.DocumentID) > 0)
                //    {
                //        if (root.Right.Remove(id) != null)
                //        {
                //            Fequency--;

                //            Context.IndexFile.Write(this);
                //        }
                //    }
                //    else
                //    {
                //        // node with only one child or no child
                //        if (root.LeftAddr == 0)
                //        {
                //            PostingAddr = root.RightAddr;

                //            Context.Allocator.Free(root);

                //            Fequency--;

                //            Context.IndexFile.Write(this);

                //            return root;
                //        }
                //        else if (root.RightAddr == 0)
                //        {
                //            PostingAddr = root.LeftAddr;

                //            Context.Allocator.Free(root);

                //            Fequency--;

                //            Context.IndexFile.Write(this);

                //            return root;
                //        }

                //        // node with two children: Get the inorder successor (smallest in the right subtree)


                //        //root.data = MinValue(root.right);

                //        //// Delete the inorder successor
                //        //root.right = Remove(root.right, root.data);
                //    }
                //}
            }

            return null;
        }

        /// <summary>
        /// Add a child node.
        /// </summary>
        /// <returns>The child node.<returns>
        private IndexStorageSegmentTermNode AddChild(IndexStorageSegmentTermNode node)
        {
            lock (Guard)
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

                        Context.IndexFile.Write(last);
                        Context.IndexFile.Write(node);
                    }
                }

                return node;
            }
        }

        /// <summary>
        /// Return all term items for a given term.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <param name="options">The retrieve options.</param>
        /// <returns>An enumeration of data ids of the terms.</returns>
        public virtual IEnumerable<Guid> Retrieve(string term, IndexRetrieveOptions options)
        {
            foreach (var posting in GetPostings(term))
            {
                yield return posting.DocumentID;
            }
        }

        /// <summary>
        /// Return all term posting items for a given term.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <returns>An enumeration of posting items.</returns>
        internal virtual IEnumerable<IndexStorageSegmentPostingNode> GetPostings(string term)
        {
            foreach (var node in GetLeafs(term))
            {
                foreach (var posting in node.Posting?.PreOrder ?? Enumerable.Empty<IndexStorageSegmentPostingNode>())
                {
                    yield return posting;
                }
            }
        }

        /// <summary>
        /// Returns the nodes in the tree.
        /// </summary>
        /// <param name="term">A subterm that is shortened by the first character at each tree level.</param>
        /// <returns>An enumeration of leafs of the term.</returns>
        public virtual IEnumerable<IndexStorageSegmentTermNode> GetLeafs(string term)
        {
            if (term == null)
            {
                yield return this;
            }
            else
            {
                var first = term.FirstOrDefault();
                var next = term.Length > 1 ? term[1..] : null;

                switch (first)
                {
                    case '?':
                        // find nodes
                        foreach (var child in Children)
                        {
                            foreach (var node in child.GetLeafs(next))
                            {
                                yield return node;
                            }
                        }
                        break;
                    case '*':
                        var pattern = next?.Replace("*", ".*").Replace("?", ".") ?? ".*";
                        foreach (var termTuple in Terms)
                        {
                            if (Regex.IsMatch(termTuple.Item1, pattern))
                            {
                                yield return termTuple.Item2;
                            }
                        }
                        break;
                    default:
                        // find nodes
                        foreach (var child in Children)
                        {
                            if (first == child.Character)
                            {
                                // recursive descent
                                foreach (var node in child.GetLeafs(next))
                                {
                                    yield return node;
                                }
                            }
                        }
                        break;
                }
            }
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