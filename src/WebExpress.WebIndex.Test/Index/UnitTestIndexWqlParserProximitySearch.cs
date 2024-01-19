using Xunit.Abstractions;

namespace WebExpress.WebIndex.Test.Index
{
    /// <summary>
    /// Proximity search
    /// </summary>
    public class UnitTestIndexWqlParserProximitySearch(UnitTestIndexWqlFixture fixture, ITestOutputHelper output) : IClassFixture<UnitTestIndexWqlFixture>
    {
        public ITestOutputHelper Output { get; private set; } = output;

        protected UnitTestIndexWqlFixture Fixture { get; set; } = fixture;

        [Fact]
        public void MultipleWords()
        {
            var wql = Fixture.ExecuteWql("description~'lorem ipsum'~2");
            var res = wql?.Apply();
            var item = res?.FirstOrDefault();

            Assert.NotNull(res);
            Assert.NotNull(item);
            Assert.True(res.Count() > 2);
            Assert.Equal("Description ~2 'lorem ipsum'", wql.ToString());
            Assert.Contains("lorem", item.Description);
            Assert.NotNull(wql.Filter);
            Assert.Null(wql.Order);
            Assert.Null(wql.Partitioning);
        }
    }
}
