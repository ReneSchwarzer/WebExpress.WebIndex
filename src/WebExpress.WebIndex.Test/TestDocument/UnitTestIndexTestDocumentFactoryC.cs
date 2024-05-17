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
            return GenerateTestData(100, 10, 3000, 20).ToList();
        }

        /// <summary>
        /// Generates a list of test data for unit testing.
        /// </summary>
        /// <returns>
        /// A list of objects.
        /// </returns>
        public static IEnumerable<UnitTestIndexTestDocumentC> GenerateTestData(int itemCount, int wordCount, int vocabulary, int wordLength)
        {
            var set = GenerateVocabulary(vocabulary, 3, wordLength);

            for (int i = 0; i < itemCount; i++)
            {
                var words = new List<string>();
                //for (int j = 0; j < Rand.Next(wordCount / 2, wordCount); j++)
                for (int j = 0; j < wordCount; j++)
                {
                    words.Add(set.Skip(Rand.Next() % set.Count()).FirstOrDefault());
                }

                yield return new UnitTestIndexTestDocumentC
                {
                    Id = Guid.NewGuid(),
                    Text = string.Join(" ", words),
                    Number = i
                };
            }

            yield break;
        }
    }
}
