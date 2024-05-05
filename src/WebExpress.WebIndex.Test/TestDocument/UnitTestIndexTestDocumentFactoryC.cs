namespace WebExpress.WebIndex.Test.Document
{
    public class UnitTestIndexTestDocumentFactoryC : UnitTestIndexTestDocumentFactory
    {
        /// <summary>
        /// Generates a list of test data for unit testing.
        /// </summary>
        /// <returns>
        /// A list of objects.
        /// </returns>
        public static List<UnitTestIndexTestDocumentC> GenerateTestData()
        {
            return GenerateTestData(10, 20, 30, 40).ToList();
        }

        /// <summary>
        /// Generates a list of test data for unit testing.
        /// </summary>
        /// <returns>
        /// A list of objects.
        /// </returns>
        public static IEnumerable<UnitTestIndexTestDocumentC> GenerateTestData(int itemCount, int wordCount, int vocabulary, int wordLength)
        {
            // Add more test data here
            for (int i = 0; i < itemCount; i++)
            {
                yield return new UnitTestIndexTestDocumentC
                {
                    Id = Guid.NewGuid(),
                    Text = GenerateWords(wordCount, vocabulary, wordLength),
                };
            }

            yield break;
        }
    }
}
