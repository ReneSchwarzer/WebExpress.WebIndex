using System.Globalization;
using WebExpress.WebIndex.Term;
using Xunit.Abstractions;

namespace WebExpress.WebIndex.Test.Index
{
    [Collection("UnitTestIndexCollectionFixture")]
    public class UnitTestIndexAnalyze
    {
        public ITestOutputHelper Output { get; private set; }
        protected UnitTestIndexFixture Fixture { get; set; }

        public UnitTestIndexAnalyze(UnitTestIndexFixture fixture, ITestOutputHelper output)
        {
            Fixture = fixture;
            Output = output;
        }

        [Fact]
        public void Analyze1_En()
        {
            var input = "abc def, ghi jkl mno-p.";
            var tokens = WebIndexAnalyzer.Analyze(input, CultureInfo.GetCultureInfo("en"));

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

        [Fact]
        public void Analyze2_En()
        {
            var input = Fixture.GetRessource("JourneyThroughTheUniverse.en");
            var tokens = WebIndexAnalyzer.Analyze(input, CultureInfo.GetCultureInfo("en"));

            Assert.True(tokens.Count() == 218); // of 546
        }

        [Fact]
        public void Analyze3_En()
        {
            var input = Fixture.GetRessource("InterstellarConversations.en");
            var tokens = WebIndexAnalyzer.Analyze(input, CultureInfo.GetCultureInfo("en"));

            Assert.True(tokens.Count() == 157); // of 281
        }

        [Fact]
        public void Analyze_De()
        {
            var input = Fixture.GetRessource("BotanischeBindungenMicrosReiseZuVerdantia.de");
            var tokens = WebIndexAnalyzer.Analyze(input, CultureInfo.GetCultureInfo("de"));

            Assert.True(tokens.Count() == 362); // of 731
        }

        /// <summary>
        /// Test of a regional language.
        /// </summary>
        [Fact]
        public void Analyze_DeDE()
        {
            var input = Fixture.GetRessource("BotanischeBindungenMicrosReiseZuVerdantia.de");
            var tokens = WebIndexAnalyzer.Analyze(input, CultureInfo.GetCultureInfo("de-DE"));

            Assert.True(tokens.Count() == 362); // of 731
        }

        /// <summary>
        /// Test of an unsupported language.
        /// </summary>
        [Fact]
        public void Analyze_Fr()
        {
            var input = Fixture.GetRessource("BotanischeBindungenMicrosReiseZuVerdantia.de");
            var tokens = WebIndexAnalyzer.Analyze(input, CultureInfo.GetCultureInfo("fr"));

            Assert.True(tokens.Count() == 716); // of 731
        }
    }
}
