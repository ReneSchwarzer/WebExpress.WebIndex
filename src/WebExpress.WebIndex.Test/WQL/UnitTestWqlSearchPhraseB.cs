using WebExpress.WebIndex.Test.Fixture;
using Xunit.Abstractions;

namespace WebExpress.WebIndex.Test.WQL
{
    /// <summary>
    /// Phrase search (exact word sequence)
    /// </summary>
    public class UnitTestWqlSearchPhraseB(UnitTestIndexFixtureWqlB fixture, ITestOutputHelper output) : IClassFixture<UnitTestIndexFixtureWqlB>
    {
        /// <summary>
        /// Returns the log.
        /// </summary>
        public ITestOutputHelper Output { get; private set; } = output;

        /// <summary>
        /// Returns the test context.
        /// </summary>
        protected UnitTestIndexFixtureWqlB Fixture { get; set; } = fixture;

        /// <summary>
        /// Tests the parser.
        /// </summary>
        [Fact]
        public void ParseValidWql1()
        {
            // test execution
            var wql = Fixture.ExecuteWql("description='lorem ipsum'");
            Assert.False(wql.HasErrors);
        }

        /// <summary>
        /// Tests the parser.
        /// </summary>
        [Fact]
        public void ParseValidWql2()
        {
            // test execution
            var wql = Fixture.ExecuteWql("description=\"lorem ipsum\"");
            Assert.False(wql.HasErrors);
        }

        /// <summary>
        /// Tests the parser.
        /// </summary>
        [Fact]
        public void ParseValidWql3()
        {
            // test execution
            var wql = Fixture.ExecuteWql("description=lorem");
            Assert.False(wql.HasErrors);
        }

        /// <summary>
        /// Tests the parser.
        /// </summary>
        [Fact]
        public void ParseInvalidWql1()
        {
            // test execution
            var wql = Fixture.ExecuteWql("description=lorem ipsum");
            Assert.True(wql.HasErrors);
        }

        /// <summary>
        /// Tests the parser.
        /// </summary>
        [Fact]
        public void ParseInvalidWql2()
        {
            // test execution
            var wql = Fixture.ExecuteWql("description='lorem ipsum");
            Assert.True(wql.HasErrors);
        }

        /// <summary>
        /// Tests the parser.
        /// </summary>
        [Fact]
        public void ParseInvalidWql3()
        {
            // test execution
            var wql = Fixture.ExecuteWql("description='lorem ipsum\"");
            Assert.True(wql.HasErrors);
        }

        /// <summary>
        /// Tests the parser.
        /// </summary>
        [Fact]
        public void ParseInvalidWql4()
        {
            // test execution
            var wql = Fixture.ExecuteWql("='lorem ipsum'");
            Assert.True(wql.HasErrors);
        }

        /// <summary>
        /// Tests phrase search, which retrieves content from documents that contain a specific order and combination of words defined by the phrase.
        /// </summary>
        [Fact]
        public void SingleMatch1()
        {
            // test execution
            var wql = Fixture.ExecuteWql("Description='lorem'");
            var res = wql?.Apply();

            Assert.NotNull(res);
            foreach (var description in res.Select(x => x.Description))
            {
                Assert.Contains("lorem", description);
            }
        }

        /// <summary>
        /// Tests phrase search, which retrieves content from documents that contain a specific order and combination of words defined by the phrase.
        /// </summary>
        [Fact]
        public void MultipleMatch1()
        {
            // test execution
            var wql = Fixture.ExecuteWql("Description='lorem ipsum'");
            var res = wql?.Apply();

            Assert.NotNull(res);
            foreach (var description in res.Select(x => x.Description))
            {
                Assert.Contains("lorem ipsum", description);
            }
        }
    }
}
