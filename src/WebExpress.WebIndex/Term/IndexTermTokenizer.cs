using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebExpress.WebIndex.Term
{
    /// <summary>
    /// A whitespace tokinizer for breaking down a document into terms.
    /// </summary>
    public class IndexTermTokenizer
    {
        /// <summary>
        /// Enumeration of separators.
        /// </summary>
        private static char[] Delimiters { get; } = ['?', '!', ':', '<', '>', '=', '%', '(', ')', '\"', '“', '”', '\''];

        /// <summary>
        /// Enumeration of wildcards.
        /// </summary>
        public static char[] Wildcards { get; } = ['?', '*'];

        /// <summary>
        /// Tokenize an input string into an enumeration of terms.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="wildcards">A enumeration of wildcards.</param>
        /// <returns>An enumeration of terms.</returns>
        public IEnumerable<IndexTermToken> Tokenize(string input, char[] wildcards = null)
        {
            var currentToken = new StringBuilder();
            var position = (uint)0;
            var except = Delimiters.Except(wildcards ?? []);

            if (input == null || input.Length == 0)
            {
                yield break;
            }

            foreach (var c in input)
            {
                if (char.IsWhiteSpace(c) || except.Contains(c))
                {
                    if (currentToken.Length > 0)
                    {
                        yield return new IndexTermToken()
                        {
                            Position = position,
                            Value = currentToken.ToString()
                        };
                    }

                    currentToken = new StringBuilder();
                    position++;
                }
                else
                {
                    currentToken.Append(c);
                }
            }

            if (currentToken.Length > 0)
            {
                yield return new IndexTermToken()
                {
                    Position = position,
                    Value = currentToken.ToString()
                };
            }
        }
    }
}
