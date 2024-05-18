using WebExpress.WebIndex.Test.Document;
using WebExpress.WebIndex.Test.Fixture;
using Xunit.Abstractions;

namespace WebExpress.WebIndex.Test.WQL
{
    /// <summary>
    /// Phrase search (exact word sequence)
    /// </summary>
    public class UnitTestWqlPhraseSearchA(UnitTestIndexFixtureWqlA fixture, ITestOutputHelper output) : IClassFixture<UnitTestIndexFixtureWqlA>
    {
        /// <summary>
        /// Returns the log.
        /// </summary>
        public ITestOutputHelper Output { get; private set; } = output;

        /// <summary>
        /// Returns the test context.
        /// </summary>
        protected UnitTestIndexFixtureWqlA Fixture { get; set; } = fixture;

        /// <summary>
        /// Tests the parser.
        /// </summary>
        [Fact]
        public void ParseValidWql1()
        {
            var wql = Fixture.ExecuteWql("text='Helena'");
            Assert.False(wql.HasErrors);
        }

        /// <summary>
        /// Tests the parser.
        /// </summary>
        [Fact]
        public void ParseValidWql2()
        {
            var wql = Fixture.ExecuteWql("text=\"Helena\"");
            Assert.False(wql.HasErrors);
        }

        /// <summary>
        /// Tests the parser.
        /// </summary>
        [Fact]
        public void ParseValidWql3()
        {
            var wql = Fixture.ExecuteWql("text=Helena");
            Assert.False(wql.HasErrors);
        }

        /// <summary>
        /// Tests the parser.
        /// </summary>
        [Fact]
        public void ParseValidWql4()
        {
            var wql = Fixture.ExecuteWql("text='Helena Helge'");
            Assert.False(wql.HasErrors);
        }

        /// <summary>
        /// Tests the parser.
        /// </summary>
        [Fact]
        public void ParseInvalidWql1()
        {
            var wql = Fixture.ExecuteWql("text~Helena Helge order by text");
            Assert.True(wql.HasErrors);
        }

        /// <summary>
        /// Tests the parser.
        /// </summary>
        [Fact]
        public void ParseInvalidWql2()
        {
            var wql = Fixture.ExecuteWql("text~'Helena Helge order by text");
            Assert.True(wql.HasErrors);
        }

        /// <summary>
        /// Tests the parser.
        /// </summary>
        [Fact]
        public void ParseInvalidWql3()
        {
            var wql = Fixture.ExecuteWql("text~'Helena Helge\" order by text");
            Assert.True(wql.HasErrors);
        }

        /// <summary>
        /// Tests the parser.
        /// </summary>
        [Fact]
        public void ParseInvalidWql4()
        {
            var wql = Fixture.ExecuteWql("~'Helena Helge\" order by text");
            Assert.True(wql.HasErrors);
        }

        /// <summary>
        /// Tests phrase search, which retrieves content from documents that contain a specific order and combination of words defined by the phrase.
        /// </summary>
        [Fact]
        public void MultipleWords()
        {
            Fixture.IndexManager.Insert(new UnitTestIndexTestDocumentD()
            {
                Id = Guid.NewGuid(),

            });

            var wql = Fixture.ExecuteWql("text='Hello Helena, Hello Helge'");
            var res = wql?.Apply();

            Assert.NotNull(res);
            Assert.Equal(2, res.Count());
            Assert.Equal("Text = 'Hello Helena, Hello Helge'", wql.ToString());
            Assert.NotNull(wql.Filter);
            Assert.Null(wql.Order);
            Assert.Null(wql.Partitioning);
        }
    }
}
