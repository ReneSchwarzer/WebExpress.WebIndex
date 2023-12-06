using System.Globalization;
using WebExpress.WebIndex.Wql;

namespace WebExpress.WebIndex.Test.Index.Wql
{
    public class UnitTestIndexWqlFixture : UnitTestIndexFixture
    {
        public IEnumerable<UnitTestIndexTestDocumentA> TestData { get; } = UnitTestIndexTestDocumentA.GenerateTestData();

        public UnitTestIndexWqlFixture()
        {
            IndexManager.Register<UnitTestIndexTestDocumentA>(CultureInfo.GetCultureInfo("en"));
            IndexManager.ReIndex(TestData);
        }

        public override void Dispose()
        {
        }

        public IWqlStatement<UnitTestIndexTestDocumentA> ExecuteWql(string wql)
        {
            return IndexManager.ExecuteWql<UnitTestIndexTestDocumentA>(wql);
        }
    }
}
