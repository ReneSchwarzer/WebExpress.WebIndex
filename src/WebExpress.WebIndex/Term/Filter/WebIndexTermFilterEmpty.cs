using System.Collections.Generic;
using System.Globalization;

namespace WebExpress.WebIndex.Term.Filter
{
    /// <summary>
    /// Removes emty words
    /// </summary>
    public static class WebIndexTermFilterEmpty
    {
        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="context">The reference to the context.</param>
        public static void Initialization(IWebIndexContext context)
        {
        }

        /// <summary>
        /// Filters specific elements on the term enumeration.
        /// </summary>
        /// <param name="input">The terms.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>The filtered term enumeration.</returns>
        public static IEnumerable<WebIndexTermToken> Empty(this IEnumerable<WebIndexTermToken> input, CultureInfo culture)
        {
            foreach (var token in input)
            {
                if (!string.IsNullOrWhiteSpace(token.Value))
                {
                    yield return token;
                }
            }
        }
    }
}
