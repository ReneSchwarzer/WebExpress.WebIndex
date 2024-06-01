using System.Globalization;
using WebExpress.WebIndex.Term;
using WebExpress.WebIndex.Term.Pipeline;
using WebExpress.WebIndex.Test.Fixture;
using Xunit.Abstractions;

namespace WebExpress.WebIndex.Test.Token
{
    public class UnitTestIndexPipeStageLowerCase : IClassFixture<UnitTestIndexFixtureToken>
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
        public UnitTestIndexPipeStageLowerCase(UnitTestIndexFixtureToken fixture, ITestOutputHelper output)
        {
            Fixture = fixture;
            Output = output;
        }

        /// <summary>
        /// Tests the lower case method. This function is part of the stemming process and convert characters in lower case.
        /// </summary>
        [Fact]
        public void LowerCase()
        {
            var culture = CultureInfo.GetCultureInfo("en");
            var pipeStage = new IndexPipeStageConverterLowerCase(Fixture.Context);

            (string, string)[] words =
            [
                ("Babies", "babies"),
                ("Cities", "cities"),
                ("Countries", "countries"),
                ("Families", "families")
            ];

            var res = pipeStage.Process(IndexTermTokenizer.Tokenize(string.Join(" ", words.Select(x => x.Item1)), culture), culture)
                .Select(x => x.Value)
                .ToList();

            Assert.True(res.Intersect(words.Select(x => x.Item2)).Count() == res.Count);
        }
    }
}
