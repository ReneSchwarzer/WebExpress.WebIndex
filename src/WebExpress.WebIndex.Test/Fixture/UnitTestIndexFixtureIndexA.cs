using WebExpress.WebIndex.Test.Document;

namespace WebExpress.WebIndex.Test.Fixture
{
    public class UnitTestIndexFixtureIndexA : UnitTestIndexFixture
    {
        /// <summary>
        /// Returns the test data.
        /// </summary>
        public List<UnitTestIndexTestDocumentA> TestData { get; } = UnitTestIndexTestDocumentFactoryA.GenerateTestData();

        /// <summary>
        /// Returns a random document item.
        /// </summary>
        public UnitTestIndexTestDocumentA RandomItem => TestData[Rand.Next(TestData.Count)];

        /// <summary>
        /// Constructor
        /// </summary>
        public UnitTestIndexFixtureIndexA()
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
