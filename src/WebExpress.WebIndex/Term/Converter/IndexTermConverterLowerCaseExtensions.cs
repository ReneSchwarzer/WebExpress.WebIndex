using System.Collections.Generic;

namespace WebExpress.WebIndex.Term.Converter
{
    /// <summary>
    /// Conversion to lowercase.
    /// </summary>
    public static class IndexTermConverterLowerCaseExtensions
    {
        /// <summary>
        /// Converts specific elements on the term enumeration in lower case.
        /// </summary>
        /// <param name="input">The terms.</param>
        /// <returns>The terms in lower case.</returns>
        public static IEnumerable<IndexTermToken> LowerCase(this IEnumerable<IndexTermToken> input)
        {
            foreach (var token in input)
            {
                token.Value = token.Value.ToLowerInvariant();
                yield return token;
            }
        }
    }
}
