using System.Collections.Generic;
using System.Globalization;

namespace WebExpress.WebIndex.Term.Pipeline
{
    /// <summary>
    /// Removes unnecessary characters from the beginning and end of a term.
    /// </summary>
    public class IndexPipeStageConverterTrim : IIndexPipeStage
    {
        /// <summary>
        /// The characters that can be removed.
        /// </summary>
        private static readonly char[] trimCharacters = ['.', ',', '-', '_', '…'];

        /// <summary>
        /// Returns the name of the process state.
        /// </summary>
        public string Name => "Trim";

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="context">The reference to the context.</param>
        public IndexPipeStageConverterTrim(IIndexContext context)
        {
        }

        /// <summary>
        /// Converts specific elements on the term enumeration in lower case.
        /// </summary>
        /// <param name="input">The terms.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>The trimmed terms.</returns>
        public IEnumerable<IndexTermToken> Process(IEnumerable<IndexTermToken> input, CultureInfo culture)
        {
            foreach (var token in input)
            {
                if (token.Value is string)
                {
                    yield return new IndexTermToken()
                    {
                        Value = token.Value.ToString().Trim(trimCharacters),
                        Position = token.Position
                    };
                }
                else
                {
                    yield return token;
                }
            }
        }
    }
}
