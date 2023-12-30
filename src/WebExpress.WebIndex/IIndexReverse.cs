using System;
using System.Collections.Generic;

namespace WebExpress.WebIndex
{
    public interface IIndexReverse<T> : IDisposable where T : IIndexItem
    {
        /// <summary>
        /// Adds a item to the index.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexData interface.</typeparam>
        /// <param name="item">The data to be added to the index.</param>
        void Add(T item);

        /// <summary>
        /// Removed all data from the index.
        /// </summary>
        void Clear();

        /// <summary>
        /// The data to be removed from the index.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexData interface.</typeparam>
        /// <param name="item">The data to be removed from the index.</param>
        void Remove(T item);

        /// <summary>
        /// Return all items for a given term.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <returns>An enumeration of the data ids.</returns>
        IEnumerable<Guid> Collect(object term);
    }
}
