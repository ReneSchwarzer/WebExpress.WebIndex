using System.Collections.Generic;
using System.Globalization;

namespace WebExpress.WebIndex.Term.Pipeline
{
    /// <summary>
    /// Conversion to lowercase.
    /// </summary>
    public class IndexPipeStageConverterLowerCase : IIndexPipeStage
    {
        /// <summary>
        /// Returns the name of the process state.
        /// </summary>
        public string Name => "LowerCase";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">The reference to the context.</param>
        public IndexPipeStageConverterLowerCase(IIndexContext context)
        {
        }

        /// <summary>
        /// Converts specific elements on the term enumeration in lower case.
        /// </summary>
        /// <param name="input">The terms.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>The terms in lower case.</returns>
        public IEnumerable<IndexTermToken> Process(IEnumerable<IndexTermToken> input, CultureInfo culture)
        {
            foreach (var token in input)
            {
                token.Value = token.Value.ToLowerInvariant();
                yield return token;
            }
        }
    }
}
