using Xunit.Abstractions;

namespace WebExpress.WebIndex.Test.Index
{
    [Collection("UnitTestIndexCollectionFixture")]
    public class UnitTestIndex
    {
        public ITestOutputHelper Output { get; private set; }
        protected UnitTestIndexFixture Fixture { get; set; }

        public UnitTestIndex(UnitTestIndexFixture fixture, ITestOutputHelper output)
        {
            Fixture = fixture;
            Output = output;
        }
    }
}
