using System.Globalization;
using WebExpress.WebIndex.Term;
using WebExpress.WebIndex.Term.Pipeline;
using WebExpress.WebIndex.Test.Fixture;
using Xunit.Abstractions;

namespace WebExpress.WebIndex.Test.Token
{
    public class UnitTestIndexPipeStageEmpty : IClassFixture<UnitTestIndexFixtureToken>
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
        public UnitTestIndexPipeStageEmpty(UnitTestIndexFixtureToken fixture, ITestOutputHelper output)
        {
            Fixture = fixture;
            Output = output;
        }

        /// <summary>
        /// Tests the empty method. This function is part of the stemming process and removes empty tokens.
        /// </summary>
        [Fact]
        public void Empty()
        {
            var culture = CultureInfo.GetCultureInfo("en");
            var pipeStage = new IndexPipeStageFilterEmpty(Fixture.Context);

            var tokenizer = new IndexTermTokenizer();
            var token = tokenizer.Tokenize("Dogs are known as man's best friend.");

            var res = pipeStage.Process(token, culture)
                .Select(x => x.Value)
                .ToList();

            Assert.True(token.Count() == res.Count);
        }
    }
}
