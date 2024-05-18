using System;

namespace WebExpress.WebIndex.Memory
{
    /// <summary>
    /// Saves the referenc to the items.
    /// </summary>
    public class IndexMemoryReversePosting<T> : IndexMemoryReversePosition where T : IIndexItem
    {
        /// <summary>
        /// Returns or sets the document id.
        /// </summary>
        public Guid DocumentID { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="position">The position of the term in the input value.</param>
        public IndexMemoryReversePosting(T item, uint position)
        {
            DocumentID = item.Id;

            Add(position);
        }

        /// <summary>
        /// Converts the order expression to a string.
        /// </summary>
        /// <returns>The order expression as a string.</returns>
        public override string ToString()
        {
            return $"{DocumentID} : {base.ToString()}";
        }
    }
}
