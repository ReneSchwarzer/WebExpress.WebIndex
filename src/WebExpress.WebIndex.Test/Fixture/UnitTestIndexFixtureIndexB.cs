using WebExpress.WebIndex.Test.Document;

namespace WebExpress.WebIndex.Test.Fixture
{
    public class UnitTestIndexFixtureIndexB : UnitTestIndexFixture
    {
        /// <summary>
        /// Returns the test data.
        /// </summary>
        public List<UnitTestIndexTestDocumentB> TestData { get; } = UnitTestIndexTestDocumentFactoryB.GenerateTestData();

        /// <summary>
        /// Returns a random document item.
        /// </summary>
        public UnitTestIndexTestDocumentB RandomItem => TestData[Rand.Next(TestData.Count)];

        /// <summary>
        /// Constructor
        /// </summary>
        public UnitTestIndexFixtureIndexB()
        {
        }

        /// <summary>
        /// Disposes of the resources used by the current instance.
        /// </summary>
        public override void Dispose()
        {
        }
    }
}
