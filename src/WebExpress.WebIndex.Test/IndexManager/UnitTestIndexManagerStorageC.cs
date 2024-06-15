using System.Diagnostics;
using System.Globalization;
using WebExpress.WebIndex.Storage;
using WebExpress.WebIndex.Test.Document;
using WebExpress.WebIndex.Test.Fixture;
using Xunit.Abstractions;

namespace WebExpress.WebIndex.Test.IndexManager
{
    /// <summary>
    /// Test class for testing the storage-based index manager.
    /// </summary>
    /// <param name="fixture">The log.</param>
    /// <param name="output">The test context.</param>
    public class UnitTestIndexManagerStorageC(UnitTestIndexFixtureIndexC fixture, ITestOutputHelper output) : UnitTestIndexManager<UnitTestIndexFixtureIndexC>(fixture, output)
    {
        /// <summary>
        /// Tests registering a document in the index manager.
        /// </summary>
        [Fact]
        public void Create()
        {
            // preconditions
            Preconditions();

            // test execution
            IndexManager.Create<UnitTestIndexTestDocumentC>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);

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
            IndexManager.Create<UnitTestIndexTestDocumentC>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);

            // test execution
            IndexManager.ReIndex(Fixture.TestData);

            var wql = IndexManager.Retrieve<UnitTestIndexTestDocumentC>($"text = '{randomItem.Text.Split(' ').FirstOrDefault()}'");
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
        public async Task ReIndexAsync_En()
        {
            // preconditions
            Preconditions();
            var randomItem = Fixture.RandomItem;
            IndexManager.Create<UnitTestIndexTestDocumentC>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);

            // test execution
            await IndexManager.ReIndexAsync(Fixture.TestData);

            var wql = IndexManager.Retrieve<UnitTestIndexTestDocumentC>($"text = '{randomItem.Text}'");
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
            IndexManager.Create<UnitTestIndexTestDocumentC>(CultureInfo.GetCultureInfo("de"), IndexType.Storage);

            // test execution
            IndexManager.ReIndex(Fixture.TestData);

            var wql = IndexManager.Retrieve<UnitTestIndexTestDocumentC>($"text = '{randomItem.Text.Split(' ').FirstOrDefault()}'");
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
            IndexManager.Create<UnitTestIndexTestDocumentC>(CultureInfo.GetCultureInfo("de-DE"), IndexType.Storage);

            // test execution
            IndexManager.ReIndex(Fixture.TestData);

            var wql = IndexManager.Retrieve<UnitTestIndexTestDocumentC>($"text = '{randomItem.Text.Split(' ').FirstOrDefault()}'");
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
            IndexManager.Create<UnitTestIndexTestDocumentC>(CultureInfo.GetCultureInfo("fr"), IndexType.Storage);

            // test execution
            IndexManager.ReIndex(Fixture.TestData);

            var wql = IndexManager.Retrieve<UnitTestIndexTestDocumentC>($"text = '{randomItem.Text.Split(' ').FirstOrDefault()}'");
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
        public void ReIndex_Series()
        {
            var stopWatch = new Stopwatch();

            var itemCount = Enumerable.Range(1, 1).Select(x => x * 10000);
            var wordCount = Enumerable.Range(1, 1).Select(x => x * 100);
            var vocabulary = Enumerable.Range(1, 1).Select(x => x * 20000);
            var wordLength = Enumerable.Range(1, 1).Select(x => x * 15);
            var maxCachedSegments = Enumerable.Range(5, 1).Select(x => x * 10000);
            var bufferSize = Enumerable.Range(12, 1).Select(x => Math.Pow(2, x));
            var path = Path.Combine(Environment.CurrentDirectory, "storage-reindex_series.csv");
            var exists = File.Exists(path);

            if (!exists)
            {
                File.AppendAllText(path, "timestamp;item count;wordCount;vocabulary;wordLength;max cached segments;buffer size;elapsed reindex [hh:mm:ss];elapsed retrieval [ms];size of document store [MB];size of reverse index [MB];∑ storage space [MB];size of process mem [MB]" + Environment.NewLine);
            }

            foreach (var w in wordCount)
            {
                foreach (var i in itemCount)
                {
                    foreach (var v in vocabulary)
                    {
                        foreach (var l in wordLength)
                        {
                            var data = UnitTestIndexTestDocumentFactoryC.GenerateTestData(i, w, v, l);
                            var randomItem = default(UnitTestIndexTestDocumentC);
                            var mem = Fixture.GetUsedMemory();

                            foreach (var m in maxCachedSegments)
                            {
                                foreach (var b in bufferSize)
                                {
                                    // preconditions
                                    IndexStorageBuffer.MaxCachedSegments = (uint)m;
                                    IndexStorageFile.BufferSize = (uint)b;

                                    Preconditions();
                                    var output = "";

                                    IndexManager.Create<UnitTestIndexTestDocumentC>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);
                                    IndexManager.Retrieve<UnitTestIndexTestDocumentC>($"text = 'xyz'").Apply();

                                    try
                                    {
                                        // preparing for a measurement
                                        stopWatch.Start();

                                        // test execution
                                        IndexManager.ReIndex(data);

                                        // stop measurement
                                        var elapsedReindex = stopWatch.Elapsed;
                                        stopWatch.Reset();

                                        randomItem ??= IndexManager.All<UnitTestIndexTestDocumentC>().Skip(new Random().Next() % data.Count()).FirstOrDefault();
                                        var wql = IndexManager.Retrieve<UnitTestIndexTestDocumentC>($"text ~ '{randomItem.Text.Split(' ').FirstOrDefault()}'");
                                        Assert.NotNull(wql);

                                        // preparing for a measurement
                                        stopWatch.Start();

                                        var item = wql.Apply();

                                        // stop measurement
                                        var elapsedRetrieval = stopWatch.Elapsed;
                                        stopWatch.Reset();

                                        Assert.NotEmpty(item);

                                        IndexManager.Dispose();

                                        var documentStoreSize = new DirectoryInfo(IndexManager.Context.IndexDirectory).GetFiles("*.wds", SearchOption.AllDirectories).Sum(file => file.Length);
                                        var reverseIndexSize = new DirectoryInfo(IndexManager.Context.IndexDirectory).GetFiles("*.wri", SearchOption.AllDirectories).Sum(file => file.Length);

                                        output = $"{DateTime.Now};{i};{w};{v};{l};{m};{b};{elapsedReindex:hh\\:mm\\:ss};{(int)Math.Ceiling(elapsedRetrieval.TotalMilliseconds)};{Math.Round((double)documentStoreSize / 1024 / 1024, 2)};{Math.Round((double)reverseIndexSize / 1024 / 1024, 2)};{Math.Round((double)(documentStoreSize + reverseIndexSize) / 1024 / 1024, 2)};{Fixture.GetUsedMemory() - mem}";
                                    }
                                    catch (Exception ex)
                                    {
                                        Output.WriteLine(ex.Message + " " + ex.StackTrace);
                                        throw;
                                    }
                                    finally
                                    {
                                        // postconditions
                                        File.AppendAllText(path, output + Environment.NewLine);
                                        Postconditions();
                                    }

                                    Thread.Sleep(5000);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Tests the reindex function in a series of tests from the index manager.
        /// </summary>
        [Fact]
        public async Task ReIndexAsync_Series()
        {
            var stopWatch = new Stopwatch();

            var itemCount = Enumerable.Range(1, 1).Select(x => x * 10000);
            var wordCount = Enumerable.Range(1, 1).Select(x => x * 100);
            var vocabulary = Enumerable.Range(1, 1).Select(x => x * 20000);
            var wordLength = Enumerable.Range(1, 1).Select(x => x * 15);
            var maxCachedSegments = Enumerable.Range(5, 1).Select(x => x * 10000);
            var bufferSize = Enumerable.Range(12, 1).Select(x => Math.Pow(2, x));
            var path = Path.Combine(Environment.CurrentDirectory, "storage-reindexasync_series.csv");
            var exists = File.Exists(path);

            if (!exists)
            {
                File.AppendAllText(path, "timestamp;item count;wordCount;vocabulary;wordLength;max cached segments;buffer size;elapsed reindex [hh:mm:ss];elapsed retrieval [ms];size of document store [MB];size of reverse index [MB];∑ storage space [MB];size of process mem [MB]" + Environment.NewLine);
            }

            foreach (var w in wordCount)
            {
                foreach (var i in itemCount)
                {
                    foreach (var v in vocabulary)
                    {
                        foreach (var l in wordLength)
                        {
                            var data = UnitTestIndexTestDocumentFactoryC.GenerateTestData(i, w, v, l);
                            var randomItem = default(UnitTestIndexTestDocumentC);
                            var mem = Fixture.GetUsedMemory();

                            foreach (var m in maxCachedSegments)
                            {
                                foreach (var b in bufferSize)
                                {
                                    // preconditions
                                    IndexStorageBuffer.MaxCachedSegments = (uint)m;
                                    IndexStorageFile.BufferSize = (uint)b;

                                    Preconditions();
                                    var output = "";

                                    IndexManager.Create<UnitTestIndexTestDocumentC>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);
                                    IndexManager.Retrieve<UnitTestIndexTestDocumentC>($"text = 'xyz'").Apply();

                                    try
                                    {
                                        // preparing for a measurement
                                        stopWatch.Start();

                                        // test execution
                                        await IndexManager.ReIndexAsync(data);

                                        // stop measurement
                                        var elapsedReindex = stopWatch.Elapsed;
                                        stopWatch.Reset();

                                        randomItem ??= IndexManager.All<UnitTestIndexTestDocumentC>().Skip(new Random().Next() % data.Count()).FirstOrDefault();
                                        var wql = IndexManager.Retrieve<UnitTestIndexTestDocumentC>($"text ~ '{randomItem.Text.Split(' ').FirstOrDefault()}'");
                                        Assert.NotNull(wql);

                                        // preparing for a measurement
                                        stopWatch.Start();

                                        var item = wql.Apply();

                                        // stop measurement
                                        var elapsedRetrieval = stopWatch.Elapsed;
                                        stopWatch.Reset();

                                        Assert.NotEmpty(item);

                                        IndexManager.Dispose();

                                        var documentStoreSize = new DirectoryInfo(IndexManager.Context.IndexDirectory).GetFiles("*.wds", SearchOption.AllDirectories).Sum(file => file.Length);
                                        var reverseIndexSize = new DirectoryInfo(IndexManager.Context.IndexDirectory).GetFiles("*.wri", SearchOption.AllDirectories).Sum(file => file.Length);

                                        output = $"{DateTime.Now};{i};{w};{v};{l};{m};{b};{elapsedReindex:hh\\:mm\\:ss};{(int)Math.Ceiling(elapsedRetrieval.TotalMilliseconds)};{Math.Round((double)documentStoreSize / 1024 / 1024, 2)};{Math.Round((double)reverseIndexSize / 1024 / 1024, 2)};{Math.Round((double)(documentStoreSize + reverseIndexSize) / 1024 / 1024, 2)};{Fixture.GetUsedMemory() - mem}";
                                    }
                                    catch (Exception ex)
                                    {
                                        Output.WriteLine(ex.Message + " " + ex.StackTrace);
                                        throw;
                                    }
                                    finally
                                    {
                                        // postconditions
                                        File.AppendAllText(path, output + Environment.NewLine);
                                        Postconditions();
                                    }

                                    Thread.Sleep(5000);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Tests the removal of a document from the index manager.
        /// </summary>
        [Fact]
        public void Delete()
        {
            // preconditions
            Preconditions();
            var randomItem = Fixture.RandomItem;
            IndexManager.Create<UnitTestIndexTestDocumentC>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);
            IndexManager.ReIndex(Fixture.TestData);

            var wql = IndexManager.Retrieve<UnitTestIndexTestDocumentC>($"text = '{randomItem.Text.Split(' ').FirstOrDefault()}'");
            Assert.NotNull(wql);

            var before = wql.Apply().ToList();
            Assert.NotEmpty(before);

            // test execution
            IndexManager.Delete(randomItem);

            wql = IndexManager.Retrieve<UnitTestIndexTestDocumentC>($"text = '{randomItem.Text.Split(' ').FirstOrDefault()}'");
            Assert.NotNull(wql);

            var after = wql.Apply().ToList();
            Assert.Equal(before.Count - 1, after.Count);

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
            IndexManager.Create<UnitTestIndexTestDocumentC>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);
            IndexManager.ReIndex(Fixture.TestData);

            // test execution
            IndexManager.Insert(new UnitTestIndexTestDocumentC()
            {
                Id = Guid.Parse("ED242C79-E41B-4214-BFBC-C4673E87433B"),
                Text = "Aurora"
            });

            var wql = IndexManager.Retrieve<UnitTestIndexTestDocumentC>("text = 'Aurora'");
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
            IndexManager.Create<UnitTestIndexTestDocumentC>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);
            IndexManager.ReIndex(Fixture.TestData);

            // test execution
            IndexManager.Update(new UnitTestIndexTestDocumentC()
            {
                Id = randomItem.Id,
                Text = "Aurora"
            });

            var wql = IndexManager.Retrieve<UnitTestIndexTestDocumentC>("text = 'Aurora'");
            Assert.NotNull(wql);

            var item = wql.Apply();
            Assert.Equal(1, item.Count());

            // postconditions
            Postconditions();
        }

        /// <summary>
        /// Tests the update function of the index manager.
        /// </summary>
        [Fact]
        public async Task UpdateAsync()
        {
            // preconditions
            Preconditions();
            var randomItem = Fixture.RandomItem;
            IndexManager.Create<UnitTestIndexTestDocumentC>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);
            await IndexManager.ReIndexAsync(Fixture.TestData);

            // test execution
            await IndexManager.UpdateAsync(new UnitTestIndexTestDocumentC()
            {
                Id = randomItem.Id,
                Text = "Aurora"
            });

            var wql = IndexManager.Retrieve<UnitTestIndexTestDocumentC>("text = 'Aurora'");
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
            IndexManager.Create<UnitTestIndexTestDocumentC>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);
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
            IndexManager.Create<UnitTestIndexTestDocumentC>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);
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
            IndexManager.Create<UnitTestIndexTestDocumentA>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);
            IndexManager.Create<UnitTestIndexTestDocumentB>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);
            IndexManager.Create<UnitTestIndexTestDocumentC>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);
            IndexManager.Create<UnitTestIndexTestDocumentD>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);
            IndexManager.Create<UnitTestIndexTestDocumentE>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);

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
            IndexManager.Create<UnitTestIndexTestDocumentA>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);
            IndexManager.Create<UnitTestIndexTestDocumentB>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);
            IndexManager.Create<UnitTestIndexTestDocumentD>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);
            IndexManager.Create<UnitTestIndexTestDocumentE>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);

            // test execution
            var document = IndexManager.GetIndexDocument<UnitTestIndexTestDocumentC>();
            Assert.Null(document);

            // postconditions
            Postconditions();
        }
    }
}
