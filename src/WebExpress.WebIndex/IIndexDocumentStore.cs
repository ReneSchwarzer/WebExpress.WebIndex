using System;
using System.Collections.Generic;

namespace WebExpress.WebIndex
{
    public interface IIndexDocumentStore<T> : IDisposable where T : IIndexItem
    {
        /// <summary>
        /// Returns all document items.
        /// </summary>
        IEnumerable<T> All { get; }

        /// <summary>
        /// Returns the predicted capacity (number of items to store).
        /// </summary>
        uint Capacity { get; }

        /// <summary>
        /// Adds a item to the index.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexData interface.</typeparam>
        /// <param name="item">The data to be added to the index.</param>
        void Add(T item);

        /// <summary>
        /// Update an item.
        /// </summary>
        /// <param name="item">The item.</param>
        void Update(T item);

        /// <summary>
        /// Removed all data from the document store.
        /// </summary>
        void Clear();

        /// <summary>
        /// The data to be removed from the document store.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexData interface.</typeparam>
        /// <param name="item">The data to be removed from the document store.</param>
        void Delete(T item);

        /// <summary>
        /// Drop the index document store.
        /// </summary>
        void Drop();

        /// <summary>
        /// Returns the item.
        /// </summary>
        /// <param name="id">The id of the item.</param>
        /// <returns>The item.</returns>
        T GetItem(Guid id);
    }
}
