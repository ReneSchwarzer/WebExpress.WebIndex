using WebExpress.WebIndex.Test.Document;
using WebExpress.WebIndex.Utility;

namespace WebExpress.WebIndex.Test.Fixture
{
    public class UnitTestIndexFixtureIndexA : IDisposable
    {
        /// <summary>
        /// Returns the test data.
        /// </summary>
        public List<UnitTestIndexTestDocumentA> TestData { get; } = UnitTestIndexTestDocumentFactoryA.GenerateTestData();

        /// <summary>
        /// Returns a random document item.
        /// </summary>
        public UnitTestIndexTestDocumentA RandomItem => TestData[new Random().Next() % TestData.Count];

        /// <summary>
        /// Constructor
        /// </summary>
        public UnitTestIndexFixtureIndexA()
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
