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
    public class WebIndexStorageReverse<T> : IWebIndexReverse<T>, IWebIndexStorage where T : IWebIndexItem
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
        public WebIndexStorageFile IndexFile { get; private set; }

        /// <summary>
        /// Returns or sets the header.
        /// </summary>
        public WebIndexStorageSegmentHeader Header { get; private set; }

        /// <summary>
        /// Returns or sets the hash map.
        /// </summary>
        public WebIndexStorageSegmentHashMap<WebIndexStorageSegmentTerm> HashMap { get; private set; }

        /// <summary>
        /// Returns or sets the memory manager.
        /// </summary>
        public WebIndexStorageSegmentAllocator Allocator { get; private set; }

        /// <summary>
        /// Returns the statistical values that can be help to optimize the index.
        /// </summary>
        public WebIndexStorageSegmentStatistic Statistic { get; private set; }

        /// <summary>
        /// Returns the index context.
        /// </summary>
        public IWebIndexContext Context { get; private set; }

        /// <summary>
        /// Returns the culture.
        /// </summary>
        public CultureInfo Culture { get; private set; }

        /// <summary>
        /// Returns or sets the predicted capacity (number of items to store) of the reverse index.
        /// </summary>
        private uint Capacity { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">The index context.</param>
        /// <param name="property">The property that makes up the index.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="capacity">The predicted capacity (number of items to store) of the reverse index.</param>
        public WebIndexStorageReverse(IWebIndexContext context, PropertyInfo property, CultureInfo culture, uint capacity)
        {
            Context = context;
            Property = property;
            Culture = culture;
            Capacity = capacity;
            FileName = Path.Combine(Context.IndexDirectory, $"{typeof(T).Name}.{property.Name}.wri");

            var exists = File.Exists(FileName);
            IndexFile = new WebIndexStorageFile(FileName);
            Header = new WebIndexStorageSegmentHeader(new WebIndexStorageContext(this)) { Identifier = "wri" };
            Allocator = new WebIndexStorageSegmentAllocator(new WebIndexStorageContext(this));
            Statistic = new WebIndexStorageSegmentStatistic(new WebIndexStorageContext(this));
            HashMap = new WebIndexStorageSegmentHashMap<WebIndexStorageSegmentTerm>(new WebIndexStorageContext(this), Capacity);

            Allocator.Alloc(Statistic);
            Allocator.Alloc(HashMap);

            if (exists)
            {
                Header = IndexFile.Read(Header);
                Allocator = IndexFile.Read(Allocator);
                Statistic = IndexFile.Read(Statistic);
                HashMap = IndexFile.Read(HashMap);
            }
            else
            {
                IndexFile.Write(Header);
                IndexFile.Write(Allocator);
                IndexFile.Write(Statistic);
                IndexFile.Write(HashMap);
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
            var terms = WebIndexAnalyzer.Analyze(value, Culture);

            foreach (var term in terms)
            {
                HashMap[term.Value]
                    .Add(new WebIndexStorageSegmentTerm(new WebIndexStorageContext(this)) { Term = term.Value, Fequency = 1 })
                    .Postings[item.Id]
                    .Add(new WebIndexStorageSegmentPosting(new WebIndexStorageContext(this)) { DocumentID = item.Id })
                    .Positions
                    .Add(new WebIndexStorageSegmentPosition(new WebIndexStorageContext(this)) { Position = term.Position });
            }
        }

        /// <summary>
        /// Removed all data from the index.
        /// </summary>
        public void Clear()
        {
            Header = new WebIndexStorageSegmentHeader(new WebIndexStorageContext(this)) { Identifier = "wri" };
            Allocator = new WebIndexStorageSegmentAllocator(new WebIndexStorageContext(this));
            Statistic = new WebIndexStorageSegmentStatistic(new WebIndexStorageContext(this));
            HashMap = new WebIndexStorageSegmentHashMap<WebIndexStorageSegmentTerm>(new WebIndexStorageContext(this), Capacity);

            Allocator.Alloc(Statistic);
            Allocator.Alloc(HashMap);

            IndexFile.Write(Header);
            IndexFile.Write(Allocator);
            IndexFile.Write(Statistic);
            IndexFile.Write(HashMap);

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
            var terms = WebIndexAnalyzer.Analyze(term?.ToString(), Culture);

            foreach (var normalized in terms)
            {
                var documents = HashMap[normalized.Value]
                    .Where(x => x.Term.Equals(normalized.Value))
                    .SelectMany(x => x.Postings.All)
                    .Select(x => x.DocumentID);

                list.AddRange(documents);
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