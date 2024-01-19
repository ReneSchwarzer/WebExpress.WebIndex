﻿using Xunit.Abstractions;

namespace WebExpress.WebIndex.Test.Index
{
    /// <summary>
    /// Wildcard search
    /// </summary>
    public class UnitTestIndexWqlParserWildcardSearch(UnitTestIndexWqlFixture fixture, ITestOutputHelper output) : IClassFixture<UnitTestIndexWqlFixture>
    {
        public ITestOutputHelper Output { get; private set; } = output;

        protected UnitTestIndexWqlFixture Fixture { get; set; } = fixture;

        [Fact]
        public void  SingleCharacterFirst()
        {
            var wql = Fixture.ExecuteWql("firstname='?livia'");
            var res = wql?.Apply();
            var item = res?.FirstOrDefault();

            Assert.NotNull(res);
            Assert.NotNull(item);
            Assert.Equal(1, res.Count());
            Assert.Equal("FirstName = '?livia'", wql.ToString());
            Assert.Equal("Olivia", item.FirstName);
            Assert.NotNull(wql.Filter);
            Assert.Null(wql.Order);
            Assert.Null(wql.Partitioning);
        }

        [Fact]
        public void  SingleCharacterMiddle()
        {
            var wql = Fixture.ExecuteWql("firstname='Ol?via'");
            var res = wql?.Apply();
            var item = res?.FirstOrDefault();

            Assert.NotNull(res);
            Assert.NotNull(item);
            Assert.Equal(1, res.Count());
            Assert.Equal("FirstName = 'Ol?via'", wql.ToString());
            Assert.Equal("Olivia", item.FirstName);
            Assert.NotNull(wql.Filter);
            Assert.Null(wql.Order);
            Assert.Null(wql.Partitioning);
        }

        [Fact]
        public void  SingleCharacterEnd()
        {
            var wql = Fixture.ExecuteWql("firstname='Olivi?'");
            var res = wql?.Apply();
            var item = res?.FirstOrDefault();

            Assert.NotNull(res);
            Assert.NotNull(item);
            Assert.Equal(1, res.Count());
            Assert.Equal("FirstName = 'Olivi?'", wql.ToString());
            Assert.Equal("Olivia", item.FirstName);
            Assert.NotNull(wql.Filter);
            Assert.Null(wql.Order);
            Assert.Null(wql.Partitioning);
        }

        [Fact]
        public void  MultipleCharacters()
        {
            var wql = Fixture.ExecuteWql("firstname='Olivi*'");
            var res = wql?.Apply();
            var item = res?.FirstOrDefault();

            Assert.NotNull(res);
            Assert.NotNull(item);
            Assert.Equal(1, res.Count());
            Assert.Equal("FirstName = 'Olivi*'", wql.ToString());
            Assert.Equal("Olivia", item.FirstName);
            Assert.NotNull(wql.Filter);
            Assert.Null(wql.Order);
            Assert.Null(wql.Partitioning);
        }

        [Fact]
        public void  FuzzyEnd()
        {
            var wql = Fixture.ExecuteWql("firstname='Olivi~'");
            var res = wql?.Apply();
            var item = res?.FirstOrDefault();

            Assert.NotNull(res);
            Assert.NotNull(item);
            Assert.Equal(1, res.Count());
            Assert.Equal("FirstName = 'Olivi~'", wql.ToString());
            Assert.Equal("Olivia", item.FirstName);
            Assert.NotNull(wql.Filter);
            Assert.Null(wql.Order);
            Assert.Null(wql.Partitioning);
        }
    }
}
