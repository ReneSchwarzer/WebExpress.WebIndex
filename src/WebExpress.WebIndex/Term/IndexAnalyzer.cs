using System.Collections.Generic;
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
        /// <returns>The terms.</returns>
        public static IEnumerable<IndexTermToken> Analyze(string input)
        {
            return Tokenizer
                .Tokenize(input)
                .StopWord()
                .LowerCase()
                .Normalize()
                .Synonym();
        }
    }
}
