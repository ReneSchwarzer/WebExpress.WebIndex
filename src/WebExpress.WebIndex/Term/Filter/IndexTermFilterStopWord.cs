using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace WebExpress.WebIndex.Term.Filter
{
    /// <summary>
    /// Removes stop words
    /// </summary>
    public static class IndexTermFilterStopWord
    {
        /// <summary>
        /// Returns a list of the stop words that are used for filtering.
        /// </summary>
        internal static Dictionary<CultureInfo, HashSet<string>> StopWordDictionary { get; } = [];

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="context">The reference to the context.</param>
        public static void Initialization(IIndexContext context)
        {
            foreach (var file in Directory.GetFiles(context.IndexDirectory, "stopwords.*"))
            {
                var extension = Path.GetExtension(file).Trim('.');

                FillStopWordDictionary(context, CultureInfo.GetCultureInfo(extension));
            }
        }

        /// <summary>
        /// Fills the directory with stop words.
        /// </summary>
        /// <param name="context">The reference to the context.</param>
        /// <param name="culture">The culture.</param>
        private static void FillStopWordDictionary(IIndexContext context, CultureInfo culture)
        {
            var fileContent = File.ReadAllText
            (
                Path.Combine(context.IndexDirectory,
                $"stopwords.{culture.TwoLetterISOLanguageName}")
            );

            if (!StopWordDictionary.ContainsKey(culture))
            {
                StopWordDictionary.Add(culture, new HashSet<string>());
            }

            foreach (var word in fileContent.Split(',', System.StringSplitOptions.RemoveEmptyEntries | System.StringSplitOptions.TrimEntries))
            {
                if (!StopWordDictionary[culture].Contains(word))
                {
                    StopWordDictionary[culture].Add(word.ToLower());
                }
            }
        }

        /// <summary>
        /// Filters specific elements on the term enumeration.
        /// </summary>
        /// <param name="input">The terms.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>The filtered term enumeration.</returns>
        public static IEnumerable<IndexTermToken> StopWord(this IEnumerable<IndexTermToken> input, CultureInfo culture)
        {
            if (!StopWordDictionary.ContainsKey(culture))
            {
                foreach (var token in input)
                {
                    yield return token;
                }
            }

            foreach (var token in input)
            {
                if (!StopWordDictionary[culture].Contains(token.Value))
                {
                    yield return token;
                }
            }
        }
    }
}
