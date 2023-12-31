using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebExpress.WebIndex.Term
{
    /// <summary>
    /// A whitespace tokinizer for breaking down a document into terms.
    /// </summary>
    public class WebIndexTermTokenizer
    {
        /// <summary>
        /// Enumeration of separators.
        /// </summary>
        private static readonly char[] delimiters = [' ', '?', '!', ':', '<', '>', '=', '%', '(', ')', '\"', '“', '”', '\''];

        /// <summary>
        /// Tokenize an input string into an enumeration of terms.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>An enumeration of terms.</returns>
        public IEnumerable<WebIndexTermToken> Tokenize(string input)
        {
            var currentToken = new StringBuilder();
            var position = (uint)0;

            if (input == null || input.Length == 0)
            {
                yield break;
            }

            foreach (var c in input)
            {
                if (char.IsWhiteSpace(c) || delimiters.Contains(c))
                {
                    if (currentToken.Length > 0)
                    {
                        yield return new WebIndexTermToken()
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
                yield return new WebIndexTermToken()
                {
                    Position = position,
                    Value = currentToken.ToString()
                };
            }
        }
    }
}
