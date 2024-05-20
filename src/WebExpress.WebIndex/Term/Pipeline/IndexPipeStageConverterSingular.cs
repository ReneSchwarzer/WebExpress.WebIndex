using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace WebExpress.WebIndex.Term.Pipeline
{
    /// <summary>
    /// Conversion to singular.
    /// </summary>
    public class IndexPipeStageConverterSingular : IIndexPipeStage
    {
        /// <summary>
        /// Returns the name of the process state.
        /// </summary>
        public string Name => "Singular";

        /// <summary>
        /// Returns a list of the irregular words.
        /// </summary>
        internal Dictionary<CultureInfo, Dictionary<string, string>> IrregularWordDictionary { get; } = [];

        /// <summary>
        /// Returns a list of the regular words.
        /// </summary>
        internal Dictionary<CultureInfo, Dictionary<string, string>> RegularWordDictionary { get; } = [];

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="context">The reference to the context.</param>
        public IndexPipeStageConverterSingular(IIndexContext context)
        {
            foreach (var file in Directory.GetFiles(context.IndexDirectory, "irregularwords.*"))
            {
                var extension = Path.GetExtension(file).Trim('.');

                FillIrregularWordDictionary(context, CultureInfo.GetCultureInfo(extension));
            }

            foreach (var file in Directory.GetFiles(context.IndexDirectory, "regularwords.*"))
            {
                var extension = Path.GetExtension(file).Trim('.');

                FillRegularWordDictionary(context, CultureInfo.GetCultureInfo(extension));
            }
        }

        /// <summary>
        /// Fills the directory with irregular words.
        /// </summary>
        /// <param name="context">The reference to the context.</param>
        /// <param name="culture">The culture.</param>
        private void FillIrregularWordDictionary(IIndexContext context, CultureInfo culture)
        {
            var fileContent = File.ReadAllLines
            (
                Path.Combine(context.IndexDirectory,
                $"irregularwords.{culture.TwoLetterISOLanguageName}")
            );

            if (!IrregularWordDictionary.ContainsKey(culture))
            {
                IrregularWordDictionary.Add(culture, new Dictionary<string, string>());
            }

            var dict = IrregularWordDictionary[culture];

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
        /// Fills the directory with irregular words.
        /// </summary>
        /// <param name="context">The reference to the context.</param>
        /// <param name="culture">The culture.</param>
        private void FillRegularWordDictionary(IIndexContext context, CultureInfo culture)
        {
            var fileContent = File.ReadAllLines
            (
                Path.Combine(context.IndexDirectory,
                $"regularwords.{culture.TwoLetterISOLanguageName}")
            );

            if (!RegularWordDictionary.ContainsKey(culture))
            {
                RegularWordDictionary.Add(culture, new Dictionary<string, string>());
            }

            var dict = RegularWordDictionary[culture];

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
        /// <returns>The terms where the words have been converted to singular.</returns>
        public IEnumerable<IndexTermToken> Process(IEnumerable<IndexTermToken> input, CultureInfo culture)
        {
            var supportedCulture = GetSupportedCulture(culture);

            if (!IrregularWordDictionary.ContainsKey(supportedCulture))
            {
                foreach (var token in input)
                {
                    yield return token;
                }
            }

            var dict = IrregularWordDictionary[supportedCulture];

            foreach (var token in input)
            {
                if (dict.TryGetValue(token.Value, out string value))
                {
                    yield return new IndexTermToken()
                    {
                        Value = value,
                        Position = token.Position
                    };
                }
                else
                {
                    var treated = false;

                    foreach (var keyValue in RegularWordDictionary[supportedCulture])
                    {
                        if (Regex.Match(token.Value, keyValue.Key).Success)
                        {
                            treated = true;

                            yield return new IndexTermToken()
                            {
                                Value = Regex.Replace(token.Value, keyValue.Key, keyValue.Value),
                                Position = token.Position
                            };

                            break;
                        }
                    }

                    if (!treated)
                    {
                        yield return token;
                    }
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
            if (RegularWordDictionary.ContainsKey(culture))
            {
                return culture;
            }

            var generalCulture = CultureInfo.GetCultureInfo(culture.TwoLetterISOLanguageName);

            if (RegularWordDictionary.ContainsKey(generalCulture))
            {
                return generalCulture;
            }

            return CultureInfo.GetCultureInfo("en");
        }
    }
}
