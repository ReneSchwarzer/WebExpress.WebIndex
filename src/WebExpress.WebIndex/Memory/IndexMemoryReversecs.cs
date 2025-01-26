using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// <param name="field">The field that makes up the index.</param>
    /// <param name="culture">The culture.</param>
    public abstract class IndexMemoryReverse<TIndexItem>(IIndexDocumemntContext context, IndexFieldData field, CultureInfo culture) : IIndexReverse<TIndexItem>
        where TIndexItem : IIndexItem
    {
        /// <summary>
        /// The field that makes up the index.
        /// </summary>
        protected IndexFieldData Field { get; } = field;

        /// <summary>
        /// Returns the index context.
        /// </summary>
        public IIndexDocumemntContext Context { get; private set; } = context;

        /// <summary>
        /// Returns the culture.
        /// </summary>
        public CultureInfo Culture { get; private set; } = culture;

        /// <summary>
        /// Returns all items.
        /// </summary>
        public virtual IEnumerable<Guid> All { get; }

        /// <summary>
        /// Adds a item to the index.
        /// </summary>
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
        /// <param name="input">The input.</param>
        /// <param name="options">The retrieve options.</param>
        /// <returns>An enumeration of the data ids.</returns>
        public abstract IEnumerable<Guid> Retrieve(object input, IndexRetrieveOptions options);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, 
        /// or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
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
