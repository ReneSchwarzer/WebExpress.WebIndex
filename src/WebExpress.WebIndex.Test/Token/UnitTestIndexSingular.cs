using System.Globalization;
using WebExpress.WebIndex.Term;
using WebExpress.WebIndex.Term.Converter;
using WebExpress.WebIndex.Test.Fixture;
using Xunit.Abstractions;

namespace WebExpress.WebIndex.Test.Token
{
    public class UnitTestIndexSingular : IClassFixture<UnitTestIndexFixtureToken>
    {
        /// <summary>
        /// Returns the log.
        /// </summary>
        public ITestOutputHelper Output { get; private set; }

        /// <summary>
        /// Returns the test context.
        /// </summary>
        protected UnitTestIndexFixtureToken Fixture { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fixture">The test context.</param>
        /// <param name="output">The log.</param>
        public UnitTestIndexSingular(UnitTestIndexFixtureToken fixture, ITestOutputHelper output)
        {
            Fixture = fixture;
            Output = output;

            var context = new IndexContext();

            IndexTermConverterLowerCase.Initialization(context);
            IndexTermConverterSingular.Initialization(context);
        }

        [Fact]
        public void PluralToSingular_En()
        {
            var culture = CultureInfo.GetCultureInfo("en");

            (string, string)[] words =
            [
                // regular nouns (ies, ses  xes, s)
                ("babies", "baby"),
                ("cities", "city"),
                ("countries", "country"),
                ("families", "family"),
                ("parties", "party"),
                ("pennies", "penny"),
                ("studies", "study"),
                ("stories", "story"),
                // irregular nouns
                ("axes", "axis"),
                ("indices", "index"),
                ("selves", "self"),
                ("vortexes", "vortex")
            ];


            var pluralWords = words.Select(x => x.Item1);
            var singularWords = words.Select(x => x.Item2);

            var tokenizer = new IndexTermTokenizer();

            var res = tokenizer
                .Tokenize(string.Join(" ", pluralWords))
                .Singular(culture)
                .Select(x => x.Value)
                .ToList();

            Assert.True(res.Intersect(singularWords).Count() == res.Count);
        }

        [Fact]
        public void PluralToSingular_De()
        {
            var culture = CultureInfo.GetCultureInfo("de");
            string[] pluralWords =
            [
                // regular nouns (en, er, e, s)
                "autos",
                "frauen",
                "kinder",
                "tische",
                // irregular nouns
                "atlanten",
                "bücher",
                "männer",
                "stühle"
            ];

            string[] singularWords =
            [
                // regular nouns (en, er, e, s)
                "auto",
                "frau",
                "kind",
                "tisch",
                // irregular nouns
                "atlas",
                "buch",
                "mann",
                "stuhl"
            ];

            var tokenizer = new IndexTermTokenizer();

            var res = tokenizer
                .Tokenize(string.Join(" ", pluralWords))
                .Singular(culture)
                .Select(x => x.Value)
                .ToList();

            Assert.True(res.Intersect(singularWords).Count() == res.Count);
        }
    }
}
