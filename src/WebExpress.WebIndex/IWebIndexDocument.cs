using System;
using System.Collections.Generic;
using System.Reflection;

namespace WebExpress.WebIndex
{
    public interface IWebIndexDocument : IDisposable
    {
    }

    public interface IIndexDocument<T> : IWebIndexDocument where T : IWebIndexItem
    {
        /// <summary>
        /// Returns the forward index.
        /// </summary>
        IWebIndexForward<T> ForwardIndex { get; }

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
        /// Returns an index field based on its name.
        /// </summary>
        /// <param name="property">The property that makes up the index.</param>
        /// <returns>The index field or null.</returns>
        IWebIndexReverse<T> GetReverseIndex(PropertyInfo property);
    }
}
