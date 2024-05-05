namespace WebExpress.WebIndex.Test.Document
{
    /// <summary>
    /// Represents a test document for a person.
    /// </summary>
    public class UnitTestIndexTestDocumentFactoryA : UnitTestIndexTestDocumentFactory
    {
        /// <summary>
        /// Generates a list of test data for unit testing.
        /// </summary>
        /// <returns>
        /// A list of objects.
        /// </returns>
        public static List<UnitTestIndexTestDocumentA> GenerateTestData()
        {
            var testDataList = new[]
            {
                new UnitTestIndexTestDocumentA { Id = new Guid("b2e8a5c3-1f6d-4e7b-9e1f-8c1a9d0f2b4a"), Text = "Hello Helena!"},
                new UnitTestIndexTestDocumentA { Id = new Guid("c7d8f9e0-3a2b-4c5d-8e6f-9a1b0c2d4e5f"), Text = "Hello Helena, Helge & Helena!"}
            };

            return testDataList.ToList();
        }
    }
}
