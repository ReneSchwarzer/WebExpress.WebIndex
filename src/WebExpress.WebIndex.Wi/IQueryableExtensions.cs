using System.Collections;

namespace WebExpress.WebIndex.Wi
{
    /// <summary>
    /// Provides extension methods for IQueryable.
    /// </summary>
    internal static class IQueryableExtensions
    {
        /// <summary>
        /// Converts an IQueryable to an IEnumerable.
        /// </summary>
        /// <param name="queryableData">The IQueryable to convert.</param>
        /// <returns>An IEnumerable containing the elements of the input IQueryable.</returns>
        public static IEnumerable ToIEnumerable(this IQueryable queryableData)
        {
            foreach (var item in queryableData)
            {
                yield return item;
            }
        }
    }
}
