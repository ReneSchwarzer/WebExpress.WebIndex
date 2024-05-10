using System.Collections.Generic;
using System.Globalization;

namespace WebExpress.WebIndex.Term.Pipeline
{
    /// <summary>
    /// This interface defines the necessary methods for stemming, lemmatization, and stop word filtering.
    /// </summary>
    public interface IIndexPipeStage
    {
        /// <summary>
        /// Returns the name of the process state.
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// The process step with the processing of the terms.
        /// </summary>
        /// <param name="input">The terms.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>The processed terms.</returns>
        IEnumerable<IndexTermToken> Process(IEnumerable<IndexTermToken> input, CultureInfo culture);
    }
}