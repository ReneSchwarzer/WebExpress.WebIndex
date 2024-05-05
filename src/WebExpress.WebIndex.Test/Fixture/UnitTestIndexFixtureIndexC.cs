using WebExpress.WebIndex.Test.Document;

namespace WebExpress.WebIndex.Test.Fixture
{
    public class UnitTestIndexFixtureIndexC : IDisposable
    {
        /// <summary>
        /// Returns the test data.
        /// </summary>
        public List<UnitTestIndexTestDocumentC> TestData { get; } = UnitTestIndexTestDocumentFactoryC.GenerateTestData();

        /// <summary>
        /// Returns a random document item.
        /// </summary>
        public UnitTestIndexTestDocumentC RandomItem => TestData[new Random().Next() % TestData.Count];

        /// <summary>
        /// Constructor
        /// </summary>
        public UnitTestIndexFixtureIndexC()
        {
        }

        /// <summary>
        /// Disposes of the resources used by the current instance.
        /// </summary>
        public virtual void Dispose()
        {
        }
    }
}
