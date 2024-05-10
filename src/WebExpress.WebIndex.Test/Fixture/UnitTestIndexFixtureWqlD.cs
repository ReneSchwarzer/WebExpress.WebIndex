using System.Globalization;
using WebExpress.WebIndex.Test.Document;
using WebExpress.WebIndex.Wql;

namespace WebExpress.WebIndex.Test.Fixture
{
    public class UnitTestIndexFixtureWqlD : IDisposable
    {
        /// <summary>
        /// Returns the index manager.
        /// </summary>
        public WebIndex.IndexManager IndexManager { get; } = new IndexManagerTest();

        /// <summary>
        /// Returns the test data.
        /// </summary>
        public IEnumerable<UnitTestIndexTestDocumentD> TestData { get; } = UnitTestIndexTestDocumentFactoryD.GenerateTestData();

        /// <summary>
        /// Constructor
        /// </summary>
        public UnitTestIndexFixtureWqlD()
        {
            var context = new IndexContext();
            IndexManager.Initialization(context);
            IndexManager.Register<UnitTestIndexTestDocumentD>(CultureInfo.GetCultureInfo("en"), IndexType.Memory);
            IndexManager.ReIndex(TestData);
        }

        /// <summary>
        /// Disposes of the resources used by the current instance.
        /// </summary>
        public virtual void Dispose()
        {
            IndexManager.Dispose();
        }

        /// <summary>
        /// Executes a wql statement.
        /// </summary>
        /// <param name="wql">Tje wql statement.</param>
        /// <returns>The WQL parser.</returns>
        public IWqlStatement<UnitTestIndexTestDocumentD> ExecuteWql(string wql)
        {
            return IndexManager.ExecuteWql<UnitTestIndexTestDocumentD>(wql);
        }
    }
}
