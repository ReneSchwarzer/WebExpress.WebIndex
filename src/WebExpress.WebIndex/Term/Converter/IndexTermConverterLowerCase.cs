using System.Collections.Generic;
using System.Globalization;

namespace WebExpress.WebIndex.Term.Converter
{
    /// <summary>
    /// Conversion to lowercase.
    /// </summary>
    public static class IndexTermConverterLowerCase
    {
        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="context">The reference to the context.</param>
        public static void Initialization(IIndexContext context)
        {
        }

        /// <summary>
        /// Converts specific elements on the term enumeration in lower case.
        /// </summary>
        /// <param name="input">The terms.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>The terms in lower case.</returns>
        public static IEnumerable<IndexTermToken> LowerCase(this IEnumerable<IndexTermToken> input, CultureInfo culture)
        {
            foreach (var token in input)
            {
                token.Value = token.Value.ToLowerInvariant();
                yield return token;
            }
        }
    }
}
