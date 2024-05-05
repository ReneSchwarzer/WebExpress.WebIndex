namespace WebExpress.WebIndex.Test.Document
{
    public class UnitTestIndexTestDocumentFactoryB : UnitTestIndexTestDocumentFactory
    {
        /// <summary>
        /// Generates a list of test data for unit testing.
        /// </summary>
        /// <returns>
        /// A list of objects.
        /// </returns>
        public static List<UnitTestIndexTestDocumentB> GenerateTestData()
        {
            var testDataList = new List<UnitTestIndexTestDocumentB>();

            // Add more test data here
            for (int i = 0; i < 1000; i++)
            {
                testDataList.Add(new UnitTestIndexTestDocumentB
                {
                    Id = Guid.NewGuid(),
                    Name = $"Name_{i}",
                    Summary = $"Der Name_{i}",
                    Description = GenerateLoremIpsum(1000),
                    Date = DateTime.Now.AddMonths(i % 12),
                    Price = i,
                    New = i % 2 != 0
                });
            }

            return testDataList;
        }
    }
}
