using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using WebExpress.WebIndex.Term;

namespace WebExpress.WebIndex.Storage
{
    /// <summary>
    /// Implementation of the web reverse index, which stores the key-value pairs on disk.
    /// </summary>
    /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
    public class IndexStorageReverse<T> : IIndexReverse<T>, IIndexStorage where T : IIndexItem
    {
        /// <summary>
        /// The property that makes up the index.
        /// </summary>
        private PropertyInfo Property { get; set; }

        /// <summary>
        /// Returns the file name for the reverse index.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Returns or sets the reverse index file.
        /// </summary>
        public IndexStorageFile IndexFile { get; private set; }

        /// <summary>
        /// Returns or sets the header.
        /// </summary>
        public IndexStorageSegmentHeader Header { get; private set; }

        /// <summary>
        /// Returns or sets the term tree.
        /// </summary>
        public IndexStorageSegmentTerm Term { get; private set; }

        /// <summary>
        /// Returns or sets the memory manager.
        /// </summary>
        public IndexStorageSegmentAllocator Allocator { get; private set; }

        /// <summary>
        /// Returns the statistical values that can be help to optimize the index.
        /// </summary>
        public IndexStorageSegmentStatistic Statistic { get; private set; }

        /// <summary>
        /// Returns the index context.
        /// </summary>
        public IIndexDocumemntContext Context { get; private set; }

        /// <summary>
        /// Returns the culture.
        /// </summary>
        public CultureInfo Culture { get; private set; }

        /// <summary>
        /// Returns all items.
        /// </summary>
        public IEnumerable<Guid> All => Term.All.Distinct();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">The index context.</param>
        /// <param name="property">The property that makes up the index.</param>
        /// <param name="culture">The culture.</param>
        public IndexStorageReverse(IIndexDocumemntContext context, PropertyInfo property, CultureInfo culture)
        {
            Context = context;
            Property = property;
            Culture = culture;
            FileName = Path.Combine(Context.IndexDirectory, $"{typeof(T).Name}.{property.Name}.wri");

            var exists = File.Exists(FileName);

            IndexFile = new IndexStorageFile(FileName);
            Header = new IndexStorageSegmentHeader(new IndexStorageContext(this)) { Identifier = "wri" };
            Allocator = new IndexStorageSegmentAllocator(new IndexStorageContext(this));
            Statistic = new IndexStorageSegmentStatistic(new IndexStorageContext(this));
            Term = new IndexStorageSegmentTerm(new IndexStorageContext(this));

            Allocator.Initialization();

            if (exists)
            {
                Header = IndexFile.Read(Header);
                Allocator = IndexFile.Read(Allocator);
                Statistic = IndexFile.Read(Statistic);
                Term = IndexFile.Read(Term);
            }
            else
            {
                IndexFile.Write(Header);
                IndexFile.Write(Allocator);
                IndexFile.Write(Statistic);
                IndexFile.Write(Term);
            }

            IndexFile.Flush();
        }

        /// <summary>
        /// Adds a item to the index.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
        /// <param name="item">The data to be added to the index.</param>
        public void Add(T item)
        {
            var value = Property?.GetValue(item)?.ToString();
            var terms = Context.TokenAnalyzer.Analyze(value, Culture);

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
                Term.Add(term.Value.ToString())?
                    .AddPosting(item.Id)?
                    .AddPosition(term.Position);

                Statistic.Count++;
                IndexFile.Write(Statistic);
            }
        }

        /// <summary>
        /// The data to be removed from the index.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexData interface.</typeparam>
        /// <param name="item">The data to be removed from the field.</param>
        public void Remove(T item)
        {
            var value = Property?.GetValue(item)?.ToString();
            var terms = Context.TokenAnalyzer.Analyze(value?.ToString(), Culture);

            Remove(item, terms);
        }

        /// <summary>
        /// The data to be removed from the index.
        /// </summary>
        /// <param name="item">The data to be removed from the field.</param>
        /// <param name="terms">The terms to add to the reverse index for the given item.</param>
        public void Remove(T item, IEnumerable<IndexTermToken> terms)
        {
            foreach (var term in terms)
            {
                var node = Term[term.Value.ToString()];

                if (node != null)
                {
                    if (node.RemovePosting(item.Id))
                    {
                        Statistic.Count--;
                        IndexFile.Write(Statistic);
                    }
                }
            }
        }

        /// <summary>
        /// Removed all data from the index.
        /// </summary>
        public void Clear()
        {
            IndexFile.NextFreeAddr = 0;

            Header = new IndexStorageSegmentHeader(new IndexStorageContext(this)) { Identifier = "wri" };
            Allocator = new IndexStorageSegmentAllocator(new IndexStorageContext(this));
            Statistic = new IndexStorageSegmentStatistic(new IndexStorageContext(this));
            Term = new IndexStorageSegmentTerm(new IndexStorageContext(this));

            Allocator.Initialization();

            IndexFile.Write(Header);
            IndexFile.Write(Allocator);
            IndexFile.Write(Statistic);
            IndexFile.Write(Term);

            IndexFile.Flush();
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

                        foreach (var posting in Term.GetPostings(firstTerm.Value.ToString()))
                        {
                            foreach (var position in posting.Positions)
                            {
                                if (CheckForPhraseMatch(posting.DocumentID, position.Position, firstTerm.Position, options.Distance, nextTerms))
                                {
                                    distinct.Add(posting.DocumentID);
                                }
                            }
                        }

                        break;
                    }
                default:
                    {
                        if (options.Distance == 0)
                        {
                            foreach (var document in terms.Take(1).SelectMany(x => Term.Retrieve(x.Value.ToString(), options)))
                            {
                                if (distinct.Add(document) && count++ >= options.MaxResults)
                                {
                                    break;
                                }
                            }

                            foreach (var normalized in terms.Skip(1))
                            {
                                var temp = new HashSet<Guid>(distinct.Count);

                                foreach (var document in Term.Retrieve(normalized.Value.ToString(), options))
                                {
                                    if (distinct.Contains(document) && temp.Add(document))
                                    {
                                    }
                                }

                                distinct = temp;
                            }
                        }
                        else
                        {
                            var firstTerm = terms.Take(1).FirstOrDefault();
                            var nextTerms = terms.Skip(1);

                            foreach (var posting in Term.GetPostings(firstTerm.Value.ToString()))
                            {
                                foreach (var position in posting.Positions)
                                {
                                    if (CheckForProximityMatch(posting.DocumentID, position.Position, options.Distance, nextTerms))
                                    {
                                        distinct.Add(posting.DocumentID);
                                    }
                                }
                            }
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
        /// <returns>True if there is an exact match, otherwise false.</returns>
        private bool CheckForPhraseMatch(Guid document, uint position, uint offset, uint distance, IEnumerable<IndexTermToken> terms)
        {
            if (!terms.Any())
            {
                return true;
            }

            var firstTerm = terms.Take(1).FirstOrDefault();
            var nextTerms = terms.Skip(1);

            foreach (var posting in Term.GetPostings(firstTerm.Value.ToString()).Where(x => x?.DocumentID == document))
            {
                foreach (var pos in posting.Positions.Where
                (
                    x =>
                    x.Position >= position + (firstTerm.Position - offset) &&
                    x.Position <= position + (firstTerm.Position - offset) + distance
                ))
                {
                    return CheckForPhraseMatch(posting.DocumentID, pos.Position, firstTerm.Position, distance, nextTerms);
                }
            }

            return false;
        }

        /// <summary>
        /// Checks whether there is an exact match.
        /// </summary>
        /// <param name="document">The document id to check.</param>
        /// <param name="position">The position of the first term in the search text.</param>
        /// <param name="terms">Further following search terms.</param>
        /// <returns>True if there is an exact match, otherwise false.</returns>
        private bool CheckForProximityMatch(Guid document, uint position, uint distance, IEnumerable<IndexTermToken> terms)
        {
            if (!terms.Any())
            {
                return true;
            }

            var firstTerm = terms.Take(1).FirstOrDefault();
            var nextTerms = terms.Skip(1);

            foreach (var posting in Term.GetPostings(firstTerm.Value.ToString()).Where(x => x?.DocumentID == document))
            {
                foreach (var pos in posting.Positions.Where
                (
                    x =>
                    (
                        x.Position >= position &&
                        x.Position <= position + distance
                    ) ||
                    (
                        x.Position <= position &&
                        x.Position >= (int)position - distance
                    )
                ))
                {
                    return CheckForProximityMatch(posting.DocumentID, position, distance, nextTerms);
                }
            }

            return false;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, 
        /// or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            IndexFile.Dispose();
        }
    }
}