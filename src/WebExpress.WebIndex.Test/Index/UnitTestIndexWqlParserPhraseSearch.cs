using Xunit.Abstractions;

namespace WebExpress.WebIndex.Test.Index
{
    /// <summary>
    /// Phrase search (exact word sequence)
    /// </summary>
    public class UnitTestIndexWqlParserPhraseSearch(UnitTestIndexWqlFixture fixture, ITestOutputHelper output) : IClassFixture<UnitTestIndexWqlFixture>
    {
        public ITestOutputHelper Output { get; private set; } = output;

        protected UnitTestIndexWqlFixture Fixture { get; set; } = fixture;

        [Fact]
        public void MultipleWords()
        {
            var wql = Fixture.ExecuteWql("description='lorem ipsum'");
            var res = wql?.Apply();
            var item = res?.FirstOrDefault();

            Assert.NotNull(res);
            Assert.NotNull(item);
            Assert.True(res.Count() > 2);
            Assert.Equal("Description = 'lorem ipsum'", wql.ToString());
            Assert.Contains("lorem", item.Description);
            Assert.NotNull(wql.Filter);
            Assert.Null(wql.Order);
            Assert.Null(wql.Partitioning);
        }
    }
}
