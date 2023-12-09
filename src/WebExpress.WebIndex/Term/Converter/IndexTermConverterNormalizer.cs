using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace WebExpress.WebIndex.Term.Converter
{
    /// <summary>
    /// Normalizes terms.
    /// </summary>
    public static class IndexTermConverterNormalizer
    {
        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="context">The reference to the context.</param>
        public static void Initialization(IIndexContext context)
        {
        }

        /// <summary>
        /// Converts specific elements on the term enumeration.
        /// </summary>
        /// <param name="input">The terms.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>The normalized form of the terms.</returns>
        public static IEnumerable<IndexTermToken> Normalize(this IEnumerable<IndexTermToken> input, CultureInfo culture)
        {
            foreach (var token in input)
            {
                token.Value = Normalize(token.Value);
                yield return token;
            }
        }

        /// <summary>
        /// Converts an input string into a standardized form.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>The normalized form of the input string.</returns>
        private static string Normalize(string input)
        {
            var normalized = input.Normalize(NormalizationForm.FormKD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString();
        }
    }
}
