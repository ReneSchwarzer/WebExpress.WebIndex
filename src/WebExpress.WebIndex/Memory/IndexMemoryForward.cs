using System.Collections.Generic;

namespace WebExpress.WebIndex.Memory
{
    /// <summary>
    /// The forward index.
    /// Key: The id of the item.
    /// Value: The item.
    /// </summary>
    public class IndexMemoryForward<T> : Dictionary<int, T>, IIndexForward<T> where T : IIndexItem
    {
        /// <summary>
        /// Returns all items.
        /// </summary>
        public IEnumerable<T> All => Values;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">The index context.</param>
        /// <param name="capacity">The predicted capacity (number of items to store) of the reverse index.</param>
        public IndexMemoryForward(IIndexContext context, uint capacity)
            : base((int)capacity)
        {
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
                T value;
                Remove(item.Id, out value);
            }
        }

        /// <summary>
        /// Returns the item.
        /// </summary>
        /// <param name="id">The id of the item.</param>
        /// <returns>The item.</returns>
        public T GetItem(int id)
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
