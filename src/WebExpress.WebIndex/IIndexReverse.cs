using System;
using System.Collections.Generic;
using WebExpress.WebIndex.Term;

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
        /// Adds a item to the index.
        /// </summary>
        /// <param name="item">The data to be added to the index.</param>
        /// <param name="terms">The terms to add to the reverse index for the given item.</param>
        void Add(T item, IEnumerable<IndexTermToken> terms);

        /// <summary>
        /// The data to be removed from the index.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexData interface.</typeparam>
        /// <param name="item">The data to be removed from the index.</param>
        void Remove(T item);

        /// <summary>
        /// The data to be removed from the index.
        /// </summary>
        /// <param name="item">The data to be removed from the field.</param>
        /// <param name="terms">The terms to add to the reverse index for the given item.</param>
        void Remove(T item, IEnumerable<IndexTermToken> terms);

        /// <summary>
        /// Removed all data from the index.
        /// </summary>
        void Clear();

        /// <summary>
        /// Return all items for a given term.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <returns>An enumeration of the data ids.</returns>
        IEnumerable<Guid> Collect(object term);
    }
}
