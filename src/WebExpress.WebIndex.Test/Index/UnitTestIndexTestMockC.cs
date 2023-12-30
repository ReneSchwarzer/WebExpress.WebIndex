namespace WebExpress.WebIndex.Test.Index
{
    public class UnitTestIndexTestMockC : UnitTestIndexTestDocument
    {
        public string Text { get; set; }

        public static IEnumerable<UnitTestIndexTestMockC> GenerateTestData(int itemCount, int wordCount, int vocabulary, int wordLength)
        {
            // Add more test data here
            for (int i = 0; i < itemCount; i++)
            {
                yield return new UnitTestIndexTestMockC
                {
                    Id = Guid.NewGuid(),
                    Text = GenerateWords(wordCount, vocabulary, wordLength),
                };
            }

            yield break;
        }

        public override string ToString()
        {
            return $"{Id}: {Text}";
        }
    }
}
