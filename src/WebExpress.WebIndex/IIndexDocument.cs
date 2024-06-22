using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace WebExpress.WebIndex
{
    /// <summary>
    /// Defines the basic functionality of an index document.
    /// </summary>
    public interface IIndexDocument : IDisposable
    {
        /// <summary>
        /// Event that is triggered when the schema has changed.
        /// </summary>
        event EventHandler<IndexSchemaMigrationEventArgs> SchemaChanged;
    }

    /// <summary>
    /// Defines the functionality of an index document for a specific type of index item.
    /// </summary>
    /// <typeparam name="T">The type of the index item. This type parameter must implement the IIndexItem interface.</typeparam>
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
        /// Returns all documents from the index.
        /// </summary>
        IEnumerable<T> All { get; }

        /// <summary>
        /// Adds a item to the index.
        /// </summary>
        /// <param name="item">The data to be added to the index.</param>
        void Add(T item);

        /// <summary>
        /// Performs an asynchronous addition of an item in the index.
        /// </summary>
        /// <param name="item">The data to be added to the index.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AddAsync(T item);

        /// <summary>
        /// Updates a item in the index.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
        /// <param name="item">The data to be updated to the index.</param>
        void Update(T item);

        /// <summary>
        /// Performs an asynchronous update of an item in the index.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
        /// <param name="item">The data to be updated to the index.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateAsync(T item);

        /// <summary>
        /// The data to be removed from the index.
        /// </summary>
        /// <param name="item">The data to be removed from the index.</param>
        void Remove(T item);

        /// <summary>
        /// Removes an item from the index asynchronously.
        /// </summary>
        /// <param name="item">The data to be removed from the index.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task RemoveAsync(T item);

        /// <summary>
        /// Drop all index documents of type T.
        /// </summary>
        void Drop();

        /// <summary>
        /// Asynchronously drops all index documents of type T.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DropAsync();

        /// <summary>
        /// Removed all data from the index.
        /// </summary>
        void Clear();

        /// <summary>
        /// Removed all data from the index asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ClearAsync();

        /// <summary>
        /// Returns an index field based on its name.
        /// </summary>
        /// <param name="property">The property that makes up the index.</param>
        /// <returns>The index field or null.</returns>
        IIndexReverse<T> GetReverseIndex(PropertyInfo property);
    }
}
