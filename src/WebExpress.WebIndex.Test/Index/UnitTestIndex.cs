using System.Diagnostics;
using System.Globalization;
using WebExpress.WebIndex;
using WebExpress.WebIndex.Term;
using Xunit.Abstractions;

namespace WebExpress.Test.Index
{
    public class UnitTestIndex : IClassFixture<UnitTestIndexFixture>
    {
        public ITestOutputHelper Output { get; private set; }
        protected UnitTestIndexFixture Fixture { get; set; }

        public UnitTestIndex(UnitTestIndexFixture fixture, ITestOutputHelper output)
        {
            Fixture = fixture;
            Output = output;
        }

        [Fact]
        public void Register()
        {
            Fixture.IndexManager.Register<UnitTestIndexTestDocumentA>(CultureInfo.GetCultureInfo("en"));
            Fixture.IndexManager.Register<UnitTestIndexTestDocumentB>(CultureInfo.GetCultureInfo("en"));

            Assert.NotNull(Fixture.IndexManager.GetIndexDocument<UnitTestIndexTestDocumentA>());
            Assert.NotNull(Fixture.IndexManager.GetIndexDocument<UnitTestIndexTestDocumentB>());
        }

        [Fact]
        public void ReIndexTestDataA()
        {
            Fixture.GetUsedMemory();

            var testData = UnitTestIndexTestDocumentA.GenerateTestData();

            Fixture.IndexManager.Register<UnitTestIndexTestDocumentA>(CultureInfo.GetCultureInfo("en"));
            Fixture.IndexManager.ReIndex(testData);

            Fixture.GetUsedMemory();
        }

        [Fact]
        public void ReIndexTestDataB()
        {
            Fixture.GetUsedMemory();

            var testData = UnitTestIndexTestDocumentB.GenerateTestData();

            Fixture.IndexManager.Register<UnitTestIndexTestDocumentB>(CultureInfo.GetCultureInfo("en"));
            Fixture.IndexManager.ReIndex(testData);

            Fixture.GetUsedMemory();
        }

        [Fact]
        public void ReIndexTestDataC()
        {
            var stopWatch = new Stopwatch();
            var itemCount = 1000;
            var wordCount = 1000;
            var vocabulary = 40000;
            var wordLength = 10;

            var testData = UnitTestIndexTestDocumentC.GenerateTestData(itemCount, wordCount, vocabulary, wordLength).ToList();

            Output.WriteLine($"ReIndex {itemCount.ToString("#,##0")} items, {vocabulary.ToString("#,##0")} vocabulary and {wordLength.ToString("#,##0")} word length");

            Fixture.IndexManager.Register<UnitTestIndexTestDocumentC>(CultureInfo.GetCultureInfo("en"));

            // preparing for a measurement
            var begin = Fixture.GetUsedMemory();
            stopWatch.Start();

            Fixture.IndexManager.ReIndex(testData);

            // stop measurement
            stopWatch.Stop();
            var end = Fixture.GetUsedMemory();
            var elapsedReindex = stopWatch.Elapsed;
            var usedReindex = (end - begin) / 1024 / 1024; // in MB

            // preparing for a measurement
            begin = Fixture.GetUsedMemory();
            stopWatch.Start();
            Fixture.IndexManager.ExecuteWql<UnitTestIndexTestDocumentC>("Text ~ 'abcdaaaaaa'");

            // stop measurement
            stopWatch.Stop();
            end = Fixture.GetUsedMemory();
            var elapsedCollect = stopWatch.Elapsed;
            var usedCollect = (end - begin) / 1024 / 1024; // in MB

            Output.WriteLine($"ReIndex take: {elapsedReindex}");
            Output.WriteLine("ReIndex ram used: " + (Convert.ToDouble(usedReindex)).ToString("0.##") + " MB");

            Output.WriteLine($"Collect take: {elapsedCollect}");
            Output.WriteLine("Collect ram used: " + (Convert.ToDouble(usedCollect)).ToString("0.##") + " MB");
        }

        [Fact]
        public void ReIndexTestDataSeriesC()
        {
            var stopWatch = new Stopwatch();
            var file = File.CreateText(Path.Combine(Environment.CurrentDirectory, "myindex-test.csv"));
            var indexDict = Path.Combine(Environment.CurrentDirectory, "index");

            var itemCount = Enumerable.Range(1, 1).Select(x => x * 1000);
            var wordCount = new int[] { 1000 };
            var vocabulary = new int[] { 40000 };
            var wordLength = new int[] { 10 };

            //var wordCount = new int[] { 100, 1000 };
            //var vocabulary = new int[] { 10000, 20000, 30000, 40000, 50000, 60000, 70000 };
            //var wordLength = new int[] { 10, 20, 30, 40, 50 };

            var heading = "item count;wordCount;vocabulary;wordLength;elapsed reindex;";
            Output.WriteLine(heading);
            file.WriteLine(heading);

            foreach (var w in wordCount)
            {
                foreach (var i in itemCount)
                {
                    foreach (var v in vocabulary)
                    {
                        foreach (var l in wordLength)
                        {
                            // remove the index files if exists
                            if (Directory.Exists(indexDict))
                            {
                                Directory.Delete(indexDict, true);
                            }

                            var testData = UnitTestIndexTestDocumentC.GenerateTestData(i, w, v, l);

                            Fixture.IndexManager.Register<UnitTestIndexTestDocumentC>(CultureInfo.GetCultureInfo("en"), (uint)i, IndexType.Storage);

                            // preparing for a measurement
                            stopWatch.Start();

                            //Fixture.IndexManager.Add(testData.FirstOrDefault());
                            Fixture.IndexManager.ReIndex(testData);

                            // stop measurement
                            var elapsedReindex = stopWatch.Elapsed;
                            stopWatch.Reset();

                            var stat = Fixture
                                .IndexManager
                                .GetIndexDocument<UnitTestIndexTestDocumentC>()
                                .GetReverseIndex(typeof(UnitTestIndexTestDocumentC).GetProperty("Text"));

                            var output = $"{i};{w};{v};{l};{elapsedReindex.ToString(@"hh\:mm\:ss")};{stat}";

                            Output.WriteLine(output);
                            file.WriteLine(output);

                            Fixture.IndexManager.Remove<UnitTestIndexTestDocumentC>();

                            file.Flush();
                        }
                    }
                }
            }
        }

        [Fact]
        public void Analyze()
        {
            var input = "abc def, ghi jkl mno-p.";
            var tokens = IndexAnalyzer.Analyze(input, CultureInfo.GetCultureInfo("en"));

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
        public void AnalyzeEn1()
        {
            var input = Fixture.GetRessource("JourneyThroughTheUniverse.en");
            var tokens = IndexAnalyzer.Analyze(input, CultureInfo.GetCultureInfo("en"));

            Assert.True(tokens.Count() == 218); // of 546
        }

        [Fact]
        public void AnalyzeEn2()
        {
            var input = Fixture.GetRessource("InterstellarConversations.en");
            var tokens = IndexAnalyzer.Analyze(input, CultureInfo.GetCultureInfo("en"));

            Assert.True(tokens.Count() == 157); // of 281
        }

        [Fact]
        public void AnalyzeDe()
        {
            var input = Fixture.GetRessource("BotanischeBindungenMicrosReiseZuVerdantia.de");
            var tokens = IndexAnalyzer.Analyze(input, CultureInfo.GetCultureInfo("de"));

            Assert.True(tokens.Count() == 361); // of 731
        }
    }
}
