﻿using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace WebExpress.WebIndex.Term.Converter
{
    /// <summary>
    /// Conversion to singular.
    /// </summary>
    public static class WebIndexTermConverterSingular
    {
        /// <summary>
        /// Returns a list of the irregular words.
        /// </summary>
        internal static Dictionary<CultureInfo, Dictionary<string, string>> IrregularWordDictionary { get; } = [];

        /// <summary>
        /// Returns a list of the regular words.
        /// </summary>
        internal static Dictionary<CultureInfo, Dictionary<string, string>> RegularWordDictionary { get; } = [];

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="context">The reference to the context.</param>
        public static void Initialization(IWebIndexContext context)
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
        private static void FillIrregularWordDictionary(IWebIndexContext context, CultureInfo culture)
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
        private static void FillRegularWordDictionary(IWebIndexContext context, CultureInfo culture)
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
        public static IEnumerable<WebIndexTermToken> Singular(this IEnumerable<WebIndexTermToken> input, CultureInfo culture)
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
                if (dict.ContainsKey(token.Value))
                {
                    token.Value = dict[token.Value];

                    yield return token;
                }
                else
                {
                    var treated = false;

                    foreach (var keyValue in RegularWordDictionary[supportedCulture])
                    {
                        if (Regex.Match(token.Value, keyValue.Key).Success)
                        {
                            token.Value = Regex.Replace(token.Value, keyValue.Key, keyValue.Value);

                            treated = true;

                            yield return token;
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
        private static CultureInfo GetSupportedCulture(CultureInfo culture)
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