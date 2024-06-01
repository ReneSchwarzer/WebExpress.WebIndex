using WebExpress.WebIndex.Test.Document;
using WebExpress.WebIndex.Utility;

namespace WebExpress.WebIndex.Test.Fixture
{
    public class UnitTestIndexFixtureIndexD : IDisposable
    {
        /// <summary>
        /// Returns the test data.
        /// </summary>
        public List<UnitTestIndexTestDocumentD> TestData { get; } = UnitTestIndexTestDocumentFactoryD.GenerateTestData();

        /// <summary>
        /// Returns a random document item.
        /// </summary>
        public UnitTestIndexTestDocumentD RandomItem => TestData[new Random().Next() % TestData.Count];

        /// <summary>
        /// Constructor
        /// </summary>
        public UnitTestIndexFixtureIndexD()
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
