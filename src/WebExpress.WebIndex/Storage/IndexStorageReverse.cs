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
            if (exists)
            {
                File.Delete(FileName);
                exists = false;
            }
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
                Term.Add(term.Value)
                    .AddPosting(item.Id)
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
                var t = Term[term.Value];
                var posting = t?.Postings
                    .Where(x => x.DocumentID == item?.Id)
                    .FirstOrDefault();

                posting?.RemovePosition(term.Position);

                if (!posting.Positions.Any())
                {
                    t.RemovePosting(item.Id);

                    Statistic.Count--;
                    IndexFile.Write(Statistic);
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

            foreach (var normalized in terms)
            {
                foreach (var document in Term.Retrieve(normalized.Value, options))
                {
                    if (distinct.Add(document))
                    {
                        yield return document;

                        if (count++ >= options.MaxResults)
                        {
                            yield break;
                        }
                    }
                }
            }

            yield break;
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