using System.Globalization;
using WebExpress.WebIndex.Wql;

namespace WebExpress.WebIndex.Test.Index
{
    public class UnitTestIndexWqlFixture : UnitTestIndexFixture
    {
        public IEnumerable<UnitTestIndexTestMockA> TestData { get; } = UnitTestIndexTestMockA.GenerateTestData();

        public UnitTestIndexWqlFixture()
        {
            IndexManager.Register<UnitTestIndexTestMockA>(CultureInfo.GetCultureInfo("en"));
            IndexManager.ReIndex(TestData);
        }

        public override void Dispose()
        {
        }

        public IWqlStatement<UnitTestIndexTestMockA> ExecuteWql(string wql)
        {
            return IndexManager.ExecuteWql<UnitTestIndexTestMockA>(wql);
        }
    }
}
