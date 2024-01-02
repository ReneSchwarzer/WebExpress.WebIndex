using System;
using WebExpress.WebIndex.WebAttribute;

namespace WebExpress.WebIndex
{
    public interface IIndexItem
    {
        /// <summary>
        /// The id of the item.
        /// </summary>
        [IndexIgnore]
        Guid Id { get; }
    }
}
