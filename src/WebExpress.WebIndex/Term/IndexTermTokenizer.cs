using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace WebExpress.WebIndex.Term
{
    /// <summary>
    /// A whitespace tokinizer for breaking down a document into terms.
    /// </summary>
    public static class IndexTermTokenizer
    {
        /// <summary>
        /// Enumeration of separators.
        /// </summary>
        private static char[] Delimiters { get; } = ['?', '!', '&', ':', ';', '<', '>', '=', '|', '%', '(', ')', '"', '“', '”', '/', '.', '␦', '\''];

        /// <summary>
        /// Enumeration of wildcards.
        /// </summary>
        public static char[] Wildcards { get; } = ['?', '*'];

        /// <summary>
        /// Tokenize an input string into an enumeration of terms.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="wildcards">A enumeration of wildcards.</param>
        /// <returns>An enumeration of terms.</returns>
        public static IEnumerable<IndexTermToken> Tokenize(string input, CultureInfo culture, char[] wildcards = null)
        {
            var currentToken = new StringBuilder();
            var position = (uint)0;
            var except = new HashSet<char>(Delimiters.Except(wildcards ?? []));
            var isString = false;
            var isNumber = false;
            var hasDecimal = false;
            var hasExponent = false;

            if (input == null || input.Length == 0)
            {
                yield break;
            }

            for (int i = 0; i < input.Length; i++)
            {
                var current = input[i];

                if
                (
                    !isString &&
                    (
                        char.IsDigit(current) ||
                        (
                            isNumber &&
                            (
                                current == '.' && !hasDecimal ||
                                current == ',' ||
                                current == 'e' ||
                                current == 'E'
                            )
                        ) ||
                        (
                            current == '-' &&
                            (
                                currentToken.Length == 0 ||
                                hasExponent
                            ) &&
                            char.IsDigit((i + 1 < input.Length) ? input[i + 1] : (char)0)
                        )
                    )
                )
                {
                    // begin a new number token
                    if (!isNumber)
                    {
                        if (currentToken.Length > 0)
                        {
                            yield return new IndexTermToken()
                            {
                                Position = position,
                                Value = Convert(currentToken, false, culture)
                            };
                            currentToken.Clear();
                        }
                        isNumber = true;
                        hasDecimal = (current == '.');
                        hasExponent = (current == 'e' || current == 'E');
                    }
                    else if (current == '.')
                    {
                        if (!hasDecimal)
                        {
                            hasDecimal = true;
                        }
                        else
                        {
                            // handle error for multiple decimals
                        }
                    }
                    else if (current == 'e' || current == 'E')
                    {
                        if (!hasExponent)
                        {
                            hasExponent = true;
                        }
                        else
                        {
                            // Handle error for multiple exponents
                        }
                    }
                    currentToken.Append(current);
                }
                else
                {
                    // End the current number token
                    if (isNumber)
                    {
                        yield return new IndexTermToken()
                        {
                            Position = position,
                            Value = Convert(currentToken, true, culture)
                        };
                        currentToken.Clear();
                        isNumber = false;
                        hasDecimal = false;
                        hasExponent = false;
                        position++;
                    }

                    if (char.IsWhiteSpace(current) || except.Contains(current))
                    {
                        if (currentToken.Length > 0)
                        {
                            yield return new IndexTermToken()
                            {
                                Position = position,
                                Value = Convert(currentToken, false, culture)
                            };
                            currentToken.Clear();
                            isString = false;
                        }
                        position++;
                    }
                    else
                    {
                        currentToken.Append(current);
                        isString = true;
                    }
                }
            }

            if (currentToken.Length > 0)
            {
                yield return new IndexTermToken()
                {
                    Position = position,
                    Value = Convert(currentToken, isNumber, culture)
                };
            }
        }

        /// <summary>
        /// Converts the content to its appropriate data type.
        /// </summary>
        /// <param name="sb">The object containing the string to convert.</param>
        /// <param name="isNumber">A flag indicating whether the string represents a numeric value.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>
        /// Returns a double if the string represents a numeric value and conversion is successful; otherwise, returns the string itself.
        /// </returns>
        private static object Convert(StringBuilder sb, bool isNumber, CultureInfo culture)
        {
            var res = sb.ToString();

            if (isNumber)
            {
                if (double.TryParse(res, NumberStyles.Any, culture, out double number))
                {
                    return number;
                }
                else if (double.TryParse(res, NumberStyles.Any, CultureInfo.InvariantCulture, out double number1))
                {
                    return number1;
                }
            }

            return res;
        }
    }
}
