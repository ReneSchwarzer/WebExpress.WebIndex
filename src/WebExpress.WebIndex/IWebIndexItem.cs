using System;
using WebExpress.WebIndex.WebAttribute;

namespace WebExpress.WebIndex
{
    public interface IWebIndexItem
    {
        /// <summary>
        /// The id of the item.
        /// </summary>
        [WebIndexIgnore]
        Guid Id { get; }
    }
}
