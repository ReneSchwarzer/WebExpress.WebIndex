﻿using System;
using System.Collections.Generic;
using WebExpress.WebIndex.Term;

namespace WebExpress.WebIndex
{
    /// <summary>
    /// Reverse index interface.
    /// </summary>
    /// <typeparam name="T">The data type. This must have the IIndexData interface.</typeparam>
    public interface IIndexReverse<T> : IDisposable where T : IIndexItem
    {
        /// <summary>
        /// Returns all items.
        /// </summary>
        IEnumerable<Guid> All { get; }

        /// <summary>
        /// Adds a item to the index.
        /// </summary>
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
        /// <param name="item">The data to be removed from the index.</param>
        void Delete(T item);

        /// <summary>
        /// The data to be removed from the index.
        /// </summary>
        /// <param name="terms">The terms to add to the reverse index for the given item.</param>
        void Delete(T item, IEnumerable<IndexTermToken> terms);

        /// <summary>
        /// Removed all data from the index.
        /// </summary>
        void Clear();

        /// <summary>
        /// Drop the reverse index.
        /// </summary>
        void Drop();

        /// <summary>
        /// Return all items for a given string.
        /// </summary>
        /// <param name="term">The term string.</param>
        /// <param name="options">The retrieve options.</param>
        /// <returns>An enumeration of the data ids.</returns>
        IEnumerable<Guid> Retrieve(string term, IndexRetrieveOptions options);
    }
}
