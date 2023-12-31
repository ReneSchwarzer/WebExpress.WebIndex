using System;
using System.Collections.Generic;

namespace WebExpress.WebIndex.Memory
{
    /// <summary>
    /// The forward index.
    /// Key: The id of the item.
    /// Value: The item.
    /// </summary>
    public class WebIndexMemoryForward<T> : Dictionary<Guid, T>, IWebIndexForward<T> where T : IWebIndexItem
    {
        /// <summary>
        /// Returns all items.
        /// </summary>
        public IEnumerable<T> All => Values;

        /// <summary>
        /// Returns the index context.
        /// </summary>
        public IWebIndexContext Context { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">The index context.</param>
        /// <param name="capacity">The predicted capacity (number of items to store) of the reverse index.</param>
        public WebIndexMemoryForward(IWebIndexContext context, uint capacity)
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
