﻿using WebExpress.WebIndex.Test.Fixture;
using Xunit.Abstractions;

namespace WebExpress.WebIndex.Test.WQL
{
    /// <summary>
    /// Proximity search
    /// </summary>
    public class UnitTestWqlSearchProximityD(UnitTestIndexFixtureWqlD fixture, ITestOutputHelper output) : IClassFixture<UnitTestIndexFixtureWqlD>
    {
        /// <summary>
        /// Returns the log.
        /// </summary>
        public ITestOutputHelper Output { get; private set; } = output;

        /// <summary>
        /// Returns the test context.
        /// </summary>
        protected UnitTestIndexFixtureWqlD Fixture { get; set; } = fixture;

        /// <summary>
        /// Tests proximity searches, in which two or more terms must appear at a certain distance from each other.
        /// </summary>
        [Fact]
        public void ProximityMatch()
        {
            // test execution
            var wql = Fixture.ExecuteWql("description~'lorem ipsum':2");
            var res = wql?.Apply();

            Assert.NotNull(res);
            foreach (var item in res)
            {
                Assert.Matches(@"^(?=.*\blorem\b)(?=.*\bipsum\b).*", item.Description);
            }
        }
    }
}
