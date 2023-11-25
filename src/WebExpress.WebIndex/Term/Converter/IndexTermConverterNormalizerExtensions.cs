using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace WebExpress.WebIndex.Term.Converter
{
    /// <summary>
    /// Normalizes terms.
    /// </summary>
    public static class IndexTermConverterNormalizerExtensions
    {
        /// <summary>
        /// Converts specific elements on the term enumeration.
        /// </summary>
        /// <param name="input">The terms.</param>
        /// <returns>The normalized form of the terms.</returns>
        public static IEnumerable<IndexTermToken> Normalize(this IEnumerable<IndexTermToken> input)
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
