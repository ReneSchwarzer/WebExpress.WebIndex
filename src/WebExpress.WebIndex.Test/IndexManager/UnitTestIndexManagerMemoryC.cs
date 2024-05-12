using System.Diagnostics;
using System.Globalization;
using WebExpress.WebIndex.Test.Document;
using WebExpress.WebIndex.Test.Fixture;
using Xunit.Abstractions;

namespace WebExpress.WebIndex.Test.IndexManager
{
    /// <summary>
    /// Test class for testing the memory-based index manager.
    /// </summary>
    public class UnitTestIndexManagerMemoryC : UnitTestIndexManager<UnitTestIndexFixtureIndexC>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fixture">The log.</param>
        /// <param name="output">The test context.</param>
        public UnitTestIndexManagerMemoryC(UnitTestIndexFixtureIndexC fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
        }

        /// <summary>
        /// Tests registering a document in the index manager.
        /// </summary>
        [Fact]
        public void Register()
        {
            // preconditions
            Preconditions();

            // test execution
            IndexManager.Register<UnitTestIndexTestDocumentC>(CultureInfo.GetCultureInfo("en"), IndexType.Memory);

            Assert.NotNull(IndexManager.GetIndexDocument<UnitTestIndexTestDocumentC>());

            // postconditions
            Postconditions();
        }

        /// <summary>
        /// Tests the reindex function from the index manager.
        /// </summary>
        [Fact]
        public void ReIndex_En()
        {
            // preconditions
            Preconditions();
            var randomItem = Fixture.RandomItem;
            IndexManager.Register<UnitTestIndexTestDocumentC>(CultureInfo.GetCultureInfo("en"), IndexType.Memory);

            // test execution
            IndexManager.ReIndex(Fixture.TestData);

            var wql = IndexManager.ExecuteWql<UnitTestIndexTestDocumentC>($"text = '{randomItem.Text}'");
            Assert.NotNull(wql);

            var item = wql.Apply();
            Assert.NotEmpty(item);

            // postconditions
            Postconditions();
        }

        /// <summary>
        /// Tests the reindex function from the index manager.
        /// </summary>
        [Fact]
        public async void ReIndexAsync_En()
        {
            // preconditions
            Preconditions();
            var randomItem = Fixture.RandomItem;
            IndexManager.Register<UnitTestIndexTestDocumentC>(CultureInfo.GetCultureInfo("en"), IndexType.Memory);

            // test execution
            await IndexManager.ReIndexAsync(Fixture.TestData);

            var wql = IndexManager.ExecuteWql<UnitTestIndexTestDocumentC>($"text = '{randomItem.Text}'");
            Assert.NotNull(wql);

            var item = wql.Apply();
            Assert.NotEmpty(item);

            // postconditions
            Postconditions();
        }

        /// <summary>
        /// Tests the reindex function from the index manager.
        /// </summary>
        [Fact]
        public void ReIndex_De()
        {
            // preconditions
            Preconditions();
            var randomItem = Fixture.RandomItem;
            IndexManager.Register<UnitTestIndexTestDocumentC>(CultureInfo.GetCultureInfo("de"), IndexType.Memory);

            // test execution
            IndexManager.ReIndex(Fixture.TestData);

            var wql = IndexManager.ExecuteWql<UnitTestIndexTestDocumentC>($"text = '{randomItem.Text}'");
            Assert.NotNull(wql);

            var item = wql.Apply();
            Assert.NotEmpty(item);

            // postconditions
            Postconditions();
        }

        /// <summary>
        /// Tests the reindex function from the index manager.
        /// </summary>
        [Fact]
        public void ReIndex_DeDE()
        {
            // preconditions
            Preconditions();
            var randomItem = Fixture.RandomItem;
            IndexManager.Register<UnitTestIndexTestDocumentC>(CultureInfo.GetCultureInfo("de-DE"), IndexType.Memory);

            // test execution
            IndexManager.ReIndex(Fixture.TestData);

            var wql = IndexManager.ExecuteWql<UnitTestIndexTestDocumentC>($"text = '{randomItem.Text}'");
            Assert.NotNull(wql);

            var item = wql.Apply();
            Assert.NotEmpty(item);

            // postconditions
            Postconditions();
        }

        /// <summary>
        /// Tests the reindex function from the index manager.
        /// </summary>
        [Fact]
        public void ReIndex_Fr()
        {
            // preconditions
            Preconditions();
            var randomItem = Fixture.RandomItem;
            IndexManager.Register<UnitTestIndexTestDocumentC>(CultureInfo.GetCultureInfo("fr"), IndexType.Memory);

            // test execution
            IndexManager.ReIndex(Fixture.TestData);

            var wql = IndexManager.ExecuteWql<UnitTestIndexTestDocumentC>($"text = '{randomItem.Text}'");
            Assert.NotNull(wql);

            var item = wql.Apply();
            Assert.NotEmpty(item);

            // postconditions
            Postconditions();
        }

        /// <summary>
        /// Tests the reindex function in a series of tests from the index manager.
        /// </summary>
        [Fact]
        public async void ReIndexAsync_Series()
        {
            var stopWatch = new Stopwatch();

            var itemCount = Enumerable.Range(1, 10).Select(x => x * 1000);
            var wordCount = Enumerable.Range(1, 1).Select(x => x * 1000);
            var vocabulary = Enumerable.Range(1, 1).Select(x => x * 2000);
            var wordLength = Enumerable.Range(1, 1).Select(x => x * 5);

            Output.WriteLine("item count;wordCount;vocabulary;wordLength;elapsed reindex [hh:mm:ss];elapsed retrieval [ms];size of document store [MB];size of reverse index [MB]");

            foreach (var w in wordCount)
            {
                foreach (var i in itemCount)
                {
                    foreach (var v in vocabulary)
                    {
                        foreach (var l in wordLength)
                        {
                            // disabled due to long execution time. activate if necessary.
                            /*
                            // preconditions
                            Preconditions();
                            var output = "";
                            var data = UnitTestIndexTestDocumentFactoryC.GenerateTestData(i, w, v, l);
                            var randomItem = data.Skip(new Random().Next() % data.Count()).FirstOrDefault();

                            IndexManager.Register<UnitTestIndexTestDocumentC>(CultureInfo.GetCultureInfo("en"), IndexType.Memory);

                            try
                            {
                                // preparing for a measurement
                                stopWatch.Start();

                                // test execution
                                await IndexManager.ReIndexAsync(data);

                                // stop measurement
                                var elapsedReindex = stopWatch.Elapsed;
                                stopWatch.Reset();

                                var wql = IndexManager.ExecuteWql<UnitTestIndexTestDocumentC>($"text = '{randomItem.Text.Split(' ').FirstOrDefault()}'");
                                Assert.NotNull(wql);

                                // preparing for a measurement
                                stopWatch.Start();

                                var item = wql.Apply();
                                Assert.NotEmpty(item);

                                // stop measurement
                                var elapsedRetrieval = stopWatch.Elapsed;
                                stopWatch.Reset();

                                var documentStoreSize = new DirectoryInfo(IndexManager.Context.IndexDirectory).GetFiles("*.wds", SearchOption.AllDirectories).Sum(file => file.Length);
                                var reverseIndexSize = new DirectoryInfo(IndexManager.Context.IndexDirectory).GetFiles("*.wri", SearchOption.AllDirectories).Sum(file => file.Length);

                                output = $"{i};{w};{v};{l};{elapsedReindex:hh\\:mm\\:ss};{elapsedRetrieval.TotalMilliseconds};{Math.Round((double)documentStoreSize / 1024 / 1024, 2)};{Math.Round((double)reverseIndexSize / 1024 / 1024, 2)}";
                            }
                            catch (Exception ex)
                            {
                                output += ex.Message;
                            }
                            finally
                            {
                                // postconditions
                                Output.WriteLine(output);
                                Postconditions();
                            }
                            /**/
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Tests the removal of a document from the index manager.
        /// </summary>
        [Fact]
        public void Remove()
        {
            // preconditions
            Preconditions();
            var randomItem = Fixture.RandomItem;
            IndexManager.Register<UnitTestIndexTestDocumentC>(CultureInfo.GetCultureInfo("en"), IndexType.Memory);
            IndexManager.ReIndex(Fixture.TestData);

            var wql = IndexManager.ExecuteWql<UnitTestIndexTestDocumentC>($"text = '{randomItem.Text}'");
            Assert.NotNull(wql);

            var before = wql.Apply().ToList();
            Assert.True(before.Any());

            // test execution
            IndexManager.Remove(randomItem);

            wql = IndexManager.ExecuteWql<UnitTestIndexTestDocumentC>($"text = '{randomItem.Text}'");
            Assert.NotNull(wql);

            var after = wql.Apply().ToList();
            Assert.True(before.Count - 1 == after.Count);

            // postconditions
            Postconditions();
        }

        /// <summary>
        /// Tests the add function of the index manager.
        /// </summary>
        [Fact]
        public void Add()
        {
            // preconditions
            Preconditions();
            IndexManager.Register<UnitTestIndexTestDocumentC>(CultureInfo.GetCultureInfo("en"), IndexType.Memory);
            IndexManager.ReIndex(Fixture.TestData);

            // test execution
            IndexManager.Add(new UnitTestIndexTestDocumentC()
            {
                Id = Guid.Parse("ED242C79-E41B-4214-BFBC-C4673E87433B"),
                Text = "Hello Aurora!"
            });

            var wql = IndexManager.ExecuteWql<UnitTestIndexTestDocumentC>("text = 'Aurora'");
            var item = wql.Apply();

            Assert.NotNull(wql);
            Assert.Equal(1, item.Count());

            // postconditions
            Postconditions();
        }

        /// <summary>
        /// Tests the update function of the index manager.
        /// </summary>
        [Fact]
        public void Update()
        {
            // preconditions
            Preconditions();
            var randomItem = Fixture.RandomItem;
            IndexManager.Register<UnitTestIndexTestDocumentC>(CultureInfo.GetCultureInfo("en"), IndexType.Memory);
            IndexManager.ReIndex(Fixture.TestData);

            // test execution
            IndexManager.Update(new UnitTestIndexTestDocumentC()
            {
                Id = randomItem.Id,
                Text = "Aurora"
            });

            var wql = IndexManager.ExecuteWql<UnitTestIndexTestDocumentC>("text = 'Aurora'");
            var item = wql.Apply();

            Assert.NotNull(wql);
            Assert.Equal(1, item.Count());

            // postconditions
            Postconditions();
        }

        /// <summary>
        /// Tests the update function of the index manager.
        /// </summary>
        [Fact]
        public async void UpdateAsync()
        {
            // preconditions
            Preconditions();
            var randomItem = Fixture.RandomItem;
            IndexManager.Register<UnitTestIndexTestDocumentC>(CultureInfo.GetCultureInfo("en"), IndexType.Memory);
            await IndexManager.ReIndexAsync(Fixture.TestData);

            // test execution
            await IndexManager.UpdateAsync(new UnitTestIndexTestDocumentC()
            {
                Id = randomItem.Id,
                Text = "Aurora"
            });

            var wql = IndexManager.ExecuteWql<UnitTestIndexTestDocumentC>("text = 'Aurora'");
            Assert.NotNull(wql);

            var item = wql.Apply();
            Assert.Equal(1, item.Count());

            // postconditions
            Postconditions();
        }

        /// <summary>
        /// Tests removing a document on the index manager.
        /// </summary>
        [Fact]
        public void Clear()
        {
            // preconditions
            Preconditions();
            IndexManager.Register<UnitTestIndexTestDocumentC>(CultureInfo.GetCultureInfo("en"), IndexType.Memory);
            IndexManager.ReIndex(Fixture.TestData);

            var documents = IndexManager.All<UnitTestIndexTestDocumentC>();

            Assert.NotNull(documents);
            Assert.True(documents.Any());

            // test execution
            IndexManager.Clear<UnitTestIndexTestDocumentC>();

            documents = IndexManager.All<UnitTestIndexTestDocumentC>();

            Assert.NotNull(documents);
            Assert.False(documents.Any());

            // postconditions
            Postconditions();
        }

        /// <summary>
        /// Return all entries of the index manager.
        /// </summary>
        [Fact]
        public void All()
        {
            // preconditions
            Preconditions();
            IndexManager.Register<UnitTestIndexTestDocumentC>(CultureInfo.GetCultureInfo("en"), IndexType.Memory);
            IndexManager.ReIndex(Fixture.TestData);

            // test execution
            var all = IndexManager.All<UnitTestIndexTestDocumentC>();

            Assert.True(all.Select(x => x.Id).OrderBy(x => x).SequenceEqual(Fixture.TestData.Select(x => x.Id).OrderBy(x => x)));

            // postconditions
            Postconditions();
        }

        /// <summary>
        /// Tests get a document from the index manager.
        /// </summary>
        [Fact]
        public void GetDocument()
        {
            // preconditions
            Preconditions();
            IndexManager.Register<UnitTestIndexTestDocumentA>(CultureInfo.GetCultureInfo("en"), IndexType.Memory);
            IndexManager.Register<UnitTestIndexTestDocumentB>(CultureInfo.GetCultureInfo("en"), IndexType.Memory);
            IndexManager.Register<UnitTestIndexTestDocumentC>(CultureInfo.GetCultureInfo("en"), IndexType.Memory);
            IndexManager.Register<UnitTestIndexTestDocumentD>(CultureInfo.GetCultureInfo("en"), IndexType.Memory);
            IndexManager.Register<UnitTestIndexTestDocumentE>(CultureInfo.GetCultureInfo("en"), IndexType.Memory);

            // test execution
            var document = IndexManager.GetIndexDocument<UnitTestIndexTestDocumentC>();
            Assert.NotNull(document);
            Assert.True(document.GetType() == typeof(IndexDocument<UnitTestIndexTestDocumentC>));

            // postconditions
            Postconditions();
        }

        /// <summary>
        /// Tests get a document from the index manager.
        /// </summary>
        [Fact]
        public void GetDocument_Not()
        {
            // preconditions
            Preconditions();
            IndexManager.Register<UnitTestIndexTestDocumentA>(CultureInfo.GetCultureInfo("en"), IndexType.Memory);
            IndexManager.Register<UnitTestIndexTestDocumentB>(CultureInfo.GetCultureInfo("en"), IndexType.Memory);
            IndexManager.Register<UnitTestIndexTestDocumentD>(CultureInfo.GetCultureInfo("en"), IndexType.Memory);
            IndexManager.Register<UnitTestIndexTestDocumentE>(CultureInfo.GetCultureInfo("en"), IndexType.Memory);

            // test execution
            var document = IndexManager.GetIndexDocument<UnitTestIndexTestDocumentC>();
            Assert.Null(document);

            // postconditions
            Postconditions();
        }
    }
}
