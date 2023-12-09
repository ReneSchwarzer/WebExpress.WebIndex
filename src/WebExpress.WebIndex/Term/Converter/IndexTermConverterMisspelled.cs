using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace WebExpress.WebIndex.Term.Converter
{
    /// <summary>
    /// Conversion of misspelled words.
    /// </summary>
    public static class IndexTermConverterMisspelled
    {
        /// <summary>
        /// Returns a list of the misspelled words.
        /// </summary>
        internal static Dictionary<CultureInfo, Dictionary<string, string>> MisspelledWordDictionary { get; } = [];

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="context">The reference to the context.</param>
        public static void Initialization(IIndexContext context)
        {
            foreach (var file in Directory.GetFiles(context.IndexDirectory, "misspelledwords.*"))
            {
                var extension = Path.GetExtension(file).Trim('.');

                FillMisspelledWordDictionary(context, CultureInfo.GetCultureInfo(extension));
            }
        }

        /// <summary>
        /// Fills the directory with misspelled words.
        /// </summary>
        /// <param name="context">The reference to the context.</param>
        /// <param name="culture">The culture.</param>
        private static void FillMisspelledWordDictionary(IIndexContext context, CultureInfo culture)
        {
            var fileContent = File.ReadAllLines
            (
                Path.Combine(context.IndexDirectory,
                $"misspelledwords.{culture.TwoLetterISOLanguageName}")
            );

            if (!MisspelledWordDictionary.ContainsKey(culture))
            {
                MisspelledWordDictionary.Add(culture, new Dictionary<string, string>());
            }

            var dict = MisspelledWordDictionary[culture];

            foreach (var line in fileContent.Select(x => x.Trim()).Where(x => !x.StartsWith('#')))
            {
                var split = line.Split('=', System.StringSplitOptions.RemoveEmptyEntries | System.StringSplitOptions.TrimEntries);
                var key = split[0]?.ToLower();
                var value = split[1]?.ToLower()?.Split('#')[0]?.TrimEnd();

                if (!string.IsNullOrWhiteSpace(key) && !dict.ContainsKey(key))
                {
                    dict.Add(key, value);
                }
            }
        }

        /// <summary>
        /// Converts specific elements on the term enumeration.
        /// </summary>
        /// <param name="input">The terms.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>The terms at which the misspelled words have been converted.</returns>
        public static IEnumerable<IndexTermToken> Misspelled(this IEnumerable<IndexTermToken> input, CultureInfo culture)
        {
            if (!MisspelledWordDictionary.ContainsKey(culture))
            {
                foreach (var token in input)
                {
                    yield return token;
                }
            }

            foreach (var token in input)
            {
                if (MisspelledWordDictionary[culture].ContainsKey(token.Value))
                {
                    token.Value = MisspelledWordDictionary[culture][token.Value];
                    yield return token;
                }
                else
                {
                    yield return token;
                }
            }
        }
    }
}
