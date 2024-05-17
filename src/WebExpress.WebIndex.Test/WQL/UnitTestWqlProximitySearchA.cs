using WebExpress.WebIndex.Test.Fixture;
using Xunit.Abstractions;

namespace WebExpress.WebIndex.Test.WQL
{
    /// <summary>
    /// Proximity search
    /// </summary>
    public class UnitTestWqlProximitySearchA(UnitTestIndexFixtureWqlA fixture, ITestOutputHelper output) : IClassFixture<UnitTestIndexFixtureWqlA>
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
        /// Tests proximity searches, in which two or more terms must appear at a certain distance from each other.
        /// </summary>
        [Fact]
        public void MultipleWords()
        {
            var wql = Fixture.ExecuteWql("text~2'Helena Helge'");
            var res = wql?.Apply();
            
            var item = res?.FirstOrDefault();
            Assert.NotNull(res);
            Assert.NotNull(item);
            Assert.True(res.Count() > 2);
            Assert.Equal("Text ~2 'Helena Helge'", wql.ToString());
            Assert.Contains("Helena", item.Text);
            Assert.Contains("Helge", item.Text);
            Assert.NotNull(wql.Filter);
            Assert.Null(wql.Order);
            Assert.Null(wql.Partitioning);
        }
    }
}
