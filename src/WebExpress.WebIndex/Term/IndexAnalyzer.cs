using System.Collections.Generic;
using System.Globalization;
using WebExpress.WebIndex.Term.Converter;
using WebExpress.WebIndex.Term.Filter;

namespace WebExpress.WebIndex.Term
{
    /// <summary>
    /// The analyzer decomposes and processes the input string into a sequence of terms.
    /// </summary>
    public static class IndexAnalyzer
    {
        /// <summary>
        /// Returns the whitespace tokinizer.
        /// </summary>
        private static IndexTermTokenizer Tokenizer { get; } = new IndexTermTokenizer();

        /// <summary>
        /// Analyze the input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>The terms.</returns>
        public static IEnumerable<IndexTermToken> Analyze(string input, CultureInfo culture)
        {
            return Tokenizer
                .Tokenize(input)
                .LowerCase()
                .Normalize()
                .Synonym()
                .StopWord(culture);
        }
    }
}
