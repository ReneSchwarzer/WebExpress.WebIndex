using System.Collections.Generic;

namespace WebExpress.WebIndex.Term.Converter
{
    /// <summary>
    /// Conversion of synonyms.
    /// </summary>
    public static class IndexTermConverterSynonymExtensions
    {
        /// <summary>
        /// Converts specific elements on the term enumeration.
        /// </summary>
        /// <param name="input">The terms.</param>
        /// <returns>The terms at which the synonyms have been converted.</returns>
        public static IEnumerable<IndexTermToken> Synonym(this IEnumerable<IndexTermToken> input)
        {
            foreach (var token in input)
            {
                yield return token;
            }
        }
    }
}
