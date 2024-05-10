﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace WebExpress.WebIndex.Memory
{
    /// <summary>
    /// Represents a tree which is formed from the characters of the terms.
    /// </summary>
    public class IndexMemoryReverseTerm<T> where T : IIndexItem
    {
        /// <summary>
        /// The character of the node.
        /// </summary>
        public char Character { get; set; }

        /// <summary>
        /// The child nodes of the tree.
        /// </summary>
        public IEnumerable<IndexMemoryReverseTerm<T>> Children { get; set; } = new List<IndexMemoryReverseTerm<T>>();

        /// <summary>
        /// The item. This is always on the leaf of an term.
        /// </summary>
        public IEnumerable<IndexMemoryReversePosting<T>> Posting { get; set; }

        /// <summary>
        /// Checks whether the current node is the root.
        /// </summary>
        public bool IsRoot => Character == 0;

        /// <summary>
        /// Passes through the tree in pre order.
        /// </summary>
        /// <returns>The tree as a list.</returns>
        public IEnumerable<IndexMemoryReverseTerm<T>> PreOrder
        {
            get
            {
                var list = new List<IndexMemoryReverseTerm<T>>
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
        public IEnumerable<string> Terms
        {
            get
            {
                var list = new List<string>();

                foreach (var child in Children)
                {
                    list.AddRange(child.Terms);
                }

                if (IsRoot)
                {
                    return list;
                }

                return list.Count > 0 ? list.Select(x => Character + x) : [Character.ToString()];
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        internal IndexMemoryReverseTerm()
        {
        }

        /// <summary>
        /// Create tree from term and save item in leaf.
        /// </summary>
        /// <param name="item">The data to be added to the index.</param>
        /// <param name="subterm">A subterm that is shortened by the first character at each tree level.</param>
        /// <param name="position">The position of the term in the input value.</param>
        public void Add(T item, string subterm, uint position)
        {
            if (subterm == null)
            {
                // end of recursive descent reached
                Posting ??= new List<IndexMemoryReversePosting<T>>();
                var posting = Posting.FirstOrDefault(x => x.Id.Equals(item.Id));

                if (posting == null)
                {
                    (Posting as List<IndexMemoryReversePosting<T>>).Add(new IndexMemoryReversePosting<T>(item, position));
                }
                else if (!posting.Contains<uint>(position)) 
                {
                    posting.Add(position);
                }

                return;
            }

            var children = Children as List<IndexMemoryReverseTerm<T>>;
            var first = subterm.FirstOrDefault();
            var next = subterm.Length > 1 ? subterm.Substring(1) : null;

            // find existing nodes
            foreach (var child in children)
            {
                if (first == child.Character)
                {
                    // recursive descent
                    child.Add(item, next, position);

                    return;
                }
            }

            // add new node
            var node = new IndexMemoryReverseTerm<T>() { Character = first };
            children.Add(node);
            node.Add(item, next, position);
        }

        /// <summary>
        /// The data to be removed from the field.
        /// </summary>
        /// <param name="subterm">A subterm that is shortened by the first character at each tree level.</param>
        /// <param name="item">The data to be added to the index.</param>
        public void Remove(string subterm, T item)
        {
            if (subterm == null)
            {
                Posting = Posting?.Where(X => !X.Id.Equals(item.Id));

                return;
            }

            var first = subterm.FirstOrDefault();
            var next = subterm.Length > 1 ? subterm.Substring(1) : null;

            // find nodes
            foreach (var child in Children)
            {
                if (first == child.Character)
                {
                    // recursive descent
                    child.Remove(next, item);
                }
            }
        }

        /// <summary>
        /// Return all term items for a given term.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <returns>An enumeration of the data ids.</returns>
        public IEnumerable<Guid> Collect(string term)
        {
            if (term == null)
            {
                return Posting.Select(x => x.Id);
            }

            var first = term.FirstOrDefault();
            var next = term.Length > 1 ? term.Substring(1) : null;

            // find nodes
            foreach (var child in Children)
            {
                if (first == child.Character)
                {
                    // recursive descent
                    return child.Collect(next);
                }
            }

            return Enumerable.Empty<Guid>();
        }

        /// <summary>
        /// Converts the order expression to a string.
        /// </summary>
        /// <returns>The order expression as a string.</returns>
        public override string ToString()
        {
            return $"{Character} → {Posting?.ToString() ?? "null"}";
        }
    }
}