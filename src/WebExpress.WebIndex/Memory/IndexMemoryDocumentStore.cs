using System;
using System.Collections.Generic;

namespace WebExpress.WebIndex.Memory
{
    /// <summary>
    /// The document store.
    /// Key: The id of the item.
    /// Value: The item.
    /// </summary>
    public class IndexMemoryDocumentStore<T> : Dictionary<Guid, T>, IIndexDocumentStore<T> where T : IIndexItem
    {
        /// <summary>
        /// Returns all items.
        /// </summary>
        public IEnumerable<T> All => Values;


        /// <summary>
        /// Returns the predicted capacity (number of items to store) of the document store.
        /// </summary>
        public uint Capacity => (uint)Count;

        /// <summary>
        /// Returns the index context.
        /// </summary>
        public IIndexContext Context { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">The index context.</param>
        /// <param name="capacity">The predicted capacity (number of items to store) of the document store.</param>
        public IndexMemoryDocumentStore(IIndexContext context, uint capacity)
            : base((int)capacity)
        {
            Context = context;
        }

        /// <summary>
        /// Adds an item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Add(T item)
        {
            if (!ContainsKey(item.Id))
            {
                Add(item.Id, item);
            }
        }

        /// <summary>
        /// Remove an item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Remove(T item)
        {
            if (ContainsKey(item.Id))
            {
                Remove(item.Id, out _);
            }
        }

        /// <summary>
        /// Returns the item.
        /// </summary>
        /// <param name="id">The id of the item.</param>
        /// <returns>The item.</returns>
        public T GetItem(Guid id)
        {
            if (ContainsKey(id))
            {
                return this[id];
            }

            return default;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, 
        /// or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }
    }
}
