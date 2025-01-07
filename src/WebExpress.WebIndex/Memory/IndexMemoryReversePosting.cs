using System;
using System.Collections.Generic;

namespace WebExpress.WebIndex.Memory
{
    /// <summary>
    /// Saves the referenc to the items.
    /// </summary>
    public class IndexMemoryReversePosting 
    {
        /// <summary>
        /// Returns or sets the document id.
        /// </summary>
        public Guid DocumentID { get; private set; }

        /// <summary>
        /// Returns the a list of the positions.
        /// </summary>
        public IndexMemoryReversePosition Positions { get; private set;} = [];

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="id">The item.</param>
        /// <param name="position">The position of the term in the input value.</param>
        public IndexMemoryReversePosting(Guid id, uint position)
        {
            DocumentID = id;

            Positions.Add(position);
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
