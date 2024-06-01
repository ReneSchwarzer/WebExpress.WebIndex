using System.Collections.Generic;
using System.Globalization;
using WebExpress.WebIndex.Utility;

namespace WebExpress.WebIndex.Term.Pipeline
{
    /// <summary>
    /// Removes emty words.
    /// </summary>
    public class IndexPipeStageFilterEmpty : IIndexPipeStage
    {
        /// <summary>
        /// Returns the name of the process state.
        /// </summary>
        public string Name => "Empty";

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="context">The reference to the context.</param>
        public IndexPipeStageFilterEmpty(IIndexContext context)
        {
        }

        /// <summary>
        /// Filters specific elements on the term enumeration.
        /// </summary>
        /// <param name="input">The terms.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>The filtered term enumeration.</returns>
        public IEnumerable<IndexTermToken> Process(IEnumerable<IndexTermToken> input, CultureInfo culture)
        {
#if DEBUG
            using var profiling = Profiling.Diagnostic();
#endif

            foreach (var token in input)
            {
                if (token.Value is string)
                {
                    if (!string.IsNullOrWhiteSpace(token.Value.ToString()))
                    {
                        yield return token;
                    }
                }
                else
                {
                    yield return token;
                }
            }
        }
    }
}
