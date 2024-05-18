using WebExpress.WebIndex.Test.Fixture;
using Xunit.Abstractions;

namespace WebExpress.WebIndex.Test.WQL
{
    /// <summary>
    /// Wildcard search
    /// </summary>
    public class UnitTestWqlWildcardSearchA(UnitTestIndexFixtureWqlA fixture, ITestOutputHelper output) : IClassFixture<UnitTestIndexFixtureWqlA>
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
            var wql = Fixture.ExecuteWql("text~'?elena'");
            Assert.False(wql.HasErrors);
        }

        /// <summary>
        /// Tests the parser.
        /// </summary>
        [Fact]
        public void ParseValidWql2()
        {
            var wql = Fixture.ExecuteWql("text='*elena'");
            Assert.False(wql.HasErrors);
        }

        /// <summary>
        /// Tests the parser.
        /// </summary>
        [Fact]
        public void ParseValidWql3()
        {
            var wql = Fixture.ExecuteWql("text='Helen?'");
            Assert.False(wql.HasErrors);
        }

        /// <summary>
        /// Tests the parser.
        /// </summary>
        [Fact]
        public void ParseValidWql4()
        {
            var wql = Fixture.ExecuteWql("text~'Helen*'");
            Assert.False(wql.HasErrors);
        }

        /// <summary>
        /// Tests the parser.
        /// </summary>
        [Fact]
        public void ParseValidWql5()
        {
            var wql = Fixture.ExecuteWql("text='*elen*'");
            Assert.False(wql.HasErrors);
        }

        /// <summary>
        /// Tests the parser.
        /// </summary>
        [Fact]
        public void ParseValidWql6()
        {
            var wql = Fixture.ExecuteWql("text~'?elen?' ~90");
            Assert.False(wql.HasErrors);
        }

        /// <summary>
        /// Tests the parser.
        /// </summary>
        [Fact]
        public void ParseValidWql7()
        {
            var wql = Fixture.ExecuteWql("text='?elen*' ORDER BY text");
            Assert.False(wql.HasErrors);
        }

        /// <summary>
        /// Tests the wildcard search.
        /// </summary>
        [Fact]
        public void SingleCharacterFirst()
        {
            var wql = Fixture.ExecuteWql("text~'?elena'");
            var res = wql?.Apply();
            var item = res?.FirstOrDefault();

            Assert.NotNull(res);
            Assert.NotNull(item);
            Assert.Equal(2, res.Count());
            Assert.Equal("Text ~ '?elena'", wql.ToString());
            Assert.Equal("Hello Helena!", item.Text);
            Assert.NotNull(wql.Filter);
            Assert.Null(wql.Order);
            Assert.Null(wql.Partitioning);
        }

        /// <summary>
        /// Tests the wildcard search.
        /// </summary>
        [Fact]
        public void SingleCharacterMiddle()
        {
            var wql = Fixture.ExecuteWql("text~'He?ena'");
            var res = wql?.Apply();
            var item = res?.FirstOrDefault();

            Assert.NotNull(res);
            Assert.NotNull(item);
            Assert.Equal(2, res.Count());
            Assert.Equal("Text ~ 'He?ena'", wql.ToString());
            Assert.Equal("Hello Helena!", item.Text);
            Assert.NotNull(wql.Filter);
            Assert.Null(wql.Order);
            Assert.Null(wql.Partitioning);
        }

        /// <summary>
        /// Tests the wildcard search.
        /// </summary>
        [Fact]
        public void SingleCharacterEnd()
        {
            var wql = Fixture.ExecuteWql("text~'Helen?'");
            var res = wql?.Apply();
            var item = res?.FirstOrDefault();

            Assert.NotNull(res);
            Assert.NotNull(item);
            Assert.Equal(2, res.Count());
            Assert.Equal("Text ~ 'Helen?'", wql.ToString());
            Assert.Equal("Hello Helena!", item.Text);
            Assert.NotNull(wql.Filter);
            Assert.Null(wql.Order);
            Assert.Null(wql.Partitioning);
        }

        /// <summary>
        /// Tests the wildcard search.
        /// </summary>
        [Fact]
        public void MultipleCharactersFirst()
        {
            var wql = Fixture.ExecuteWql("text~'*ena'");
            var res = wql?.Apply();
            var item = res?.FirstOrDefault();

            Assert.NotNull(res);
            Assert.NotNull(item);
            Assert.Equal(2, res.Count());
            Assert.Equal("Text ~ '*ena'", wql.ToString());
            Assert.Equal("Hello Helena!", item.Text);
            Assert.NotNull(wql.Filter);
            Assert.Null(wql.Order);
            Assert.Null(wql.Partitioning);
        }

        /// <summary>
        /// Tests the wildcard search.
        /// </summary>
        [Fact]
        public void MultipleCharactersMiddle()
        {
            var wql = Fixture.ExecuteWql("text~'He*a'");
            var res = wql?.Apply();
            var item = res?.FirstOrDefault();

            Assert.NotNull(res);
            Assert.NotNull(item);
            Assert.Equal(2, res.Count());
            Assert.Equal("Text ~ 'He*a'", wql.ToString());
            Assert.Equal("Hello Helena!", item.Text);
            Assert.NotNull(wql.Filter);
            Assert.Null(wql.Order);
            Assert.Null(wql.Partitioning);
        }

        /// <summary>
        /// Tests the wildcard search.
        /// </summary>
        [Fact]
        public void MultipleCharactersEnd()
        {
            var wql = Fixture.ExecuteWql("text~'Hel*'");
            var res = wql?.Apply();
            var item = res?.FirstOrDefault();

            Assert.NotNull(res);
            Assert.NotNull(item);
            Assert.Equal(2, res.Count());
            Assert.Equal("Text ~ 'Hel*'", wql.ToString());
            Assert.Equal("Hello Helena!", item.Text);
            Assert.NotNull(wql.Filter);
            Assert.Null(wql.Order);
            Assert.Null(wql.Partitioning);
        }
    }
}
