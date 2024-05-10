using System.Globalization;
using WebExpress.WebIndex.Term;
using WebExpress.WebIndex.Term.Pipeline;
using WebExpress.WebIndex.Test.Fixture;
using Xunit.Abstractions;

namespace WebExpress.WebIndex.Test.Token
{
    public class UnitTestIndexPipeStageMisspelled : IClassFixture<UnitTestIndexFixtureToken>
    {
        /// <summary>
        /// Returns the log.
        /// </summary>
        public ITestOutputHelper Output { get; private set; }

        /// <summary>
        /// Returns the test context.
        /// </summary>
        protected UnitTestIndexFixtureToken Fixture { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fixture">The test context.</param>
        /// <param name="output">The log.</param>
        public UnitTestIndexPipeStageMisspelled(UnitTestIndexFixtureToken fixture, ITestOutputHelper output)
        {
            Fixture = fixture;
            Output = output;
        }

        /// <summary>
        /// Tests the misspelled method. This function is part of the lemmatization process and corrects commonly misspelled terms.
        /// </summary>
        [Fact]
        public void Misspelled_En()
        {
            var culture = CultureInfo.GetCultureInfo("en");
            var pipeStage = new IndexPipeStageConverterMisspelled(Fixture.Context);

            (string, string)[] words =
            [
                ("febuary", "february"),
                ("finaly", "finally"),
                ("flourescent", "fluorescent"),
                ("foriegn", "foreign"),
                ("greatful", "grateful"),
                ("garentee", "guarantee"),
                ("happend", "happened"),
                ("independant", "independent"),
                ("inturrupt", "interrupt"),
                ("knowlege", "knowledge"),
                ("libary", "library"),
                ("noticable", "noticeable"),
                ("ocassion", "occasion"),
                ("occured", "occurred")
            ];

            var tokenizer = new IndexTermTokenizer();
            var res = pipeStage.Process(tokenizer.Tokenize(string.Join(" ",  words.Select(x => x.Item1))), culture)
                .Select(x => x.Value)
                .ToList();

            Assert.True(res.Intersect(words.Select(x => x.Item2)).Count() == res.Count);
        }
    }
}
