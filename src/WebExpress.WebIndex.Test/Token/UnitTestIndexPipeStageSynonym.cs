using System.Globalization;
using WebExpress.WebIndex.Term;
using WebExpress.WebIndex.Term.Pipeline;
using WebExpress.WebIndex.Test.Fixture;
using Xunit.Abstractions;

namespace WebExpress.WebIndex.Test.Token
{
    public class UnitTestIndexPipeStageSynonym : IClassFixture<UnitTestIndexFixtureToken>
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
        public UnitTestIndexPipeStageSynonym(UnitTestIndexFixtureToken fixture, ITestOutputHelper output)
        {
            Fixture = fixture;
            Output = output;
        }

        /// <summary>
        /// Tests the synonym method. This function is part of the lemmatization process and reduced sysnonyms.
        /// </summary>
        [Fact]
        public void Synonym_En()
        {
            var culture = CultureInfo.GetCultureInfo("en");
            var pipeStage = new IndexPipeStageConverterSynonym(Fixture.Context);

            (string, string)[] words =
            [
                ("joyful", "happy")
            ];

            var res = pipeStage.Process(IndexTermTokenizer.Tokenize(string.Join(" ", words.Select(x => x.Item1)), culture), culture)
                .Select(x => x.Value)
                .ToList();

            Assert.Equal(words.Select(x => x.Item2), res);
        }

        /// <summary>
        /// Tests the synonym method. This function is part of the lemmatization process and reduced sysnonyms.
        /// </summary>
        [Fact]
        public void Synonym_De()
        {
            var culture = CultureInfo.GetCultureInfo("de");
            var pipeStage = new IndexPipeStageConverterSynonym(Fixture.Context);

            (string, string)[] words =
            [
                ("kfz", "auto")
            ];

            var res = pipeStage.Process(IndexTermTokenizer.Tokenize(string.Join(" ", words.Select(x => x.Item1)), culture), culture)
                .Select(x => x.Value)
                .ToList();

            Assert.Equal(words.Select(x => x.Item2), res);
        }
    }
}
