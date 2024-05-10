using System.Globalization;
using WebExpress.WebIndex.Term;
using WebExpress.WebIndex.Term.Pipeline;
using WebExpress.WebIndex.Test.Fixture;
using Xunit.Abstractions;

namespace WebExpress.WebIndex.Test.Token
{
    public class UnitTestIndexPipeStageNormalizer : IClassFixture<UnitTestIndexFixtureToken>
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
        public UnitTestIndexPipeStageNormalizer(UnitTestIndexFixtureToken fixture, ITestOutputHelper output)
        {
            Fixture = fixture;
            Output = output;
        }

        /// <summary>
        /// Tests the Normalize method. This function is part of the stemming process and normalize terms.
        /// </summary>
        [Fact]
        public void Normalize()
        {
            var culture = CultureInfo.GetCultureInfo("en");
            var pipeStage = new IndexPipeStageConverterNormalizer(Fixture.Context);

            (string, string)[] words =
            [
                ("résumé", "resume"),
                ("Mëtàl", "Metal"),
                ("élégant", "elegant"),
                ("cliché", "cliche"),
                ("naïve", "naive"),
                ("soufflé", "souffle"),
                ("déjà-vu", "deja-vu"),
                ("tête-à-tête", "tete-a-tete"),
                ("São-Paulo", "Sao-Paulo"),
                ("Björk", "Bjork")
            ];

            var tokenizer = new IndexTermTokenizer();
            var res = pipeStage.Process(tokenizer.Tokenize(string.Join(" ",  words.Select(x => x.Item1))), culture)
                .Select(x => x.Value)
                .ToList();

            Assert.True(res.Intersect(words.Select(x => x.Item2)).Count() == res.Count);
        }
    }
}
