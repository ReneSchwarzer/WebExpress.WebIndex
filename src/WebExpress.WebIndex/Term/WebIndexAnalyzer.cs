using System.Collections.Generic;
using System.Globalization;
using WebExpress.WebIndex.Term.Converter;
using WebExpress.WebIndex.Term.Filter;

namespace WebExpress.WebIndex.Term
{
    /// <summary>
    /// The analyzer decomposes and processes the input string into a sequence of terms.
    /// </summary>
    public static class WebIndexAnalyzer
    {
        /// <summary>
        /// Returns the whitespace tokinizer.
        /// </summary>
        private static WebIndexTermTokenizer Tokenizer { get; } = new WebIndexTermTokenizer();

        /// <summary>
        /// Analyze the input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>The terms.</returns>
        public static IEnumerable<WebIndexTermToken> Analyze(string input, CultureInfo culture)
        {
            return Tokenizer
                .Tokenize(input)
                .Trim(culture)
                .LowerCase(culture)
                .Normalize(culture)
                .Misspelled(culture)
                .Synonym(culture)
                .Empty(culture)
                .StopWord(culture);
        }
    }
}
