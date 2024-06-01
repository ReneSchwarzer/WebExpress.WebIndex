using WebExpress.WebIndex.Test.Document;
using WebExpress.WebIndex.Utility;

namespace WebExpress.WebIndex.Test.Fixture
{
    public class UnitTestIndexFixtureIndexE : IDisposable
    {
        /// <summary>
        /// Returns the test data.
        /// </summary>
        public List<UnitTestIndexTestDocumentE> TestData { get; } = UnitTestIndexTestDocumentFactoryE.GenerateTestData();

        /// <summary>
        /// Returns a random document item.
        /// </summary>
        public UnitTestIndexTestDocumentE RandomItem => TestData[new Random().Next() % TestData.Count];

        /// <summary>
        /// Constructor
        /// </summary>
        public UnitTestIndexFixtureIndexE()
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
