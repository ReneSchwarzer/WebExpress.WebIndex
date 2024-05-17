using WebExpress.WebIndex.Test.Fixture;
using Xunit.Abstractions;

namespace WebExpress.WebIndex.Test.WQL
{
    /// <summary>
    /// Proximity search
    /// </summary>
    public class UnitTestWqlProximitySearchB(UnitTestIndexFixtureWqlB fixture, ITestOutputHelper output) : IClassFixture<UnitTestIndexFixtureWqlB>
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
        /// Tests proximity searches, in which two or more terms must appear at a certain distance from each other.
        /// </summary>
        [Fact]
        public void MultipleWords()
        {
            var wql = Fixture.ExecuteWql("name~2'Name_123'");
            var res = wql?.Apply();
            
            var item = res?.FirstOrDefault();
            Assert.NotNull(res);
            Assert.NotNull(item);
            Assert.True(res.Count() > 2);
            Assert.Equal("Name ~2 'Name_123'", wql.ToString());
            Assert.Contains("Name_123", item.Name);
            Assert.NotNull(wql.Filter);
            Assert.Null(wql.Order);
            Assert.Null(wql.Partitioning);
        }
    }
}
