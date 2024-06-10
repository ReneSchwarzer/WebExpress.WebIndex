using System.Text;
using WebExpress.WebIndex.WebAttribute;

namespace WebExpress.WebIndex.Test.Document
{
    public abstract class UnitTestIndexTestDocumentFactory 
    {
        /// <summary>
        /// The lorem ipsum vocabulary.
        /// </summary>
        private static readonly string[] LoremIpsumVocabulary =
        [
            "lorem",
            "ipsum",
            "dolor",
            "sit",
            "amet",
            "consectetur",
            "adipiscing",
            "elit",
            "sed",
            "do",
            "eiusmod",
            "tempor",
            "incididunt",
            "ut",
            "labore",
            "et",
            "dolore",
            "magna",
            "aliqua",
            "phasellus",
            "fermentum",
            "malesuada",
            "phasellus",
            "netus",
            "dictum",
            "aenean",
            "placerat",
            "egestas",
            "amet",
            "ornare",
            "taciti",
            "semper",
            "tristique",
            "morbi",
            "sem",
            "leo",
            "tincidunt",
            "aliquet",
            "eu",
            "lectus",
            "scelerisque",
            "quis",
            "sagittis",
            "vivamus",
            "mollis",
            "nisi",
            "enim",
            "laoreet"
        ];

        /// <summary>
        /// The random number generator.
        /// </summary>
        protected static Random Rand { get; } = new(10);

        /// <summary>
        /// Generate lorem ipsum sentence.
        /// </summary>
        /// <param name="numWords">The number of words.</param>
        /// <returns>One orem ipsum sentence.</returns>
        protected static string GenerateLoremIpsum(int numWords)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < numWords; i++)
            {
                sb.Append(LoremIpsumVocabulary[Rand.Next(LoremIpsumVocabulary.Length)]);
                if (i < numWords - 1)
                {
                    sb.Append(' ');
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Generate the vocabulary.
        /// </summary>
        /// <param name="vocabulary">The number of words in the vocabulary.</param>
        /// <param name="wordLength">The minimum length of the words in the vocabulary.</param>
        /// /// <param name="wordLength">The maximum length of the words in the vocabulary.</param>
        /// <returns>The vocabulary.</returns>
        protected static IEnumerable<string> GenerateVocabulary(int vocabulary, int minWordLength, int maxWordLength)
        {
            const string characters = "abcdefghijklmnopqrstuvwxyz";
            var set = new HashSet<string>();

            while (set.Count < vocabulary)
            {
                var word = new string(Enumerable.Repeat(characters, /*Rand.Next(minWordLength, maxWordLength + 1)*/maxWordLength)
                    .Select(s => s[Rand.Next(s.Length)]).ToArray());

                if (!set.Contains(word))
                {
                    set.Add(word);
                }
            }
            
            return set;
        }

        /// <summary>
        /// Generate street names.
        /// </summary>
        /// <param name="index">The house number.</param>
        /// <returns>The street name including the house number.</returns>
        protected static string GenerateSreet(int index)
        {
            int rand = Rand.Next() % 5;

            return rand switch
            {
                0 => $"{index % 99} Elm St.",
                1 => $"{index % 99} Maple Ave.",
                2 => $"{index % 99} Oak Ave.",
                3 => $"{index % 99} Pine St.",
                _ => $"{index % 99} Main St."
            };
        }

        /// <summary>
        /// Generate city names.
        /// </summary>
        /// <param name="index">The index number.</param>
        /// <returns>The city name.</returns>
        protected static string GenerateCity(int index)
        {
            int rand = Rand.Next() % 5;

            return rand switch
            {
                0 => "Phoenix",
                1 => "Los Angeles",
                2 => "Chicago,",
                3 => "Houston",
                _ => "New York City"
            };
        }

        /// <summary>
        /// Generate zip code.
        /// </summary>
        /// <param name="index">The index number.</param>
        /// <returns>The zip code.</returns>
        protected static int GenerateZip(int index)
        {
            int rand = Rand.Next() % 99999;

            return rand;
        }
    }
}
