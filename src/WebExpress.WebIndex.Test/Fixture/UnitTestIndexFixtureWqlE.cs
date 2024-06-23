using System.Globalization;
using WebExpress.WebIndex.Test.Document;
using WebExpress.WebIndex.Wql;

namespace WebExpress.WebIndex.Test.Fixture
{
    public class UnitTestIndexFixtureWqlE : UnitTestIndexFixture
    {
        /// <summary>
        /// Returns the index manager.
        /// </summary>
        public WebIndex.IndexManager IndexManager { get; } = new IndexManagerTest();

        /// <summary>
        /// Returns the test data.
        /// </summary>
        public IEnumerable<UnitTestIndexTestDocumentE> TestData { get; } = UnitTestIndexTestDocumentFactoryE.GenerateTestData();

        /// <summary>
        /// Constructor
        /// </summary>
        public UnitTestIndexFixtureWqlE()
        {
            var context = new IndexContext();
            context.IndexDirectory = Path.Combine(context.IndexDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
            IndexManager.Initialization(context);
            IndexManager.Create<UnitTestIndexTestDocumentE>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);
            IndexManager.ReIndex(TestData);
        }

        /// <summary>
        /// Disposes of the resources used by the current instance.
        /// </summary>
        public override void Dispose()
        {
            IndexManager.Dispose();
            Directory.Delete(IndexManager.Context.IndexDirectory, true);
        }

        /// <summary>
        /// Executes a wql statement.
        /// </summary>
        /// <param name="wql">Tje wql statement.</param>
        /// <returns>The WQL parser.</returns>
        public IWqlStatement<UnitTestIndexTestDocumentE> ExecuteWql(string wql)
        {
            return IndexManager.Retrieve<UnitTestIndexTestDocumentE>(wql);
        }
    }
}
