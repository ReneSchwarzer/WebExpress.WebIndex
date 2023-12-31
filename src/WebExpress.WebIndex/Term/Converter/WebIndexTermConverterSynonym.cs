using System.Collections.Generic;
using System.Globalization;

namespace WebExpress.WebIndex.Term.Converter
{
    /// <summary>
    /// Conversion of synonyms.
    /// </summary>
    public static class WebIndexTermConverterSynonym
    {
        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="context">The reference to the context.</param>
        public static void Initialization(IWebIndexContext context)
        {
        }

        /// <summary>
        /// Converts specific elements on the term enumeration.
        /// </summary>
        /// <param name="input">The terms.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>The terms at which the synonyms have been converted.</returns>
        public static IEnumerable<WebIndexTermToken> Synonym(this IEnumerable<WebIndexTermToken> input, CultureInfo culture)
        {
            foreach (var token in input)
            {
                yield return token;
            }
        }
    }
}
