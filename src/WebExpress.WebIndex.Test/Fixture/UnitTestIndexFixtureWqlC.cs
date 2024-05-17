using System.Globalization;
using WebExpress.WebIndex.Test.Document;
using WebExpress.WebIndex.Wql;

namespace WebExpress.WebIndex.Test.Fixture
{
    public class UnitTestIndexFixtureWqlC : IDisposable
    {
        /// <summary>
        /// Returns the index manager.
        /// </summary>
        public WebIndex.IndexManager IndexManager { get; } = new IndexManagerTest();

        /// <summary>
        /// Returns the test data.
        /// </summary>
        public IEnumerable<UnitTestIndexTestDocumentC> TestData { get; } = UnitTestIndexTestDocumentFactoryC.GenerateTestData();

        /// <summary>
        /// Returns a random document item.
        /// </summary>
        public UnitTestIndexTestDocumentC RandomItem { get; private set; }

        /// <summary>
        /// Returns a random term.
        /// </summary>
        public string Term { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public UnitTestIndexFixtureWqlC()
        {
            var context = new IndexContext();
            context.IndexDirectory = Path.Combine(context.IndexDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
            IndexManager.Initialization(context);
            IndexManager.Create<UnitTestIndexTestDocumentC>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);
            IndexManager.ReIndex(TestData);
            RandomItem = TestData.Skip(new Random().Next() % TestData.Count()).FirstOrDefault();
            Term = RandomItem.Text.Split(' ').FirstOrDefault();
        }

        /// <summary>
        /// Disposes of the resources used by the current instance.
        /// </summary>
        public virtual void Dispose()
        {
            IndexManager.Dispose();
            Directory.Delete(IndexManager.Context.IndexDirectory, true);
        }

        /// <summary>
        /// Executes a wql statement.
        /// </summary>
        /// <param name="wql">Tje wql statement.</param>
        /// <returns>The WQL parser.</returns>
        public IWqlStatement<UnitTestIndexTestDocumentC> ExecuteWql(string wql)
        {
            return IndexManager.Select<UnitTestIndexTestDocumentC>(wql);
        }
    }
}
