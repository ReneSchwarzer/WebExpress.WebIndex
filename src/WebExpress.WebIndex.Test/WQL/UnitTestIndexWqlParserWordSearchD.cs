using WebExpress.WebIndex.Test.Fixture;
using Xunit.Abstractions;

namespace WebExpress.WebIndex.Test.WQL
{
    /// <summary>
    /// Word search
    /// </summary>
    public class UnitTestIndexWqlParserWordSearchD(UnitTestIndexFixtureWqlD fixture, ITestOutputHelper output) : IClassFixture<UnitTestIndexFixtureWqlD>
    {
        /// <summary>
        /// Returns the log.
        /// </summary>
        public ITestOutputHelper Output { get; private set; } = output;

        /// <summary>
        /// Returns the test context.
        /// </summary>
        protected UnitTestIndexFixtureWqlD Fixture { get; set; } = fixture;

        [Fact]
        public void SingleWordFromQueryable()
        {
            var wql = Fixture.ExecuteWql("firstname~'Olivia'");
            var res = wql?.Apply(Fixture.TestData.AsQueryable());
            var item = res?.FirstOrDefault();

            Assert.NotNull(res);
            Assert.NotNull(item);
            Assert.Equal(1, res.Count());
            Assert.Equal("FirstName ~ 'Olivia'", wql.ToString());
            Assert.Equal("Olivia", item.FirstName);
            Assert.NotNull(wql.Filter);
            Assert.Null(wql.Order);
            Assert.Null(wql.Partitioning);
        }

        [Fact]
        public void SingleWord()
        {
            var wql = Fixture.ExecuteWql("firstname~'Olivia'");
            var res = wql?.Apply();
            var item = res?.FirstOrDefault();

            Assert.NotNull(res);
            Assert.NotNull(item);
            Assert.Equal(1, res.Count());
            Assert.Equal("FirstName ~ 'Olivia'", wql.ToString());
            Assert.Equal("Olivia", item.FirstName);
            Assert.NotNull(wql.Filter);
            Assert.Null(wql.Order);
            Assert.Null(wql.Partitioning);
        }

        [Fact]
        public void MultipleWords()
        {
            var wql = Fixture.ExecuteWql("description~'lorem ipsum'");
            var res = wql?.Apply();
            var item = res?.FirstOrDefault();

            Assert.NotNull(res);
            Assert.NotNull(item);
            Assert.True(res.Count() > 2);
            Assert.Equal("Description ~ 'lorem ipsum'", wql.ToString());
            Assert.Contains("lorem", item.Description);
            Assert.NotNull(wql.Filter);
            Assert.Null(wql.Order);
            Assert.Null(wql.Partitioning);
        }
    }
}
