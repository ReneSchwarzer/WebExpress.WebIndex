using System.Diagnostics;
using System.Globalization;
using WebExpress.WebIndex.Storage;
using Xunit.Abstractions;

namespace WebExpress.WebIndex.Test.Index
{
    [Collection("UnitTestIndexCollectionFixture")]
    public class UnitTestStorageIndex(UnitTestIndexFixture fixture, ITestOutputHelper output)
    {
        public ITestOutputHelper Output { get; private set; } = output;
        protected UnitTestIndexFixture Fixture { get; set; } = fixture;

        [Fact]
        public void CreateDocumentStore()
        {
            var context = new IndexContext();
            var ds = new IndexStorageDocumentStore<UnitTestIndexTestMockA>(context, 5);

            Assert.NotNull(ds);
        }

        [Fact]
        public void AddDocumentStore()
        {
            var context = new IndexContext();
            var ds = new IndexStorageDocumentStore<UnitTestIndexTestMockA>(context, 5);
            var testData = new[]
            {
                new UnitTestIndexTestMockA { Id = new Guid("b2e8a5c3-1f6d-4e7b-9e1f-8c1a9d0f2b4a"), Name = "Hello Helena!"},
                new UnitTestIndexTestMockA { Id = new Guid("c7d8f9e0-3a2b-4c5d-8e6f-9a1b0c2d4e5f"), Name = "Hello Helena, Helge & Helena!"}
            };

            ds.Clear();
            ds.Add(testData[0]);
            ds.Add(testData[1]);

            Assert.NotNull(ds);
        }

        [Fact]
        public void RemoveDocumentStore()
        {
            var context = new IndexContext();
            var ds = new IndexStorageDocumentStore<UnitTestIndexTestMockA>(context, 5);
            var testData = new[]
            {
                new UnitTestIndexTestMockA { Id = new Guid("b2e8a5c3-1f6d-4e7b-9e1f-8c1a9d0f2b4a"), Name = "Hello Helena!"},
                new UnitTestIndexTestMockA { Id = new Guid("c7d8f9e0-3a2b-4c5d-8e6f-9a1b0c2d4e5f"), Name = "Hello Helena, Helge & Helena!"}
            };

            ds.Clear();
            ds.Add(testData[0]);
            ds.Add(testData[1]);

            var item = ds.GetItem(testData[0].Id);

            Assert.NotNull(ds);
            Assert.NotNull(item);

            ds.Remove(item);

            item = ds.GetItem(testData[0].Id);
            Assert.Null(item);
        }

        [Fact]
        public void RetrieveDocumentStore()
        {
            var context = new IndexContext();
            var ds = new IndexStorageDocumentStore<UnitTestIndexTestMockA>(context, 5);
            var testData = new[]
            {
                new UnitTestIndexTestMockA { Id = new Guid("b2e8a5c3-1f6d-4e7b-9e1f-8c1a9d0f2b4a"), Name = "Hello Helena!"},
                new UnitTestIndexTestMockA { Id = new Guid("c7d8f9e0-3a2b-4c5d-8e6f-9a1b0c2d4e5f"), Name = "Hello Helena, Helge & Helena!"}
            };

            ds.Clear();
            ds.Add(testData[0]);
            ds.Add(testData[1]);

            var item = ds.GetItem(testData[0].Id);

            Assert.NotNull(ds);
            Assert.NotNull(item);
        }

        [Fact]
        public void CreateReverseIndex()
        {
            var context = new IndexContext();
            var property = typeof(UnitTestIndexTestMockA).GetProperty("Name");
            var ds = new IndexStorageReverse<UnitTestIndexTestMockA>(context, property, CultureInfo.GetCultureInfo("en"));

            Assert.NotNull(ds);
        }

        [Fact]
        public void AddReverseIndex()
        {
            var context = new IndexContext();
            var property = typeof(UnitTestIndexTestMockA).GetProperty("Name");
            var ri = new IndexStorageReverse<UnitTestIndexTestMockA>(context, property, CultureInfo.GetCultureInfo("en"));
            var testData = new[]
            {
                new UnitTestIndexTestMockA { Id = new Guid("b2e8a5c3-1f6d-4e7b-9e1f-8c1a9d0f2b4a"), Name = "Hello Helena!"},
                new UnitTestIndexTestMockA { Id = new Guid("c7d8f9e0-3a2b-4c5d-8e6f-9a1b0c2d4e5f"), Name = "Hello Helena, Helge & Helena!"}
            };

            ri.Clear();
            ri.Add(testData[0]);
            ri.Add(testData[1]);

            Assert.NotNull(ri);
        }

        [Fact]
        public void RemoveReverseIndex()
        {
            var context = new IndexContext();
            var property = typeof(UnitTestIndexTestMockA).GetProperty("Name");
            var ri = new IndexStorageReverse<UnitTestIndexTestMockA>(context, property, CultureInfo.GetCultureInfo("en"));
            var testData = new[]
            {
                new UnitTestIndexTestMockA { Id = new Guid("b2e8a5c3-1f6d-4e7b-9e1f-8c1a9d0f2b4a"), Name = "Hello Helena!"},
                new UnitTestIndexTestMockA { Id = new Guid("c7d8f9e0-3a2b-4c5d-8e6f-9a1b0c2d4e5f"), Name = "Hello Helena, Helge & Helena!"}
            };

            ri.Clear();
            ri.Add(testData[0]);
            ri.Add(testData[1]);

            var items = ri.Collect("Helena");

            Assert.NotNull(ri);
            Assert.Equal(2, items.Count());

            ri.Remove(testData[1]);

            items = ri.Collect("Helena");
            Assert.Single(items);
        }

        [Fact]
        public void RetrieveReverseIndex()
        {
            var context = new IndexContext();
            var property = typeof(UnitTestIndexTestMockA).GetProperty("Name");
            var ri = new IndexStorageReverse<UnitTestIndexTestMockA>(context, property, CultureInfo.GetCultureInfo("en"));
            var testData = new[]
            {
                new UnitTestIndexTestMockA { Id = new Guid("b2e8a5c3-1f6d-4e7b-9e1f-8c1a9d0f2b4a"), Name = "Hello Helena!"},
                new UnitTestIndexTestMockA { Id = new Guid("c7d8f9e0-3a2b-4c5d-8e6f-9a1b0c2d4e5f"), Name = "Hello Helena, Helge & Helena!"}
            };

            ri.Clear();
            ri.Add(testData[0]);
            ri.Add(testData[1]);

            var items = ri.Collect("Helena");

            Assert.NotNull(ri);
            Assert.Equal(2, items.Count());
        }

        [Fact]
        public void RegisterTestDataA()
        {
            Fixture.IndexManager.Register<UnitTestIndexTestMockA>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);

            Assert.NotNull(Fixture.IndexManager.GetIndexDocument<UnitTestIndexTestMockA>());
        }

        [Fact]
        public void ClearTestDataA()
        {
            var testData = UnitTestIndexTestMockA.GenerateTestData();

            Fixture.IndexManager.Register<UnitTestIndexTestMockA>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);
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

            Fixture.IndexManager.Register<UnitTestIndexTestMockA>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);
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

            Fixture.IndexManager.Register<UnitTestIndexTestMockA>(CultureInfo.GetCultureInfo("de"), IndexType.Storage);
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

            Fixture.IndexManager.Register<UnitTestIndexTestMockA>(CultureInfo.GetCultureInfo("de-DE"), IndexType.Storage);
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

            Fixture.IndexManager.Register<UnitTestIndexTestMockA>(CultureInfo.GetCultureInfo("fr"), IndexType.Storage);
            Fixture.IndexManager.ReIndex(testData);

            var wql = Fixture.IndexManager.ExecuteWql<UnitTestIndexTestMockA>("name = 'Noah'");
            var item = wql.Apply();

            Assert.NotNull(wql);
            Assert.Equal(2, item.Count());
        }

        //[Fact]
        //public void ReIndexTestDataB()
        //{
        //    var testData = UnitTestIndexTestMockB.GenerateTestData();

        //    Fixture.IndexManager.Register<UnitTestIndexTestMockB>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);
        //    Fixture.IndexManager.ReIndex(testData);

        //    var wql = Fixture.IndexManager.ExecuteWql<UnitTestIndexTestMockB>("description = 'phasellus'");
        //    var item = wql.Apply();

        //    Assert.NotNull(wql);
        //    Assert.True(item.Any());
        //}

        [Fact]
        public void ReIndexTestDataC()
        {
            var itemCount = 1000;
            var wordCount = 1000;
            var vocabulary = 40000;
            var wordLength = 10;

            var testData = UnitTestIndexTestMockC.GenerateTestData(itemCount, wordCount, vocabulary, wordLength).ToList();

            Fixture.IndexManager.Register<UnitTestIndexTestMockC>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);
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

                            Fixture.IndexManager.Register<UnitTestIndexTestMockC>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);

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

            Fixture.IndexManager.Register<UnitTestIndexTestMockD>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);
            Fixture.IndexManager.ReIndex(testData);

            var wql = Fixture.IndexManager.ExecuteWql<UnitTestIndexTestMockD>("firstname = 'Noah' and lastname = 'Smith'");
            var item = wql.Apply();

            Assert.NotNull(wql);
            Assert.True(item.Any());
        }

        [Fact]
        public void RemoveTestDataA_Noah()
        {
            var testData = UnitTestIndexTestMockA.GenerateTestData();

            Fixture.IndexManager.Register<UnitTestIndexTestMockA>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);
            Fixture.IndexManager.ReIndex(testData);

            var wql = Fixture.IndexManager.ExecuteWql<UnitTestIndexTestMockA>("name = 'Noah'");
            var item = wql.Apply();

            Assert.NotNull(wql);
            Assert.Equal(2, item.Count());

            Fixture.IndexManager.Remove(testData.Where(x => x.Name == "Noah").FirstOrDefault());

            wql = Fixture.IndexManager.ExecuteWql<UnitTestIndexTestMockA>("name = 'Noah'");
            item = wql.Apply();

            Assert.NotNull(wql);
            Assert.Equal(1, item.Count());
        }

        [Fact]
        public void RemoveTestDataA_Ines()
        {
            var testData = UnitTestIndexTestMockA.GenerateTestData();

            Fixture.IndexManager.Register<UnitTestIndexTestMockA>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);
            Fixture.IndexManager.ReIndex(testData);

            var wql = Fixture.IndexManager.ExecuteWql<UnitTestIndexTestMockA>("name = 'Ines'");
            var item = wql.Apply();

            Assert.NotNull(wql);
            Assert.Equal(1, item.Count());

            Fixture.IndexManager.Remove(testData.Where(x => x.Name == "Ines").FirstOrDefault());

            wql = Fixture.IndexManager.ExecuteWql<UnitTestIndexTestMockA>("name = 'Ines'");
            item = wql.Apply();

            Assert.NotNull(wql);
            Assert.Equal(0, item.Count());
        }

        [Fact]
        public void UpdateTestDataA()
        {
            var testData = new List<UnitTestIndexTestMockA>
            {
                new()
                {
                    Id = Guid.Parse("ED242C79-E41B-4214-BFBC-C4673E87433B"),
                    Name = "Noah"
                }
            };

            Fixture.IndexManager.Register<UnitTestIndexTestMockA>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);
            Fixture.IndexManager.ReIndex(testData);
            Fixture.IndexManager.Update(new UnitTestIndexTestMockA()
            {
                Id = Guid.Parse("ED242C79-E41B-4214-BFBC-C4673E87433B"),
                Name = "Aurora"
            });

            var wql = Fixture.IndexManager.ExecuteWql<UnitTestIndexTestMockA>("name = 'Aurora'");
            var item = wql.Apply();

            Assert.NotNull(wql);
            Assert.Equal(1, item.Count());
        }
    }
}
