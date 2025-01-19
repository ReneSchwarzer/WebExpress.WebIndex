using System;
using System.Collections.Generic;
using System.Linq;

namespace WebExpress.WebIndex.Memory
{
    /// <summary>
    /// Represents a tree which is formed from the characters of the numeric values.
    /// </summary>
    /// <remarks> 
    /// TODO: Implement balanced tree algorithm for optimal performance. 
    /// </remarks>
    public class IndexMemorySegmentNumericNode
    {
        /// <summary>
        /// The character of the node.
        /// </summary>
        public decimal Value { get; set; }

        /// <summary>
        /// Returns the left child of the node.
        /// </summary>
        public IndexMemorySegmentNumericNode Left { get; set; }

        /// <summary>
        /// Returns the right child of the node.
        /// </summary>
        public IndexMemorySegmentNumericNode Right { get; set; }

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
        /// Returns or sets the postings. This is always on the leaf of an term.
        /// </summary>
        public IEnumerable<IndexMemorySegmentPosting> Postings { get; private set; } = [];

        /// <summary>
        /// Passes through the tree in pre order.
        /// </summary>
        /// <returns>The tree as a list.</returns>
        public IEnumerable<IndexMemorySegmentNumericNode> PreOrder
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
        public IEnumerable<(decimal, IndexMemorySegmentNumericNode)> Values
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
            .SelectMany(x => x.Item2.Postings?.Select(x => x.DocumentId));

        /// <summary>
        /// Returns the value in the tree.
        /// </summary>
        /// <param name="subterm">A subterm that is shortened by the first character at each tree level.</param>
        /// <returns>The leaf of the term.</returns>
        public IndexMemorySegmentNumericNode this[decimal value]
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
        /// Initializes a new instance of the class.
        /// </summary>
        internal IndexMemorySegmentNumericNode()
        {
        }

        /// <summary>
        /// Create tree from term and save item in leaf.
        /// </summary>
        /// <param name="id">The data to be added to the index.</param>
        /// <param name="value">The value.</param>
        public void Add(Guid id, decimal value)
        {
            if (value.CompareTo(Value) < 0)
            {
                if (Left == null)
                {
                    Left = new IndexMemorySegmentNumericNode
                    {
                        Value = value,
                        Postings = [new IndexMemorySegmentPosting(id, 0)]
                    };
                }
                else
                {
                    Left.Add(id, value);
                }
            }
            else if (value.CompareTo(Value) > 0)
            {
                if (Right == null)
                {
                    Right = new IndexMemorySegmentNumericNode
                    {
                        Value = value,
                        Postings = [new IndexMemorySegmentPosting(id, 0)]
                    };
                }
                else { Right.Add(id, value); }
            }
            else
            {
                var posting = Postings.FirstOrDefault(x => x.DocumentId.Equals(id));

                if (posting == null)
                {
                    Postings = [new IndexMemorySegmentPosting(id, 0)];
                }
            }
        }

        /// <summary>
        /// Return all document ids for a given numeric value.
        /// </summary>
        /// <param name="search">The numeric value.</param>
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
                        foreach (var postting in Postings)
                        {
                            yield return postting.DocumentId;
                        }
                    }

                    if (Left?.Value >= search)
                    {
                        // recurse on the left subtree
                        foreach (var value in Left?.Retrieve(search, options) ?? [])
                        {
                            yield return value;
                        }
                    }

                    if (Right?.Value <= search)
                    {
                        // recurse on the right subtree
                        foreach (var value in Right?.Retrieve(search, options) ?? [])
                        {
                            yield return value;
                        }
                    }

                    break;
                case IndexRetrieveMethod.GratherThan:
                    if (Value > search)
                    {
                        foreach (var postting in Postings)
                        {
                            yield return postting.DocumentId;
                        }
                    }

                    if (Left?.Value >= search)
                    {
                        // recurse on the left subtree
                        foreach (var value in Left?.Retrieve(search, options) ?? [])
                        {
                            yield return value;
                        }
                    }

                    // recurse on the right subtree
                    foreach (var value in Right?.Retrieve(search, options) ?? [])
                    {
                        yield return value;
                    }

                    break;
                case IndexRetrieveMethod.GratherThanOrEqual:
                    // searches the binary tree for the largest value that is less or equals than the specified value
                    if (Value >= search)
                    {
                        foreach (var postting in Postings)
                        {
                            yield return postting.DocumentId;
                        }
                    }

                    if (Left?.Value >= search)
                    {
                        // recurse on the left subtree
                        foreach (var value in Left?.Retrieve(search, options) ?? [])
                        {
                            yield return value;
                        }
                    }

                    // recurse on the right subtree
                    foreach (var value in Right?.Retrieve(search, options) ?? [])
                    {
                        yield return value;
                    }

                    break;
                case IndexRetrieveMethod.LessThan:
                    // searches the binary tree for the smallest value that is greater than the specified value
                    if (Value < search)
                    {
                        foreach (var postting in Postings)
                        {
                            yield return postting.DocumentId;
                        }
                    }

                    // recurse on the left subtree
                    foreach (var value in Left?.Retrieve(search, options) ?? [])
                    {
                        yield return value;
                    }

                    if (Right?.Value <= search)
                    {
                        // recurse on the right subtree
                        foreach (var value in Right?.Retrieve(search, options) ?? [])
                        {
                            yield return value;
                        }
                    }

                    break;
                case IndexRetrieveMethod.LessThanOrEqual:
                    // searches the binary tree for the smallest value that is greater or equals than the specified value
                    if (Value <= search)
                    {
                        foreach (var postting in Postings)
                        {
                            yield return postting.DocumentId;
                        }
                    }

                    // recurse on the left subtree
                    foreach (var value in Left?.Retrieve(search, options) ?? [])
                    {
                        yield return value;
                    }

                    if (Right?.Value <= search)
                    {
                        // recurse on the right subtree
                        foreach (var value in Right?.Retrieve(search, options) ?? [])
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
        /// Converts the order expression to a string.
        /// </summary>
        /// <returns>The order expression as a string.</returns>
        public override string ToString()
        {
            return $"{Value} → {Postings?.ToString() ?? "null"}";
        }
    }
}
