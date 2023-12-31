using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace WebExpress.WebIndex.Term.Filter
{
    /// <summary>
    /// Removes stop words
    /// </summary>
    public static class WebIndexTermFilterStopWord
    {
        /// <summary>
        /// Returns a list of the stop words that are used for filtering.
        /// </summary>
        internal static Dictionary<CultureInfo, HashSet<string>> StopWordDictionary { get; } = [];

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="context">The reference to the context.</param>
        public static void Initialization(IWebIndexContext context)
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
        private static void FillStopWordDictionary(IWebIndexContext context, CultureInfo culture)
        {
            var fileContent = File.ReadAllLines
            (
                Path.Combine(context.IndexDirectory,
                $"stopwords.{culture.TwoLetterISOLanguageName}")
            );

            if (!StopWordDictionary.ContainsKey(culture))
            {
                StopWordDictionary.Add(culture, new HashSet<string>());
            }

            foreach (var word in fileContent.Select(x => x.Trim()).Where(x => !x.StartsWith('#')))
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
        public static IEnumerable<WebIndexTermToken> StopWord(this IEnumerable<WebIndexTermToken> input, CultureInfo culture)
        {
            var supportedCulture = GetSupportedCulture(culture);

            if (!StopWordDictionary.ContainsKey(supportedCulture))
            {
                foreach (var token in input)
                {
                    yield return token;
                }
            }

            foreach (var token in input)
            {
                if (!StopWordDictionary[supportedCulture].Contains(token.Value))
                {
                    yield return token;
                }
            }
        }

        /// <summary>
        /// Transforms a given culture into a supported culture.
        /// </summary>
        /// <param name="culture">The culture to be used.</param>
        /// <returns>A supported culture, this may differ from the desired culture.</returns>
        private static CultureInfo GetSupportedCulture(CultureInfo culture)
        {
            if (StopWordDictionary.ContainsKey(culture))
            {
                return culture;
            }

            var generalCulture = CultureInfo.GetCultureInfo(culture.TwoLetterISOLanguageName);

            if (StopWordDictionary.ContainsKey(generalCulture))
            {
                return generalCulture;
            }

            return CultureInfo.GetCultureInfo("en");
        }
    }
}
