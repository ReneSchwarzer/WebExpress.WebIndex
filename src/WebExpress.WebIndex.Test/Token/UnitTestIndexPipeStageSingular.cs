using System.Globalization;
using WebExpress.WebIndex.Term;
using WebExpress.WebIndex.Term.Pipeline;
using WebExpress.WebIndex.Test.Fixture;
using Xunit.Abstractions;

namespace WebExpress.WebIndex.Test.Token
{
    public class UnitTestIndexPipeStageSingular : IClassFixture<UnitTestIndexFixtureToken>
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
        public UnitTestIndexPipeStageSingular(UnitTestIndexFixtureToken fixture, ITestOutputHelper output)
        {
            Fixture = fixture;
            Output = output;
        }

        /// <summary>
        /// Tests the singular method. This function is part of the lemmatization process and the transformation of the token into the singular.
        /// </summary>
        [Fact]
        public void PluralToSingular_En()
        {
            var culture = CultureInfo.GetCultureInfo("en");
            var pipeStage = new IndexPipeStageConverterSingular(Fixture.Context);

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

            var res = pipeStage.Process(IndexTermTokenizer.Tokenize(string.Join(" ", pluralWords), culture), culture)
                .Select(x => x.Value)
                .ToList();

            Assert.True(res.Intersect(singularWords).Count() == res.Count);
        }

        /// <summary>
        /// Tests the singular method. This function is part of the lemmatization process and the transformation of the token into the singular.
        /// </summary>
        [Fact]
        public void PluralToSingular_De()
        {
            var culture = CultureInfo.GetCultureInfo("de");
            var pipeStage = new IndexPipeStageConverterSingular(Fixture.Context);

            (string, string)[] words =
            [
                // regular nouns (en, er, e, s)
                ("autos", "auto"),
                ("frauen", "frau"),
                ("kinder", "kind"),
                ("tische", "tisch"),
                // irregular nouns
                ("atlanten", "atlas"),
                ("bücher", "buch"),
                ("männer", "mann"),
                ("stühle", "stuhl")
            ];


            var pluralWords = words.Select(x => x.Item1);
            var singularWords = words.Select(x => x.Item2);

            var res = pipeStage.Process(IndexTermTokenizer.Tokenize(string.Join(" ", pluralWords), culture), culture)
                .Select(x => x.Value)
                .ToList();

            Assert.True(res.Intersect(singularWords).Count() == res.Count);
        }
    }
}
