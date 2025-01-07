using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace WebExpress.WebIndex.Term.Pipeline
{
    /// <summary>
    /// Removes surrogate characters.
    /// </summary>
    public class IndexPipeStageFilterSurrogateCharacter : IIndexPipeStage
    {
        /// <summary>
        /// Returns the name of the process state.
        /// </summary>
        public string Name => "SurrogateCharacter";

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="context">The reference to the context.</param>
        public IndexPipeStageFilterSurrogateCharacter(IIndexContext context)
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
            foreach (var token in input)
            {
                if (token.Value is string)
                {
                    if (!token.Value.ToString().Where(x => char.IsSurrogate(x)).Any())
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
