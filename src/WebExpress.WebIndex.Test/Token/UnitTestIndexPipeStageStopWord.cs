using System.Globalization;
using WebExpress.WebIndex.Term;
using WebExpress.WebIndex.Term.Pipeline;
using WebExpress.WebIndex.Test.Fixture;
using Xunit.Abstractions;

namespace WebExpress.WebIndex.Test.Token
{
    public class UnitTestIndexPipeStageStopWord : IClassFixture<UnitTestIndexFixtureToken>
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
        public UnitTestIndexPipeStageStopWord(UnitTestIndexFixtureToken fixture, ITestOutputHelper output)
        {
            Fixture = fixture;
            Output = output;
        }

        /// <summary>
        /// Tests the stop word method. This function is part of the stemming process and removes stop words.
        /// </summary>
        [Fact]
        public void StopWord_En()
        {
            var culture = CultureInfo.GetCultureInfo("en");
            var pipeStage = new IndexPipeStageFilterStopWord(Fixture.Context);

            var token = IndexTermTokenizer.Tokenize("May the force be with you.");

            var res = pipeStage.Process(token, culture)
                .Select(x => x.Value)
                .ToList();

            Assert.DoesNotContain("the", res);
            Assert.DoesNotContain("be", res);
            Assert.DoesNotContain("with", res);

            Assert.True(token.Count() - 3 == res.Count);
        }

        /// <summary>
        /// Tests the stop word method. This function is part of the stemming process and removes stop words.
        /// </summary>
        [Fact]
        public void StopWord_De()
        {
            var culture = CultureInfo.GetCultureInfo("de");
            var pipeStage = new IndexPipeStageFilterStopWord(Fixture.Context);

            var token = IndexTermTokenizer.Tokenize("Als Gregor Samsa eines Morgens aus unruhigen Träumen erwachte, fand er sich in seinem Bett zu einem ungeheueren Ungeziefer verwandelt.".ToLower());

            var res = pipeStage.Process(token, culture)
                .Select(x => x.Value)
                .ToList();

            Assert.DoesNotContain("als", res);
            Assert.DoesNotContain("eines", res);
            Assert.DoesNotContain("aus", res);
            Assert.DoesNotContain("er", res);
            Assert.DoesNotContain("sich", res);
            Assert.DoesNotContain("in", res);
            Assert.DoesNotContain("seinem", res);
            Assert.DoesNotContain("zu", res);
            Assert.DoesNotContain("einem", res);

            Assert.True(token.Count() - 9 == res.Count);
        }
    }
}
