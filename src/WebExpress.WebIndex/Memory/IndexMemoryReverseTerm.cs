using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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
        /// Returns or sets the child nodes of the tree.
        /// </summary>
        public List<IndexMemoryReverseTerm<T>> Children { get; set; } = [];

        /// <summary>
        /// Returns or sets the postings. This is always on the leaf of an term.
        /// </summary>
        public List<IndexMemoryReversePosting<T>> Postings { get; set; }

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
        public IEnumerable<(string, Guid[])> Terms
        {
            get
            {
                foreach (var child in Children)
                {
                    foreach (var term in child.Terms)
                    {
                        yield return (Character + term.Item1, term.Item2);
                    }
                }

                if (IsRoot)
                {
                    yield break;
                }

                if (Children.Any())
                {
                    yield break;
                }

                yield return (Character.ToString(), Postings?.Select(x => x.DocumentID).ToArray());
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
                Postings ??= [];
                var posting = Postings.FirstOrDefault(x => x.DocumentID.Equals(item.Id));

                if (posting == null)
                {
                    Postings.Add(new IndexMemoryReversePosting<T>(item, position));
                }
                else if (!posting.Contains<uint>(position))
                {
                    posting.Add(position);
                }

                return;
            }

            var children = Children as List<IndexMemoryReverseTerm<T>>;
            var first = subterm.FirstOrDefault();
            var next = subterm.Length > 1 ? subterm[1..] : null;

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
                Postings = Postings?.Where(X => !X.DocumentID.Equals(item.Id)).ToList();

                return;
            }

            var first = subterm.FirstOrDefault();
            var next = subterm.Length > 1 ? subterm[1..] : null;

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
        /// <param name="options">The retrieve options.</param>
        /// <returns>An enumeration of tuples with data ids and the terms.</returns>
        public virtual IEnumerable<Guid> Retrieve(string term, IndexRetrieveOptions options)
        {
            if (term == null)
            {
                foreach (var posting in Postings)
                {
                    yield return posting.DocumentID;
                }
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
                            foreach (var id in child.Retrieve(next, options))
                            {
                                yield return id;
                            }
                        }
                        break;
                    case '*':
                        var pattern = next?.Replace("*", ".*").Replace("?", ".") ?? ".*";
                        foreach (var termTuple in Terms)
                        {
                            if (Regex.IsMatch(termTuple.Item1, pattern))
                            {
                                foreach (var id in termTuple.Item2)
                                {
                                    yield return id;
                                }
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
                                foreach (var id in child.Retrieve(next, options))
                                {
                                    yield return id;
                                }
                            }
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Converts the order expression to a string.
        /// </summary>
        /// <returns>The order expression as a string.</returns>
        public override string ToString()
        {
            return $"{Character} → {Postings?.ToString() ?? "null"}";
        }
    }
}
