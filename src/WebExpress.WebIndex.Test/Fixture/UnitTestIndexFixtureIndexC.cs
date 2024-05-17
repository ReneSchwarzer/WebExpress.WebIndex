using System.Diagnostics;
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

        /// <summary>
        /// Returns the amount of memory used by the process.
        /// </summary>
        /// <returns>The memory used in MB.</returns>
        public long GetUsedMemory()
        {
            long lngSessMemory = Process.GetCurrentProcess().WorkingSet64;

            return lngSessMemory / (1024 * 1024);
        }
    }
}
