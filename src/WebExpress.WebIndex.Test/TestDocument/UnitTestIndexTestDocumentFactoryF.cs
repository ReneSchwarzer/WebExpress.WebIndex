namespace WebExpress.WebIndex.Test.Document
{
    /// <summary>
    /// Factory class for creating unit test documents of type UnitTestIndexTestDocumentF.
    /// </summary>
    public class UnitTestIndexTestDocumentFactoryF : UnitTestIndexTestDocumentFactory
    {
        /// <summary>
        /// Generates a list of test data for unit testing.
        /// </summary>
        /// <returns>
        /// A list of objects.
        /// </returns>
        public static List<UnitTestIndexTestDocumentF> GenerateTestData()
        {
            var testDataList = new List<UnitTestIndexTestDocumentF>
            {
                new ()
                {
                    Id = Guid.Parse("c3d50744-5d66-422c-a0ab-ff024c1eacfc"),
                    Name = "😊🌸🐼"
                },
                new ()
                {
                    Id = Guid.Parse("98714568-2925-46a0-b9ea-999a94ef07b4"),
                    Name = "张伟"
                }

            };

            return testDataList;
        }
    }
}
