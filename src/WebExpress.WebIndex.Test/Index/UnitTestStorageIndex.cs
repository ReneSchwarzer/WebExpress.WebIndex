using System.Diagnostics;
using System.Globalization;
using Xunit.Abstractions;

namespace WebExpress.WebIndex.Test.Index
{
    [Collection("UnitTestIndexCollectionFixture")]
    public class UnitTestStorageIndex(UnitTestIndexFixture fixture, ITestOutputHelper output)
    {
        public ITestOutputHelper Output { get; private set; } = output;
        protected UnitTestIndexFixture Fixture { get; set; } = fixture;

        [Fact]
        public void Register()
        {
            Fixture.IndexManager.Register<UnitTestIndexTestMockA>(CultureInfo.GetCultureInfo("en"), 11, IndexType.Storage);

            Assert.NotNull(Fixture.IndexManager.GetIndexDocument<UnitTestIndexTestMockA>());
        }

        [Fact]
        public void Clear()
        {
            var testData = UnitTestIndexTestMockA.GenerateTestData();

            Fixture.IndexManager.Register<UnitTestIndexTestMockA>(CultureInfo.GetCultureInfo("en"), 11, IndexType.Storage);
            Fixture.IndexManager.ReIndex(testData);
            Fixture.IndexManager.Clear<UnitTestIndexTestMockA>();

            var wql = Fixture.IndexManager.ExecuteWql<UnitTestIndexTestMockA>("name = 'Noah'");
            var item = wql.Apply();

            Assert.NotNull(wql);
            Assert.False(item.Any());
        }

        [Fact]
        public void ReIndexTestDataA_En()
        {
            var testData = UnitTestIndexTestMockA.GenerateTestData();

            Fixture.IndexManager.Register<UnitTestIndexTestMockA>(CultureInfo.GetCultureInfo("en"), 11, IndexType.Storage);
            Fixture.IndexManager.ReIndex(testData);

            var wql = Fixture.IndexManager.ExecuteWql<UnitTestIndexTestMockA>("name = 'Noah'");
            var item = wql.Apply();

            Assert.NotNull(wql);
            Assert.Equal(2, item.Count());
        }

        [Fact]
        public void ReIndexTestDataA_De()
        {
            var testData = UnitTestIndexTestMockA.GenerateTestData();

            Fixture.IndexManager.Register<UnitTestIndexTestMockA>(CultureInfo.GetCultureInfo("de"), 11, IndexType.Storage);
            Fixture.IndexManager.ReIndex(testData);

            var wql = Fixture.IndexManager.ExecuteWql<UnitTestIndexTestMockA>("name = 'Noah'");
            var item = wql.Apply();

            Assert.NotNull(wql);
            Assert.Equal(2, item.Count());
        }

        [Fact]
        public void ReIndexTestDataA_DeDE()
        {
            var testData = UnitTestIndexTestMockA.GenerateTestData();

            Fixture.IndexManager.Register<UnitTestIndexTestMockA>(CultureInfo.GetCultureInfo("de-DE"), 11, IndexType.Storage);
            Fixture.IndexManager.ReIndex(testData);

            var wql = Fixture.IndexManager.ExecuteWql<UnitTestIndexTestMockA>("name = 'Noah'");
            var item = wql.Apply();

            Assert.NotNull(wql);
            Assert.Equal(2, item.Count());
        }

        [Fact]
        public void ReIndexTestDataA_Fr()
        {
            var testData = UnitTestIndexTestMockA.GenerateTestData();

            Fixture.IndexManager.Register<UnitTestIndexTestMockA>(CultureInfo.GetCultureInfo("fr"), 11, IndexType.Storage);
            Fixture.IndexManager.ReIndex(testData);

            var wql = Fixture.IndexManager.ExecuteWql<UnitTestIndexTestMockA>("name = 'Noah'");
            var item = wql.Apply();

            Assert.NotNull(wql);
            Assert.Equal(2, item.Count());
        }

        [Fact]
        public void ReIndexTestDataB()
        {
            var testData = UnitTestIndexTestMockB.GenerateTestData();

            Fixture.IndexManager.Register<UnitTestIndexTestMockB>(CultureInfo.GetCultureInfo("en"), ushort.MaxValue, IndexType.Storage);
            Fixture.IndexManager.ReIndex(testData);

            var wql = Fixture.IndexManager.ExecuteWql<UnitTestIndexTestMockB>("description = 'phasellus'");
            var item = wql.Apply();

            Assert.NotNull(wql);
            Assert.True(item.Any());
        }

        [Fact]
        public void ReIndexTestDataC()
        {
            var itemCount = 1000;
            var wordCount = 1000;
            var vocabulary = 40000;
            var wordLength = 10;

            var testData = UnitTestIndexTestMockC.GenerateTestData(itemCount, wordCount, vocabulary, wordLength).ToList();

            Fixture.IndexManager.Register<UnitTestIndexTestMockC>(CultureInfo.GetCultureInfo("en"), ushort.MaxValue, IndexType.Storage);
            Fixture.IndexManager.ReIndex(testData);
            Fixture.IndexManager.ExecuteWql<UnitTestIndexTestMockC>("Text ~ 'abcdaaaaaa'");
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

                            var testData = UnitTestIndexTestMockC.GenerateTestData(i, w, v, l);

                            Fixture.IndexManager.Register<UnitTestIndexTestMockC>(CultureInfo.GetCultureInfo("en"), (uint)i, IndexType.Storage);

                            // preparing for a measurement
                            stopWatch.Start();

                            //Fixture.IndexManager.Add(testData.FirstOrDefault());
                            Fixture.IndexManager.ReIndex(testData);

                            // stop measurement
                            var elapsedReindex = stopWatch.Elapsed;
                            stopWatch.Reset();

                            var stat = Fixture
                                .IndexManager
                                .GetIndexDocument<UnitTestIndexTestMockC>()
                                .GetReverseIndex(typeof(UnitTestIndexTestMockC).GetProperty("Text"));

                            var output = $"{i};{w};{v};{l};{elapsedReindex:hh\\:mm\\:ss};{stat}";

                            Output.WriteLine(output);
                            file.WriteLine(output);

                            Fixture.IndexManager.Remove<UnitTestIndexTestMockC>();

                            file.Flush();
                        }
                    }
                }
            }
        }

        [Fact]
        public void ReIndexTestDataD()
        {
            var testData = UnitTestIndexTestMockD.GenerateTestData();

            Fixture.IndexManager.Register<UnitTestIndexTestMockD>(CultureInfo.GetCultureInfo("en"), ushort.MaxValue, IndexType.Storage);
            Fixture.IndexManager.ReIndex(testData);

            var wql = Fixture.IndexManager.ExecuteWql<UnitTestIndexTestMockD>("firstname = 'Noah' and lastname = 'Smith'");
            var item = wql.Apply();

            Assert.NotNull(wql);
            Assert.True(item.Any());
        }
    }
}
