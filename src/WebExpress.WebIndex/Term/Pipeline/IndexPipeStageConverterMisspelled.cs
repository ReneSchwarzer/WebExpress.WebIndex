using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace WebExpress.WebIndex.Term.Pipeline
{
    /// <summary>
    /// Conversion of misspelled words.
    /// </summary>
    public class IndexPipeStageConverterMisspelled : IIndexPipeStage
    {
        /// <summary>
        /// Returns the name of the process state.
        /// </summary>
        public string Name => "Misspelled";

        /// <summary>
        /// Returns a list of the misspelled words.
        /// </summary>
        internal Dictionary<CultureInfo, Dictionary<string, string>> MisspelledWordDictionary { get; } = [];

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">The reference to the context.</param>
        public IndexPipeStageConverterMisspelled(IIndexContext context)
        {
            foreach (var file in Directory.GetFiles(context.IndexDirectory, "misspelledwords.*"))
            {
                var extension = Path.GetExtension(file).Trim('.');

                FillMisspelledWordDictionary(context, CultureInfo.GetCultureInfo(extension));
            }
        }

        /// <summary>
        /// Converts specific elements on the term enumeration.
        /// </summary>
        /// <param name="input">The terms.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>The terms at which the misspelled words have been converted.</returns>
        public IEnumerable<IndexTermToken> Process(IEnumerable<IndexTermToken> input, CultureInfo culture)
        {
            var supportedCulture = GetSupportedCulture(culture);

            if (!MisspelledWordDictionary.ContainsKey(supportedCulture))
            {
                foreach (var token in input)
                {
                    yield return token;
                }
            }

            foreach (var token in input)
            {
                if (token.Value is string)
                {
                    if (MisspelledWordDictionary[supportedCulture].ContainsKey(token.Value.ToString()))
                    {
                        yield return new IndexTermToken()
                        {
                            Value = MisspelledWordDictionary[supportedCulture][token.Value.ToString()],
                            Position = token.Position
                        };
                    }
                    else
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

        /// <summary>
        /// Fills the directory with misspelled words.
        /// </summary>
        /// <param name="context">The reference to the context.</param>
        /// <param name="culture">The culture.</param>
        private void FillMisspelledWordDictionary(IIndexContext context, CultureInfo culture)
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
                var l = line.Normalize(System.Text.NormalizationForm.FormKD).ToLower();
                var split = l.Split('=', System.StringSplitOptions.RemoveEmptyEntries | System.StringSplitOptions.TrimEntries);
                var key = split[0]?.ToLower();
                var value = split[1]?.ToLower()?.Split('#')[0]?.TrimEnd();

                if (!string.IsNullOrWhiteSpace(key) && !dict.ContainsKey(key))
                {
                    dict.Add(key, value);
                }
            }
        }

        /// <summary>
        /// Transforms a given culture into a supported culture.
        /// </summary>
        /// <param name="culture">The culture to be used.</param>
        /// <returns>A supported culture, this may differ from the desired culture.</returns>
        private CultureInfo GetSupportedCulture(CultureInfo culture)
        {
            if (MisspelledWordDictionary.ContainsKey(culture))
            {
                return culture;
            }

            var generalCulture = CultureInfo.GetCultureInfo(culture.TwoLetterISOLanguageName);

            if (MisspelledWordDictionary.ContainsKey(generalCulture))
            {
                return generalCulture;
            }

            return CultureInfo.GetCultureInfo("en");
        }
    }
}
