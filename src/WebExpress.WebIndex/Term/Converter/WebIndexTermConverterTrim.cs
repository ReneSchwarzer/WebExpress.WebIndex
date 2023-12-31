using System.Collections.Generic;
using System.Globalization;

namespace WebExpress.WebIndex.Term.Converter
{
    /// <summary>
    /// Removes unnecessary characters from the beginning and end of a term.
    /// </summary>
    public static class WebIndexTermConverterTrim
    {
        private static readonly char[] trimCharacters = ['.', ',', '-', '_', '…'];

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="context">The reference to the context.</param>
        public static void Initialization(IWebIndexContext context)
        {
        }

        /// <summary>
        /// Converts specific elements on the term enumeration in lower case.
        /// </summary>
        /// <param name="input">The terms.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>The trimmed terms.</returns>
        public static IEnumerable<WebIndexTermToken> Trim(this IEnumerable<WebIndexTermToken> input, CultureInfo culture)
        {
            foreach (var token in input)
            {
                token.Value = token.Value.Trim(trimCharacters);

                yield return token;
            }
        }
    }
}
