using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WebExpress.WebIndex.Term;
using WebExpress.WebIndex.Utility;

namespace WebExpress.WebIndex.Memory
{
    /// <summary>
    /// Provides a reverse index that manages the data in the main memory.
    /// </summary>
    /// <param name="context">The index context.</param>
    /// <param name="field">The field that makes up the index.</param>
    /// <param name="culture">The culture.</param>
    public class IndexMemoryReverseTerm<TIndexItem> : IndexMemoryReverse<TIndexItem>
        where TIndexItem : IIndexItem
    {
        /// <summary>
        /// Gets the root term.
        /// </summary>
        public IndexMemorySegmentTermNode Root { get; private set; } = new();

        /// <summary>
        /// Gets all items.
        /// </summary>
        public override IEnumerable<Guid> All => Root.Terms
            .SelectMany(x => x.Item2.Postings)
            .Select(x => x.DocumentId)
            .Distinct();

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="context">The index context.</param>
        /// <param name="field">The field that makes up the index.</param>
        /// <param name="culture">The culture.</param>
        public IndexMemoryReverseTerm(IIndexDocumemntContext context, IndexFieldData field, CultureInfo culture)
            : base(context, field, culture)
        {
        }

        /// <summary>
        /// Adds an item to the index.
        /// </summary>
        /// <param name="item">The data to be added to the index.</param>
        public override void Add(TIndexItem item)
        {
            var value = Field.GetPropertyValue(item)?.ToString();
            var terms = Context.TokenAnalyzer.Analyze(value, Culture);

            Add(item, terms);
        }

        /// <summary>
        /// Adds an item to the index.
        /// </summary>
        /// <param name="item">The data to be added to the index.</param>
        /// <param name="terms">The terms to add to the reverse index for the given item.</param>
        public override void Add(TIndexItem item, IEnumerable<IndexTermToken> terms)
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
        public override void Delete(TIndexItem item)
        {
            var value = Field.GetPropertyValue(item);
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
                Root.Remove(term.Value.ToString(), item.Id);
            }
        }

        /// <summary>
        /// Removed all data from the index.
        /// </summary>
        public override void Clear()
        {
            Root = new IndexMemorySegmentTermNode();
        }

        /// <summary>
        /// Drop the reverse index.
        /// </summary>
        public override void Drop()
        {

        }

        /// <summary>
        /// Return all items for a given input.
        /// </summary>
        /// <param name="input">The input.</param>
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

                        foreach (var posting in Root.GetPostings(firstTerm.Value.ToString()))
                        {
                            foreach (var position in posting.Positions)
                            {
                                if (CheckForPhraseMatch(posting.DocumentId, position, firstTerm.Position, options.Distance, nextTerms))
                                {
                                    distinct.Add(posting.DocumentId);
                                }
                            }
                        }

                        break;
                    }
                default:
                    {
                        if (options.Distance == 0)
                        {
                            foreach (var document in tokens.Take(1).SelectMany(x => RetrieveTerm(x.Value.ToString(), options)))
                            {
                                if (distinct.Add(document) && count++ >= options.MaxResults)
                                {
                                    break;
                                }
                            }

                            foreach (var normalized in tokens.Skip(1))
                            {
                                var temp = new HashSet<Guid>(distinct.Count);

                                foreach (var document in RetrieveTerm(normalized.Value.ToString(), options))
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
                            // proximity search: all terms must occur within the given
                            // distance of each other (mirrors the storage variant)
                            var firstTerm = tokens.Take(1).FirstOrDefault();
                            var nextTerms = tokens.Skip(1);

                            foreach (var posting in Root.GetPostings(firstTerm.Value.ToString()))
                            {
                                foreach (var position in posting.Positions)
                                {
                                    if (CheckForProximityMatch(posting.DocumentId, position, options.Distance, nextTerms))
                                    {
                                        distinct.Add(posting.DocumentId);
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
        /// Returns the document ids for a single term. When a similarity threshold
        /// is set, the term vocabulary is scanned and every term whose Levenshtein
        /// similarity reaches the threshold contributes its documents (fuzzy search).
        /// </summary>
        /// <param name="term">The (normalized) search term.</param>
        /// <param name="options">The retrieval options.</param>
        /// <returns>An enumeration of matching document ids.</returns>
        private IEnumerable<Guid> RetrieveTerm(string term, IndexRetrieveOptions options)
        {
            if (options.Similarity is > 0 and < 100)
            {
                var threshold = options.Similarity / 100.0;

                foreach (var (candidate, node) in Root.Terms)
                {
                    if (IndexFuzzy.CalculateLevenshteinSimilarity(term, candidate) >= threshold)
                    {
                        foreach (var posting in node.Postings ?? [])
                        {
                            yield return posting.DocumentId;
                        }
                    }
                }

                yield break;
            }

            foreach (var id in Root.Retrieve(term, options))
            {
                yield return id;
            }
        }

        /// <summary>
        /// Checks whether the subsequent terms match in phrase order, allowing the
        /// given distance tolerance between the expected and actual positions.
        /// </summary>
        /// <param name="document">The document id to check.</param>
        /// <param name="position">The position of the term within the document.</param>
        /// <param name="offset">The position within the search term.</param>
        /// <param name="distance">The allowed distance tolerance.</param>
        /// <param name="terms">Further following search terms.</param>
        /// <returns>True if there is a match, otherwise false.</returns>
        private bool CheckForPhraseMatch(Guid document, uint position, uint offset, uint distance, IEnumerable<IndexTermToken> terms)
        {
            if (!terms.Any())
            {
                return true;
            }

            var firstTerm = terms.Take(1).FirstOrDefault();
            var nextTerms = terms.Skip(1);

            // compute uint-safe bounds (mirrors the storage variant)
            var baseOffset = firstTerm.Position >= offset ? firstTerm.Position - offset : 0u;
            var minU = position + (ulong)baseOffset;
            var maxU = minU + distance;
            var min = minU > uint.MaxValue ? uint.MaxValue : (uint)minU;
            var max = maxU > uint.MaxValue ? uint.MaxValue : (uint)maxU;

            foreach (var posting in Root.GetPostings(firstTerm.Value.ToString()).Where(x => x?.DocumentId == document))
            {
                foreach (var pos in posting.Positions.Where(x => x >= min && x <= max))
                {
                    return CheckForPhraseMatch(posting.DocumentId, pos, firstTerm.Position, distance, nextTerms);
                }
            }

            return false;
        }

        /// <summary>
        /// Checks whether there is a proximity match within a given distance window.
        /// </summary>
        /// <param name="document">The document id to check.</param>
        /// <param name="position">The absolute position of the previously matched term.</param>
        /// <param name="distance">The allowed distance tolerance.</param>
        /// <param name="terms">The remaining terms to check.</param>
        /// <returns>True if a proximity match is found, otherwise false.</returns>
        private bool CheckForProximityMatch(Guid document, uint position, uint distance, IEnumerable<IndexTermToken> terms)
        {
            if (!terms.Any())
            {
                return true;
            }

            var firstTerm = terms.Take(1).FirstOrDefault();
            var nextTerms = terms.Skip(1);

            // compute uint-safe bounds around the current position
            var lower = position >= distance ? position - distance : 0u;
            var upperU = position + (ulong)distance;
            var upper = upperU > uint.MaxValue ? uint.MaxValue : (uint)upperU;

            foreach (var posting in Root.GetPostings(firstTerm.Value.ToString()).Where(x => x?.DocumentId == document))
            {
                foreach (var pos in posting.Positions.Where(x => x >= lower && x <= upper))
                {
                    return CheckForProximityMatch(posting.DocumentId, pos, distance, nextTerms);
                }
            }

            return false;
        }
    }
}
