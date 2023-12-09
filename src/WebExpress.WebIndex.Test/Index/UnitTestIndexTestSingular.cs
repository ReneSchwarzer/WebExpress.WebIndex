using System.Globalization;
using WebExpress.WebIndex.Term;
using WebExpress.WebIndex.Term.Converter;
using Xunit.Abstractions;

namespace WebExpress.WebIndex.Test.Index
{
    [Collection("UnitTestIndexCollectionFixture")]
    public class UnitTestIndexTestSingular
    {
        public ITestOutputHelper Output { get; private set; }
        protected UnitTestIndexFixture Fixture { get; set; }

        public UnitTestIndexTestSingular(UnitTestIndexFixture fixture, ITestOutputHelper output)
        {
            Fixture = fixture;
            Output = output;

            var context = new IndexContext();

            IndexTermConverterLowerCase.Initialization(context);
            IndexTermConverterSingular.Initialization(context);
        }
        [Fact]
        public void PluralToSingularEn()
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

            Assert.True(res.Intersect(singularWords).Count() == res.Count());
        }

        [Fact]
        public void PluralToSingularDe()
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

            Assert.True(res.Intersect(singularWords).Count() == res.Count());
        }
    }
}
