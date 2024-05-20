using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace WebExpress.WebIndex.Term.Pipeline
{
    /// <summary>
    /// Conversion of synonyms.
    /// </summary>
    public class IndexPipeStageConverterSynonym : IIndexPipeStage
    {
        /// <summary>
        /// Returns the name of the process state.
        /// </summary>
        public string Name => "Synonym";

        /// <summary>
        /// Returns a list of the synonyms that are used for filtering.
        /// </summary>
        internal Dictionary<CultureInfo, Dictionary<string, string>> SynonymDictionary { get; } = [];

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="context">The reference to the context.</param>
        public IndexPipeStageConverterSynonym(IIndexContext context)
        {
            foreach (var file in Directory.GetFiles(context.IndexDirectory, "synonyms.*"))
            {
                var extension = Path.GetExtension(file).Trim('.');

                FillSynonymDictionary(context, CultureInfo.GetCultureInfo(extension));
            }
        }

        /// <summary>
        /// Converts specific elements on the term enumeration.
        /// </summary>
        /// <param name="input">The terms.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>The terms at which the synonyms have been converted.</returns>
        public IEnumerable<IndexTermToken> Process(IEnumerable<IndexTermToken> input, CultureInfo culture)
        {
            var supportedCulture = GetSupportedCulture(culture);

            if (!SynonymDictionary.ContainsKey(supportedCulture))
            {
                foreach (var token in input)
                {
                    yield return token;
                }
            }

            foreach (var token in input)
            {
                if (SynonymDictionary[supportedCulture].TryGetValue(token.Value, out string value))
                {
                    yield return new IndexTermToken()
                    {
                        Value = value,
                        Position = token.Position
                    };
                }
                else
                {
                    yield return token;
                }
            }
        }

        /// <summary>
        /// Fills the directory with synonyms.
        /// </summary>
        /// <param name="context">The reference to the context.</param>
        /// <param name="culture">The culture.</param>
        private void FillSynonymDictionary(IIndexContext context, CultureInfo culture)
        {
            var fileContent = File.ReadAllLines
            (
                Path.Combine(context.IndexDirectory,
                $"synonyms.{culture.TwoLetterISOLanguageName}")
            );

            if (!SynonymDictionary.ContainsKey(culture))
            {
                SynonymDictionary.Add(culture, new Dictionary<string, string>());
            }

            var dict = SynonymDictionary[culture];

            foreach (var line in fileContent.Select(x => x.Trim()).Where(x => !x.StartsWith('#')))
            {
                var l = line.Normalize(System.Text.NormalizationForm.FormKD).ToLower();
                var split = l.Split('=', System.StringSplitOptions.RemoveEmptyEntries | System.StringSplitOptions.TrimEntries);
                var key = split[0];
                var value = split[1]?.Split('#')[0]?.TrimEnd();

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
            if (SynonymDictionary.ContainsKey(culture))
            {
                return culture;
            }

            var generalCulture = CultureInfo.GetCultureInfo(culture.TwoLetterISOLanguageName);

            if (SynonymDictionary.ContainsKey(generalCulture))
            {
                return generalCulture;
            }

            return CultureInfo.GetCultureInfo("en");
        }
    }
}
