using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using WebExpress.WebIndex.Term;

namespace WebExpress.WebIndex.Memory
{
    /// <summary>
    /// Provides a reverse index that manages the data in the main memory.
    /// Key: The terms.
    /// Value: The index item.
    /// </summary>
    /// <param name="context">The index context.</param>
    /// <param name="property">The property that makes up the index.</param>
    /// <param name="culture">The culture.</param>
    public class IndexMemoryReverse<T>(IIndexDocumemntContext context, PropertyInfo property, CultureInfo culture) : IIndexReverse<T> where T : IIndexItem
    {
        /// <summary>
        /// Returns the field name for the reverse index.
        /// </summary>
        public string Field => Property?.Name;

        /// <summary>
        /// A delegate that determines the value of the current property.
        /// </summary>
        private Func<T, object> GetValueDelegate { get; set; } = CreateDelegate(property);

        /// <summary>
        /// The property that makes up the index.
        /// </summary>
        private PropertyInfo Property { get; set; } = property;

        /// <summary>
        /// Returns the index context.
        /// </summary>
        public IIndexDocumemntContext Context { get; private set; } = context;

        /// <summary>
        /// Returns the culture.
        /// </summary>
        public CultureInfo Culture { get; private set; } = culture;

        /// <summary>
        /// The root term.
        /// </summary>
        public IndexMemoryReverseTerm Root { get; private set; } = new();

        /// <summary>
        /// Returns all items.
        /// </summary>
        public IEnumerable<Guid> All => Root.Terms
            .SelectMany(x => x.Item2.Postings)
            .Select(x => x.DocumentID)
            .Distinct();

        /// <summary>
        /// Adds a item to the index.
        /// </summary>
        /// <param name="item">The data to be added to the index.</param>
        public void Add(T item)
        {
            var value = GetValueDelegate(item);
            var terms = Context.TokenAnalyzer.Analyze(value?.ToString(), Culture);

            Add(item, terms);
        }

        /// <summary>
        /// Adds a item to the index.
        /// </summary>
        /// <param name="item">The data to be added to the index.</param>
        /// <param name="terms">The terms to add to the reverse index for the given item.</param>
        public void Add(T item, IEnumerable<IndexTermToken> terms)
        {
            foreach (var term in terms)
            {
                Root.Add(item.Id, term.Value.ToString(), term.Position);
            }
        }

        /// <summary>
        /// The data to be removed from the index.
        /// </summary>
        /// <param name="item">The data to be removed from the field.</param>
        public void Delete(T item)
        {
            var value = GetValueDelegate(item);
            var terms = Context.TokenAnalyzer.Analyze(value?.ToString(), Culture);

            Delete(item, terms);
        }

        /// <summary>
        /// The data to be removed from the index.
        /// </summary>
        /// <param name="item">The data to be removed from the field.</param>
        /// <param name="terms">The terms to add to the reverse index for the given item.</param>
        public void Delete(T item, IEnumerable<IndexTermToken> terms)
        {
            foreach (var term in terms)
            {
                Root.Remove(term.Value.ToString(), item.Id);
            }
        }

        /// <summary>
        /// Removed all data from the index.
        /// </summary>
        public void Clear()
        {
            Root = new IndexMemoryReverseTerm();
        }

        /// <summary>
        /// Drop the reverse index.
        /// </summary>
        public void Drop()
        {

        }

        /// <summary>
        /// Return all items for a given string.
        /// </summary>
        /// <param name="term">The term string.</param>
        /// <param name="options">The retrieve options.</param>
        /// <returns>An enumeration of the data ids.</returns>
        public IEnumerable<Guid> Retrieve(string term, IndexRetrieveOptions options)
        {
            var terms = Context.TokenAnalyzer.Analyze(term, Culture, true);
            var distinct = new HashSet<Guid>((int)Math.Min(options.MaxResults, int.MaxValue / 2));
            var count = 0u;

            if (!terms.Any())
            {
                return distinct;
            }

            switch (options.Method)
            {
                case IndexRetrieveMethod.Phrase:
                    {
                        var firstTerm = terms.Take(1).FirstOrDefault();
                        var nextTerms = terms.Skip(1);

                        foreach (var posting in Root.GetPostings(firstTerm.Value.ToString()))
                        {
                            foreach (var position in posting.Positions)
                            {
                                if (CheckForPhraseMatch(posting.DocumentID, position, firstTerm.Position, nextTerms))
                                {
                                    distinct.Add(posting.DocumentID);
                                }
                            }
                        }

                        break;
                    }
                default:
                    {
                        foreach (var document in terms.Take(1).SelectMany(x => Root.Retrieve(x.Value.ToString(), options)))
                        {
                            if (distinct.Add(document) && count++ >= options.MaxResults)
                            {
                                break;
                            }
                        }

                        foreach (var normalized in terms.Skip(1))
                        {
                            var temp = new HashSet<Guid>(distinct.Count);

                            foreach (var document in Root.Retrieve(normalized.Value.ToString(), options))
                            {
                                if (distinct.Contains(document) && temp.Add(document))
                                {
                                }
                            }

                            distinct = temp;
                        }

                        break;
                    }
            }

            return distinct;
        }

        /// <summary>
        /// Checks whether there is an exact match.
        /// </summary>
        /// <param name="document">The document id to check.</param>
        /// <param name="position">The position of the term within the document.</param>
        /// <param name="offset">The position within the search term.</param>
        /// <param name="terms">Further following search terms.</param>
        /// <returns>True ff there is an exact match, otherwise false.</returns>
        private bool CheckForPhraseMatch(Guid document, uint position, uint offset, IEnumerable<IndexTermToken> terms)
        {
            if (!terms.Any())
            {
                return true;
            }

            var firstTerm = terms.Take(1).FirstOrDefault();
            var nextTerms = terms.Skip(1);

            foreach (var posting in Root.GetPostings(firstTerm.Value.ToString()).Where(x => x?.DocumentID == document))
            {
                foreach (var pos in posting.Positions.Where(x => x == position + (firstTerm.Position - offset)))
                {
                    return CheckForPhraseMatch(posting.DocumentID, pos, firstTerm.Position, nextTerms);
                }
            }

            return false;
        }

        /// <summary>
        /// Creates a delegate for faster access to the value of the property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>A delegate that determines the value of the current property.</returns>
        private static Func<T, object> CreateDelegate(PropertyInfo property)
        {
            var helper = typeof(IndexMemoryReverse<T>).GetMethod("CreateDelegateInternal", BindingFlags.Static | BindingFlags.NonPublic);
            var method = helper.MakeGenericMethod(typeof(T), property.PropertyType);

            return (Func<T, object>)method.Invoke(null, [property.GetGetMethod()]);
        }

        /// <summary>
        /// An auxiliary function used to determine the value of a property.
        /// </summary>
        /// <param name="methodInfo">The method Info.</param>
        /// <returns>A delegate that determines the value of the current property.</returns>
        [SuppressMessage("CodeQuality", "IDE0051")]
        private static Func<X, object> CreateDelegateInternal<X, TReturn>(MethodInfo methodInfo)
        {
            var f = (Func<X, TReturn>)System.Delegate.CreateDelegate(typeof(Func<X, TReturn>), methodInfo);
            return t => (object)f(t);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, 
        /// or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
        }
    }
}
