using WebExpress.WebIndex.Test.Fixture;
using Xunit.Abstractions;

namespace WebExpress.WebIndex.Test.WQL
{
    /// <summary>
    /// Wildcard search
    /// </summary>
    public class UnitTestWqlFuzzySearchA(UnitTestIndexFixtureWqlA fixture, ITestOutputHelper output) : IClassFixture<UnitTestIndexFixtureWqlA>
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
        public void ParseValidWql()
        {
            var wql = Fixture.ExecuteWql("text~'Hel' ~ 80");
            Assert.False(wql.HasErrors);

            Assert.NotNull(wql.Filter);
            Assert.Null(wql.Order);
            Assert.Null(wql.Partitioning);
        }

        /// <summary>
        /// Tests the parser.
        /// </summary>
        [Fact]
        public void ParseValidWqlOrderBy()
        {
            var wql = Fixture.ExecuteWql("text~'Hel' ~ 80 Order by text");
            Assert.False(wql.HasErrors);

            Assert.NotNull(wql.Filter);
            Assert.NotNull(wql.Order);
            Assert.Null(wql.Partitioning);
        }

        /// <summary>
        /// Tests the parser.
        /// </summary>
        [Fact]
        public void ParseValidWqlAnd1()
        {
            var wql = Fixture.ExecuteWql("text~'Hel' ~ 80 And text = 'Helge' Order by text skip 1");
            Assert.False(wql.HasErrors);

            Assert.NotNull(wql.Filter);
            Assert.NotNull(wql.Order);
            Assert.NotNull(wql.Partitioning);

            wql = Fixture.ExecuteWql("text~'Hel' ~ 80 & text = 'Helge' Order by text take 10");
            Assert.False(wql.HasErrors);

            Assert.NotNull(wql.Filter);
            Assert.NotNull(wql.Order);
            Assert.NotNull(wql.Partitioning);
        }

        /// <summary>
        /// Tests the parser.
        /// </summary>
        [Fact]
        public void ParseValidWqlAnd2()
        {
            var wql = Fixture.ExecuteWql("text~'Hel' ~ 80 & text = 'Helge' Order by text take 10");
            Assert.False(wql.HasErrors);

            Assert.NotNull(wql.Filter);
            Assert.NotNull(wql.Order);
            Assert.NotNull(wql.Partitioning);
        }

        /// <summary>
        /// Tests the parser.
        /// </summary>
        [Fact]
        public void ParseInvalidWql()
        {
            var wql = Fixture.ExecuteWql("text~'Hel' ~a0");
            Assert.True(wql.HasErrors);
        }

        /// <summary>
        /// Tests the parser.
        /// </summary>
        [Fact]
        public void ParseInvalidWqlIn()
        {
            var wql = Fixture.ExecuteWql("text in ('Hel' ~ 80)");
            Assert.True(wql.HasErrors);
        }

        /// <summary>
        /// Tests the wildcard search.
        /// </summary>
        [Fact]
        public void Fuzzy()
        {
            var wql = Fixture.ExecuteWql("text~'Hel' ~50");
            var res = wql?.Apply();
            var item = res?.FirstOrDefault();

            Assert.NotNull(res);
            Assert.NotNull(item);
            Assert.Equal(3, res.Count());
            Assert.Equal("Text = 'Hel' ~50", wql.ToString());
            Assert.NotNull(wql.Filter);
            Assert.Null(wql.Order);
            Assert.Null(wql.Partitioning);
        }

        /// <summary>
        /// Tests the wildcard search.
        /// </summary>
        [Fact]
        public void FuzzyFromQueryable()
        {
            var wql = Fixture.ExecuteWql("text~'Hel' ~50");
            var res = wql?.Apply(Fixture.TestData.AsQueryable());
            var item = res?.FirstOrDefault();

            Assert.NotNull(res);
            Assert.NotNull(item);
            Assert.Equal(3, res.Count());
            Assert.Equal("Text = 'Hel~' ~50", wql.ToString());
            Assert.NotNull(wql.Filter);
            Assert.Null(wql.Order);
            Assert.Null(wql.Partitioning);
        }
    }
}
