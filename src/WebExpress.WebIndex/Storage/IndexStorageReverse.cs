using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using WebExpress.WebIndex.Term;

namespace WebExpress.WebIndex.Storage
{
    /// <summary>
    /// The web reverse index, which stores the key-value pairs on disk.
    /// </summary>
    /// <typeparam name="TIndexItem">The data type. This must have the IIndexItem interface.</typeparam>
    public abstract class IndexStorageReverse<TIndexItem> : IIndexReverse<TIndexItem>, IIndexStorage
        where TIndexItem : IIndexItem
    {
        /// <summary>
        /// The field that makes up the index.
        /// </summary>
        protected IndexFieldData Field { get; private set; }

        /// <summary>
        /// Returns the file name for the reverse index.
        /// </summary>
        public string FileName { get; protected set; }

        /// <summary>
        /// Returns or sets the reverse index file.
        /// </summary>
        public IndexStorageFile IndexFile { get; protected set; }

        /// <summary>
        /// Returns or sets the header.
        /// </summary>
        public IndexStorageSegmentHeader Header { get; protected set; }

        /// <summary>
        /// Returns or sets the memory manager.
        /// </summary>
        public IndexStorageSegmentAllocator Allocator { get; protected set; }

        /// <summary>
        /// Returns the statistical values that can be help to optimize the index.
        /// </summary>
        public IndexStorageSegmentStatistic Statistic { get; protected set; }

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
        public abstract IEnumerable<Guid> All { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexStorageReverse{TIndexItem}"/> class.
        /// </summary>
        /// <param name="context">The context of the indexed document.</param>
        /// <param name="field">The field that makes up the index.</param>
        /// <param name="culture">The culture information.</param>
        public IndexStorageReverse(IIndexDocumemntContext context, IndexFieldData field, CultureInfo culture)
        {
            Context = context;
            Field = field;
            Culture = culture;
        }

        /// <summary>
        /// Adds a item to the index.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
        /// <param name="item">The data to be added to the index.</param>
        public abstract void Add(TIndexItem item);

        /// <summary>
        /// Adds a item to the index.
        /// </summary>
        /// <param name="item">The data to be added to the index.</param>
        /// <param name="terms">The terms to add to the reverse index for the given item.</param>
        public abstract void Add(TIndexItem item, IEnumerable<IndexTermToken> terms);

        /// <summary>
        /// The data to be removed from the index.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexData interface.</typeparam>
        /// <param name="item">The data to be removed from the field.</param>
        public abstract void Delete(TIndexItem item);

        /// <summary>
        /// The data to be removed from the index.
        /// </summary>
        /// <param name="item">The data to be removed from the field.</param>
        /// <param name="terms">The terms to add to the reverse index for the given item.</param>
        public abstract void Delete(TIndexItem item, IEnumerable<IndexTermToken> terms);

        /// <summary>
        /// Removed all data from the index.
        /// </summary>
        public abstract void Clear();

        /// <summary>
        /// Drop the reverse index.
        /// </summary>
        public abstract void Drop();

        /// <summary>
        /// Return all items for a given input.
        /// </summary>
        /// <param name="term">The input.</param>
        /// <param name="options">The retrieve options.</param>
        /// <returns>An enumeration of the data ids.</returns>
        public abstract IEnumerable<Guid> Retrieve(object input, IndexRetrieveOptions options);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, 
        /// or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            IndexFile.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Retrieves the value of a property from an object based on the specified field.
        /// </summary>
        /// <param name="item">The object from which to retrieve the property value.</param>
        /// <param name="field">The field that specifies the property to retrieve.</param>
        /// <returns>The value of the specified property, or null if the property is not found.</returns>
        protected static object GetPropertyValue(object item, IndexFieldData field)
        {
            var propertyNames = field.Name.Split('.');
            object currentObject = item;

            foreach (var propertyName in propertyNames)
            {
                if (currentObject == null)
                {
                    return null;
                }

                var property = currentObject.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
                currentObject = property.GetValue(currentObject);
            }

            return currentObject;
        }
    }
}