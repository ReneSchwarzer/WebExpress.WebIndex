using System;

namespace WebExpress.WebIndex.Memory
{
    /// <summary>
    /// Saves the referenc to the items.
    /// </summary>
    public class IndexMemoryReversePosting<T> : IndexMemoryReversePosition where T : IIndexItem
    {
        /// <summary>
        /// Returns or sets the item id.
        /// </summary>
        public Guid Id { get; private set;}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="position">The position of the term in the input value.</param>
        public IndexMemoryReversePosting(T item, uint position)
        {
            Id = item.Id;
            
            Add(position);
        }

        /// <summary>
        /// Converts the order expression to a string.
        /// </summary>
        /// <returns>The order expression as a string.</returns>
        public override string ToString()
        {
            return $"{Id} : {base.ToString()}";
        }
    }
}
