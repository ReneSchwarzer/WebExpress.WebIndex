using System.Collections.Generic;

namespace WebExpress.WebIndex.Term.Filter
{
    /// <summary>
    /// Removes stop words
    /// </summary>
    public static class IndexTermFilterStopWordExtensions
    {
        /// <summary>
        /// Filters specific elements on the term enumeration.
        /// </summary>
        /// <param name="input">The terms.</param>
        /// <returns>The filtered term enumeration.</returns>
        public static IEnumerable<IndexTermToken> StopWord(this IEnumerable<IndexTermToken> input)
        {
            foreach (var token in input)
            {
                yield return token;
            }
        }
    }
}
