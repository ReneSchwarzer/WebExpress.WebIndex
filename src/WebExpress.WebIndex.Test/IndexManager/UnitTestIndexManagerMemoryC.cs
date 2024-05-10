﻿using System.Globalization;
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
        //[Fact]
        //public void ReIndex_Series()
        //{
        //    // preconditions
        //    var stopWatch = new Stopwatch();
        //    var file = File.CreateText(Path.Combine(Environment.CurrentDirectory, "index-memory-reindexseries-c.csv"));
        //    var indexDict = Path.Combine(Environment.CurrentDirectory, "index");

        //    var itemCount = Enumerable.Range(1, 1).Select(x => x * 1000);
        //    var wordCount = new int[] { 1000 };     // new int[] { 100, 1000 };
        //    var vocabulary = new int[] { 40000 };   // new int[] { 10000, 20000, 30000, 40000, 50000, 60000, 70000 };
        //    var wordLength = new int[] { 10 };      // new int[] { 10, 20, 30, 40, 50 };

        //    var context = new IndexContext();
        //    Fixture.IndexManager.Initialization(context);

        //    var heading = "item count;wordCount;vocabulary;wordLength;elapsed reindex;";
        //    Output.WriteLine(heading);
        //    file.WriteLine(heading);

        //    foreach (var w in wordCount)
        //    {
        //        foreach (var i in itemCount)
        //        {
        //            foreach (var v in vocabulary)
        //            {
        //                foreach (var l in wordLength)
        //                {
        //                    // remove the index files if exists
        //                    if (Directory.Exists(indexDict))
        //                    {
        //                        Directory.Delete(indexDict, true);
        //                    }

        //                    var data = UnitTestIndexTestDocumentFactoryC.GenerateTestData(i, w, v, l);

        //                    Fixture.IndexManager.Register<UnitTestIndexTestDocumentC>(CultureInfo.GetCultureInfo("en"), IndexType.Memory);

        //                    // preparing for a measurement
        //                    stopWatch.Start();

        //                    // test execution
        //                    Fixture.IndexManager.ReIndex(data);

        //                    // stop measurement
        //                    var elapsedReindex = stopWatch.Elapsed;
        //                    stopWatch.Reset();

        //                    var stat = Fixture
        //                        .IndexManager
        //                        .GetIndexDocument<UnitTestIndexTestDocumentC>()
        //                        .GetReverseIndex(typeof(UnitTestIndexTestDocumentC).GetProperty("Text"));

        //                    var output = $"{i};{w};{v};{l};{elapsedReindex:hh\\:mm\\:ss};{stat}";

        //                    Output.WriteLine(output);
        //                    file.WriteLine(output);
        //                    file.Flush();

        //                    // postconditions
        //                    Fixture.IndexManager.Dispose();
        //                }
        //            }
        //        }
        //    }
        //}

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