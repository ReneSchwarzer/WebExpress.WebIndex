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
        public IndexStorageSegmentTreeNode Tree { get; private set; }

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
        public IIndexContext Context { get; private set; }

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
        public IndexStorageReverse(IIndexContext context, PropertyInfo property, CultureInfo culture)
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
            Tree = new IndexStorageSegmentTreeNode(new IndexStorageContext(this));

            Allocator.Alloc(Statistic);
            Allocator.Alloc(Tree);

            if (exists)
            {
                Header = IndexFile.Read(Header);
                Allocator = IndexFile.Read(Allocator);
                Statistic = IndexFile.Read(Statistic);
                Tree = IndexFile.Read(Tree);
            }
            else
            {
                IndexFile.Write(Header);
                IndexFile.Write(Allocator);
                IndexFile.Write(Statistic);
                IndexFile.Write(Tree);
            }

            IndexFile.Flush();
        }

        /// <summary>
        /// Adds a item to the field.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
        /// <param name="item">The data to be added to the index.</param>
        public void Add(T item)
        {
            var value = Property?.GetValue(item)?.ToString();
            var terms = IndexAnalyzer.Analyze(value, Culture);

            foreach (var term in terms)
            {
                Tree.Add(term.Value)
                    .Term
                    .Postings[item.Id]
                    .Add(new IndexStorageSegmentPosting(new IndexStorageContext(this)) { DocumentID = item.Id })
                    .Positions
                    .Add(new IndexStorageSegmentPosition(new IndexStorageContext(this)) { Position = term.Position });
            }
        }

        /// <summary>
        /// Removed all data from the index.
        /// </summary>
        public void Clear()
        {
            Header = new IndexStorageSegmentHeader(new IndexStorageContext(this)) { Identifier = "wri" };
            Allocator = new IndexStorageSegmentAllocator(new IndexStorageContext(this));
            Statistic = new IndexStorageSegmentStatistic(new IndexStorageContext(this));
            Tree = new IndexStorageSegmentTreeNode(new IndexStorageContext(this));

            Allocator.Alloc(Statistic);
            Allocator.Alloc(Tree);

            IndexFile.Write(Header);
            IndexFile.Write(Allocator);
            IndexFile.Write(Statistic);
            IndexFile.Write(Tree);

            IndexFile.Flush();
        }

        /// <summary>
        /// The data to be removed from the field.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexData interface.</typeparam>
        /// <param name="item">The data to be removed from the field.</param>
        public void Remove(T item)
        {

        }

        /// <summary>
        /// Return all items for a given term.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <returns>An enumeration of the data ids.</returns>
        public IEnumerable<Guid> Collect(object term)
        {
            var list = new List<Guid>();
            var terms = IndexAnalyzer.Analyze(term?.ToString(), Culture);

            foreach (var normalized in terms)
            {
                var documents = Tree[normalized.Value]
                    ?.Term
                    ?.Postings
                    ?.All
                    .Select(x => x.DocumentID);

                list.AddRange(documents ?? Enumerable.Empty<Guid>());
            }

            return list;
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