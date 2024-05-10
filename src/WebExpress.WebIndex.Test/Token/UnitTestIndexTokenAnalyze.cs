using System.Globalization;
using WebExpress.WebIndex.Term;
using WebExpress.WebIndex.Test.Fixture;
using Xunit.Abstractions;

namespace WebExpress.WebIndex.Test.Token
{
    public class UnitTestIndexAnalyze : IClassFixture<UnitTestIndexFixtureToken>
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
        public UnitTestIndexAnalyze(UnitTestIndexFixtureToken fixture, ITestOutputHelper output)
        {
            Fixture = fixture;
            Output = output;
        }

        /// <summary>
        /// Tests the analysis function of an supported language.
        /// </summary>
        [Fact]
        public void Analyze1_En()
        {
            // preconditions
            var input = "abc def, ghi jkl mno-p.";
            
            // test execution
            var tokens = Fixture.TokenAnalyzer.Analyze(input, CultureInfo.GetCultureInfo("en"));

            Assert.True(tokens.Count() == 5);
            Assert.True(tokens.First().Position == 0);
            Assert.True(tokens.First().Value == "abc");
            Assert.True(tokens.Skip(1).First().Position == 1);
            Assert.True(tokens.Skip(1).First().Value == "def");
            Assert.True(tokens.Skip(2).First().Position == 2);
            Assert.True(tokens.Skip(2).First().Value == "ghi");
            Assert.True(tokens.Skip(3).First().Position == 3);
            Assert.True(tokens.Skip(3).First().Value == "jkl");
            Assert.True(tokens.Skip(4).First().Position == 4);
            Assert.True(tokens.Skip(4).First().Value == "mno-p");
        }

        /// <summary>
        /// Tests the analysis function of an supported language.
        /// </summary>
        [Fact]
        public void Analyze2_En()
        {
            // preconditions
            var input = Fixture.GetRessource("JourneyThroughTheUniverse.en");

            // test execution
            var tokens = Fixture.TokenAnalyzer.Analyze(input, CultureInfo.GetCultureInfo("en"));

            Assert.Equal(241, tokens.Count());  // of 546
        }

        /// <summary>
        /// Tests the analysis function of an supported language.
        /// </summary>
        [Fact]
        public void Analyze3_En()
        {
            var input = Fixture.GetRessource("InterstellarConversations.en");

            // test execution
            var tokens = Fixture.TokenAnalyzer.Analyze(input, CultureInfo.GetCultureInfo("en"));

            Assert.Equal(170, tokens.Count()); // of 281
        }

        /// <summary>
        /// Tests the analysis function of an supported language.
        /// </summary>
        [Fact]
        public void Analyze_De()
        {
            // preconditions
            var input = Fixture.GetRessource("BotanischeBindungenMicrosReiseZuVerdantia.de");

            // test execution
            var tokens = Fixture.TokenAnalyzer.Analyze(input, CultureInfo.GetCultureInfo("de"));

            Assert.Equal(418, tokens.Count()); // of 731
        }

        /// <summary>
        /// Tests the analysis function of a regional language.
        /// </summary>
        [Fact]
        public void Analyze_DeDE()
        {
            // preconditions
            var input = Fixture.GetRessource("BotanischeBindungenMicrosReiseZuVerdantia.de");

            // test execution
            var tokens = Fixture.TokenAnalyzer.Analyze(input, CultureInfo.GetCultureInfo("de-DE"));

            Assert.Equal(418, tokens.Count()); // of 731
        }

        /// <summary>
        /// Tests the analysis function of an unsupported language.
        /// </summary>
        [Fact]
        public void Analyze_Fr()
        {
            // preconditions
            var input = Fixture.GetRessource("BotanischeBindungenMicrosReiseZuVerdantia.de");

            // test execution
            var tokens = Fixture.TokenAnalyzer.Analyze(input, CultureInfo.GetCultureInfo("fr"));

            Assert.Equal(719, tokens.Count()); // of 731
        }
    }
}
