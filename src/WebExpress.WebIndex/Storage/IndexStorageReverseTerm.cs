using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using WebExpress.WebIndex.Term;

namespace WebExpress.WebIndex.Storage
{
    /// <summary>
    /// Implementation of the web reverse index, which stores the key-value pairs on disk.
    /// </summary>
    /// <typeparam name="TIndexItem">The data type. This must have the IIndexItem interface.</typeparam>
    public class IndexStorageReverseTerm<TIndexItem> : IndexStorageReverse<TIndexItem>, IIndexStorage
        where TIndexItem : IIndexItem
    {
        private readonly string _extentions = "wrt";
        private readonly int _version = 1;

        /// <summary>
        /// Returns or sets the term tree.
        /// </summary>
        public IndexStorageSegmentTerm Term { get; private set; }

        /// <summary>
        /// Returns all items.
        /// </summary>
        public override IEnumerable<Guid> All => Term.All.Distinct();

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="context">The index context.</param>
        /// <param name="field">The field that makes up the index.</param>
        /// <param name="culture">The culture.</param>
        public IndexStorageReverseTerm(IIndexDocumemntContext context, IndexFieldData field, CultureInfo culture)
            : base(context, field, culture)
        {
            FileName = Path.Combine(Context.IndexDirectory, $"{typeof(TIndexItem).Name}.{Field.Name}.{_extentions}");

            var exists = File.Exists(FileName);

            IndexFile = new IndexStorageFile(FileName);
            Header = new IndexStorageSegmentHeader(new IndexStorageContext(this)) { Identifier = _extentions, Version = (byte)_version };
            Allocator = new IndexStorageSegmentAllocatorReverseIndex(new IndexStorageContext(this));
            Statistic = new IndexStorageSegmentStatistic(new IndexStorageContext(this));
            Term = new IndexStorageSegmentTerm(new IndexStorageContext(this));

            Header.Initialization(exists);
            Statistic.Initialization(exists);
            Term.Initialization(exists);
            Allocator.Initialization(exists);

            IndexFile.Flush();
        }

        /// <summary>
        /// Adds a item to the index.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
        /// <param name="item">The data to be added to the index.</param>
        public override void Add(TIndexItem item)
        {
            var value = GetPropertyValue(item, Field)?.ToString();
            var terms = Context.TokenAnalyzer.Analyze(value, Culture);

            Add(item, terms);
        }

        /// <summary>
        /// Adds a item to the index.
        /// </summary>
        /// <param name="item">The data to be added to the index.</param>
        /// <param name="terms">The terms to add to the reverse index for the given item.</param>
        public override void Add(TIndexItem item, IEnumerable<IndexTermToken> terms)
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
        public override void Delete(TIndexItem item)
        {
            var value = GetPropertyValue(item, Field)?.ToString();
            var terms = Context.TokenAnalyzer.Analyze(value?.ToString(), Culture);

            Delete(item, terms);
        }

        /// <summary>
        /// The data to be removed from the index.
        /// </summary>
        /// <param name="item">The data to be removed from the field.</param>
        /// <param name="terms">The terms to add to the reverse index for the given item.</param>
        public override void Delete(TIndexItem item, IEnumerable<IndexTermToken> terms)
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
        public override void Clear()
        {
            IndexFile.NextFreeAddr = 0;
            IndexFile.InvalidationAll();
            IndexFile.Flush();

            Header = new IndexStorageSegmentHeader(new IndexStorageContext(this)) { Identifier = _extentions, Version = (byte)_version };
            Allocator = new IndexStorageSegmentAllocatorReverseIndex(new IndexStorageContext(this));
            Statistic = new IndexStorageSegmentStatistic(new IndexStorageContext(this));
            Term = new IndexStorageSegmentTerm(new IndexStorageContext(this));

            Header.Initialization(false);
            Statistic.Initialization(false);
            Term.Initialization(false);
            Allocator.Initialization(false);

            IndexFile.Flush();
        }

        /// <summary>
        /// Drop the reverse index.
        /// </summary>
        public override void Drop()
        {
            IndexFile.Delete();
        }

        /// <summary>
        /// Return all items for a given input.
        /// </summary>
        /// <param name="term">The input.</param>
        /// <param name="options">The retrieve options.</param>
        /// <returns>An enumeration of the data ids.</returns>
        public override IEnumerable<Guid> Retrieve(object input, IndexRetrieveOptions options)
        {
            var tokens = Context.TokenAnalyzer.Analyze(input?.ToString(), Culture, true);
            var distinct = new HashSet<Guid>((int)Math.Min(options.MaxResults, int.MaxValue / 2));
            var count = 0u;

            if (!tokens.Any())
            {
                return distinct;
            }

            switch (options.Method)
            {
                case IndexRetrieveMethod.Phrase:
                    {
                        var firstTerm = tokens.Take(1).FirstOrDefault();
                        var nextTerms = tokens.Skip(1);

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
                            foreach (var document in tokens.Take(1).SelectMany(x => Term.Retrieve(x.Value.ToString(), options)))
                            {
                                if (distinct.Add(document) && count++ >= options.MaxResults)
                                {
                                    break;
                                }
                            }

                            foreach (var normalized in tokens.Skip(1))
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
                            var firstTerm = tokens.Take(1).FirstOrDefault();
                            var nextTerms = tokens.Skip(1);

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
    }
}