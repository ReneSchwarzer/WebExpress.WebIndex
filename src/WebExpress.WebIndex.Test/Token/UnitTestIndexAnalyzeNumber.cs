using System.Globalization;
using WebExpress.WebIndex.Test.Fixture;
using Xunit.Abstractions;

namespace WebExpress.WebIndex.Test.Token
{
    public class UnitTestIndexAnalyzeNumber : IClassFixture<UnitTestIndexFixtureToken>
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
        public UnitTestIndexAnalyzeNumber(UnitTestIndexFixtureToken fixture, ITestOutputHelper output)
        {
            Fixture = fixture;
            Output = output;
        }

        /// <summary>
        /// Tests the number input.
        /// </summary>
        [Fact]
        public void Number_En()
        {
            // test execution
            var tokens = Fixture.TokenAnalyzer.Analyze("1", CultureInfo.GetCultureInfo("en"));

            Assert.Equal(1, (double)tokens.FirstOrDefault()?.Value);
        }

        /// <summary>
        /// Tests the number input.
        /// </summary>
        [Fact]
        public void Number_De()
        {
            // test execution
            var tokens = Fixture.TokenAnalyzer.Analyze("1", CultureInfo.GetCultureInfo("de"));

            Assert.Equal(1, (double)tokens.FirstOrDefault()?.Value);
        }

        /// <summary>
        /// Tests the number input.
        /// </summary>
        [Fact]
        public void NevativeNumber_En()
        {
            // test execution
            var tokens = Fixture.TokenAnalyzer.Analyze("-1", CultureInfo.GetCultureInfo("en"));

            Assert.Equal(-1, (double)tokens.FirstOrDefault()?.Value);
        }

        /// <summary>
        /// Tests the number input.
        /// </summary>
        [Fact]
        public void NevativeNumber_De()
        {
            // test execution
            var tokens = Fixture.TokenAnalyzer.Analyze("-1", CultureInfo.GetCultureInfo("de"));

            Assert.Equal(-1, (double)tokens.FirstOrDefault()?.Value);
        }

        /// <summary>
        /// Tests the number input.
        /// </summary>
        [Fact]
        public void InvalidNevativeNumber_En()
        {
            // test execution
            var tokens = Fixture.TokenAnalyzer.Analyze("-st", CultureInfo.GetCultureInfo("en"));

            Assert.Equal("st", tokens.FirstOrDefault()?.Value);
        }

        /// <summary>
        /// Tests the number input.
        /// </summary>
        [Fact]
        public void InvalidNevativeNumber_De()
        {
            // test execution
            var tokens = Fixture.TokenAnalyzer.Analyze("-st", CultureInfo.GetCultureInfo("de"));

            Assert.Equal("st", tokens.FirstOrDefault()?.Value);
        }

        /// <summary>
        /// Tests the number input.
        /// </summary>
        [Fact]
        public void Double_En()
        {
            // test execution
            var tokens = Fixture.TokenAnalyzer.Analyze("1,0038.76", CultureInfo.GetCultureInfo("en"));

            Assert.Equal(10038.76, (double)tokens.FirstOrDefault()?.Value);
        }

        /// <summary>
        /// Tests the number input.
        /// </summary>
        [Fact]
        public void Double_De()
        {
            // test execution
            var tokens = Fixture.TokenAnalyzer.Analyze("1.0038,76", CultureInfo.GetCultureInfo("de"));

            Assert.Equal(10038.76, (double)tokens.FirstOrDefault()?.Value);
        }

        /// <summary>
        /// Tests the number input.
        /// </summary>
        [Fact]
        public void InvalidDouble_En()
        {
            // test execution
            var tokens = Fixture.TokenAnalyzer.Analyze("1.0038,76", CultureInfo.GetCultureInfo("en"));

            Assert.Equal("1.0038,76", tokens.FirstOrDefault()?.Value);
        }

        /// <summary>
        /// Tests the number input.
        /// </summary>
        [Fact]
        public void InvalidDouble_De()
        {
            // test execution
            var tokens = Fixture.TokenAnalyzer.Analyze("1,0038.76", CultureInfo.GetCultureInfo("de"));

            Assert.Equal(10038.76, (double)tokens.FirstOrDefault()?.Value);
        }

        /// <summary>
        /// Tests the number with exponent input.
        /// </summary>
        [Fact]
        public void Exponent_En()
        {
            // test execution
            var tokens = Fixture.TokenAnalyzer.Analyze("1.0038e76", CultureInfo.GetCultureInfo("en"));

            Assert.Equal(1.0038E+76, (double)tokens.FirstOrDefault()?.Value);
        }

        /// <summary>
        /// Tests the number with exponent input.
        /// </summary>
        [Fact]
        public void Exponent_De()
        {
            // test execution
            var tokens = Fixture.TokenAnalyzer.Analyze("1,0038e76", CultureInfo.GetCultureInfo("de"));

            Assert.Equal(1.0038E+76, (double)tokens.FirstOrDefault()?.Value);
        }

        /// <summary>
        /// Tests the negative number input.
        /// </summary>
        [Fact]
        public void NevativeExponent_En()
        {
            // test execution
            var tokens = Fixture.TokenAnalyzer.Analyze("1e-76", CultureInfo.GetCultureInfo("en"));

            Assert.Equal(1E-76, (double)tokens.FirstOrDefault()?.Value);
        }

        /// <summary>
        /// Tests the number input.
        /// </summary>
        [Fact]
        public void NevativeExponent_De()
        {
            // test execution
            var tokens = Fixture.TokenAnalyzer.Analyze("1e-76", CultureInfo.GetCultureInfo("de"));

            Assert.Equal(1E-76, (double)tokens.FirstOrDefault()?.Value);
        }

        /// <summary>
        /// Tests the text_number input.
        /// </summary>
        [Fact]
        public void TextWithNumber_En1()
        {
            // test execution
            var tokens = Fixture.TokenAnalyzer.Analyze("N_1", CultureInfo.GetCultureInfo("en"));

            Assert.Equal("n_1", tokens.FirstOrDefault()?.Value);
        }

        /// <summary>
        /// Tests the text_number input.
        /// </summary>
        [Fact]
        public void TextWithNumber_En2()
        {
            // test execution
            var tokens = Fixture.TokenAnalyzer.Analyze("N1", CultureInfo.GetCultureInfo("en"));

            Assert.Equal("n1", tokens.FirstOrDefault()?.Value);
        }

        /// <summary>
        /// Tests the wildcard input.
        /// </summary>
        [Fact]
        public void NumberWithWildcard_En()
        {
            // test execution
            var tokens = Fixture.TokenAnalyzer.Analyze("Name_?23", CultureInfo.GetCultureInfo("en"), true);

            Assert.Equal("name_?23", tokens.FirstOrDefault()?.Value);
        }
    }
}
