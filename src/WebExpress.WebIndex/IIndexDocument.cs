using System;
using System.Collections.Generic;
using System.Reflection;

namespace WebExpress.WebIndex
{
    public interface IIndexDocument : IDisposable
    {
    }

    public interface IIndexDocument<T> : IIndexDocument where T : IIndexItem
    {
        /// <summary>
        /// Returns the document store.
        /// </summary>
        IIndexDocumentStore<T> DocumentStore { get; }

        /// <summary>
        /// Return the index field names.
        /// </summary>
        IEnumerable<string> Fields { get; }

        /// <summary>
        /// Adds a item to the index.
        /// </summary>
        /// <param name="item">The data to be added to the index.</param>
        void Add(T item);

        /// <summary>
        /// Removed all data from the index.
        /// </summary>
        void Clear();

        /// <summary>
        /// The data to be removed from the index.
        /// </summary>
        /// <param name="item">The data to be removed from the index.</param>
        void Remove(T item);

        /// <summary>
        /// Rebuilds the index.
        /// </summary>
        /// <param name="capacity">The predicted capacity (number of items to store) of the index.</param>
        void ReBuild(uint capacity);

        /// <summary>
        /// Returns an index field based on its name.
        /// </summary>
        /// <param name="property">The property that makes up the index.</param>
        /// <returns>The index field or null.</returns>
        IIndexReverse<T> GetReverseIndex(PropertyInfo property);
    }
}
