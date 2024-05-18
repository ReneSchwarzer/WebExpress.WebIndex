﻿using WebExpress.WebIndex.Test.Fixture;
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
        /// Tests the parser.
        /// </summary>
        [Fact]
        public void ParseValidWql()
        {
            var wql = Fixture.ExecuteWql("text~'Helena Helge' :2");
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
            var wql = Fixture.ExecuteWql("text~'Helena Helge' :2 Order by text");
            Assert.False(wql.HasErrors);

            Assert.NotNull(wql.Filter);
            Assert.NotNull(wql.Order);
            Assert.Null(wql.Partitioning);
        }

        /// <summary>
        /// Tests the parser.
        /// </summary>
        [Fact]
        public void ParseValidWqlAnd()
        {
            var wql = Fixture.ExecuteWql("text~'Helena Helge' :2 And text = 'Helge' Order by text skip 1");
            Assert.False(wql.HasErrors);

            Assert.NotNull(wql.Filter);
            Assert.NotNull(wql.Order);
            Assert.NotNull(wql.Partitioning);

            wql = Fixture.ExecuteWql("text~'Helena Helge' :2 & text = 'Helge' Order by text take 10");
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
            var wql = Fixture.ExecuteWql("text~'Helena Helge' :a");
            Assert.True(wql.HasErrors);
        }

        /// <summary>
        /// Tests the parser.
        /// </summary>
        [Fact]
        public void ParseInvalidWqlIn()
        {
            var wql = Fixture.ExecuteWql("text in ('Helena Helge' :2)");
            Assert.True(wql.HasErrors);
        }

        /// <summary>
        /// Tests proximity searches, in which two or more terms must appear at a certain distance from each other.
        /// </summary>
        [Fact]
        public void MultipleWords()
        {
            var wql = Fixture.ExecuteWql("text~'Helena Helge':2");
            var res = wql?.Apply();

            var item = res?.FirstOrDefault();
            Assert.NotNull(res);
            Assert.NotNull(item);
            Assert.True(res.Count() > 2);
            Assert.Equal("Text ~2 'Helena Helge' :2", wql.ToString());
            Assert.Contains("Helena", item.Text);
            Assert.Contains("Helge", item.Text);
            Assert.NotNull(wql.Filter);
            Assert.Null(wql.Order);
            Assert.Null(wql.Partitioning);
        }
    }
}