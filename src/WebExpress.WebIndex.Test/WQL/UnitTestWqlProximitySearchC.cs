using WebExpress.WebIndex.Test.Fixture;
using Xunit.Abstractions;

namespace WebExpress.WebIndex.Test.WQL
{
    /// <summary>
    /// Proximity search
    /// </summary>
    public class UnitTestWqlProximitySearchC(UnitTestIndexFixtureWqlC fixture, ITestOutputHelper output) : IClassFixture<UnitTestIndexFixtureWqlC>
    {
        /// <summary>
        /// Returns the log.
        /// </summary>
        public ITestOutputHelper Output { get; private set; } = output;

        /// <summary>
        /// Returns the test context.
        /// </summary>
        protected UnitTestIndexFixtureWqlC Fixture { get; set; } = fixture;

        /// <summary>
        /// Tests proximity searches, in which two or more terms must appear at a certain distance from each other.
        /// </summary>
        [Fact]
        public void MultipleWords()
        {
            var term = Fixture.RandomItem.Text.Split(' ').Skip(5).FirstOrDefault();
            var secondTerm = Fixture.RandomItem.Text.Split(' ').Skip(10).FirstOrDefault();
            var wql = Fixture.ExecuteWql($"text~12'{secondTerm} {term}'");
            var res = wql?.Apply();
            
            var item = res?.FirstOrDefault();
            Assert.NotNull(res);
            Assert.NotNull(item);
            Assert.True(res.Count() > 2);
            Assert.Equal($"Text ~12 '{secondTerm} {term}'", wql.ToString());
            Assert.Contains(term, item.Text);
            Assert.Contains(secondTerm, item.Text);
            Assert.NotNull(wql.Filter);
            Assert.Null(wql.Order);
            Assert.Null(wql.Partitioning);
        }
    }
}
