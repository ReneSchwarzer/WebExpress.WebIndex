using System.Globalization;
using WebExpress.WebIndex.Wql;

namespace WebExpress.WebIndex.Test.Index
{
    public class UnitTestIndexWqlFixture : UnitTestIndexFixture
    {
        public IEnumerable<UnitTestIndexTestMockD> TestData { get; } = UnitTestIndexTestMockD.GenerateTestData();

        public UnitTestIndexWqlFixture()
        {
            IndexManager.Register<UnitTestIndexTestMockD>(CultureInfo.GetCultureInfo("en"));
            IndexManager.ReIndex(TestData);
        }

        public override void Dispose()
        {
        }

        public IWqlStatement<UnitTestIndexTestMockD> ExecuteWql(string wql)
        {
            return IndexManager.ExecuteWql<UnitTestIndexTestMockD>(wql);
        }
    }
}
