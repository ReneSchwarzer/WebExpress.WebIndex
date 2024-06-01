using WebExpress.WebIndex.Test.Document;
using WebExpress.WebIndex.Utility;

namespace WebExpress.WebIndex.Test.Fixture
{
    public class UnitTestIndexFixtureIndexB : IDisposable
    {
        /// <summary>
        /// Returns the test data.
        /// </summary>
        public List<UnitTestIndexTestDocumentB> TestData { get; } = UnitTestIndexTestDocumentFactoryB.GenerateTestData();

        /// <summary>
        /// Returns a random document item.
        /// </summary>
        public UnitTestIndexTestDocumentB RandomItem => TestData[new Random().Next() % TestData.Count];

        /// <summary>
        /// Constructor
        /// </summary>
        public UnitTestIndexFixtureIndexB()
        {
        }

        /// <summary>
        /// Disposes of the resources used by the current instance.
        /// </summary>
        public virtual void Dispose()
        {
            Profiling.Store();
        }
    }
}
