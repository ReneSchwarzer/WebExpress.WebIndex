using System.Globalization;
using WebExpress.WebIndex.Test.Document;
using WebExpress.WebIndex.Test.Fixture;
using Xunit.Abstractions;

namespace WebExpress.WebIndex.Test.IndexManager
{
    /// <summary>
    /// Test class for testing the storage-based index manager.
    /// </summary>
    public class UnitTestIndexManagerStorageA : UnitTestIndexManager<UnitTestIndexFixtureIndexA>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fixture">The log.</param>
        /// <param name="output">The test context.</param>
        public UnitTestIndexManagerStorageA(UnitTestIndexFixtureIndexA fixture, ITestOutputHelper output)
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
            IndexManager.Register<UnitTestIndexTestDocumentA>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);

            Assert.NotNull(IndexManager.GetIndexDocument<UnitTestIndexTestDocumentA>());

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
            IndexManager.Register<UnitTestIndexTestDocumentA>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);

            // test execution
            IndexManager.ReIndex(Fixture.TestData);

            var wql = IndexManager.ExecuteWql<UnitTestIndexTestDocumentA>("name = 'Helena'");
            Assert.NotNull(wql);

            var item = wql.Apply();
            Assert.Equal(2, item.Count());

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
            IndexManager.Register<UnitTestIndexTestDocumentA>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);

            // test execution
            await IndexManager.ReIndexAsync(Fixture.TestData);

            var wql = IndexManager.ExecuteWql<UnitTestIndexTestDocumentA>($"text = '{randomItem.Text}'");
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
            IndexManager.Register<UnitTestIndexTestDocumentA>(CultureInfo.GetCultureInfo("de"), IndexType.Storage);

            // test execution
            IndexManager.ReIndex(Fixture.TestData);

            var wql = IndexManager.ExecuteWql<UnitTestIndexTestDocumentA>("name = 'Helena'");
            Assert.NotNull(wql);

            var item = wql.Apply();
            Assert.Equal(2, item.Count());

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
            IndexManager.Register<UnitTestIndexTestDocumentA>(CultureInfo.GetCultureInfo("de-DE"), IndexType.Storage);

            // test execution
            IndexManager.ReIndex(Fixture.TestData);

            var wql = IndexManager.ExecuteWql<UnitTestIndexTestDocumentA>("name = 'Helena'");
            Assert.NotNull(wql);

            var item = wql.Apply();
            Assert.Equal(2, item.Count());

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
            IndexManager.Register<UnitTestIndexTestDocumentA>(CultureInfo.GetCultureInfo("fr"), IndexType.Storage);

            // test execution
            IndexManager.ReIndex(Fixture.TestData);

            var wql = IndexManager.ExecuteWql<UnitTestIndexTestDocumentA>("name = 'Helena'");
            Assert.NotNull(wql);

            var item = wql.Apply();
            Assert.Equal(2, item.Count());

            // postconditions
            Postconditions();
        }

        /// <summary>
        /// Tests the removal of a document from the index manager.
        /// </summary>
        [Fact]
        public void Remove()
        {
            // preconditions
            Preconditions();
            IndexManager.Register<UnitTestIndexTestDocumentA>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);
            IndexManager.ReIndex(Fixture.TestData);

            var wql = IndexManager.ExecuteWql<UnitTestIndexTestDocumentA>("name = 'Helena'");
            Assert.NotNull(wql);

            var item = wql.Apply();
            Assert.Equal(2, item.Count());

            // test execution
            IndexManager.Remove(Fixture.TestData[0]);

            wql = IndexManager.ExecuteWql<UnitTestIndexTestDocumentA>("name = 'Helena'");
            Assert.NotNull(wql);

            item = wql.Apply();
            Assert.Single(item);

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
            IndexManager.Register<UnitTestIndexTestDocumentA>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);
            IndexManager.ReIndex(Fixture.TestData);

            // test execution
            IndexManager.Add(new UnitTestIndexTestDocumentA()
            {
                Id = Guid.Parse("ED242C79-E41B-4214-BFBC-C4673E87433B"),
                Text = "Hello Aurora!"
            });

            var wql = IndexManager.ExecuteWql<UnitTestIndexTestDocumentA>("text = 'Aurora'");
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
            IndexManager.Register<UnitTestIndexTestDocumentA>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);
            IndexManager.ReIndex(Fixture.TestData);

            // test execution
            IndexManager.Update(new UnitTestIndexTestDocumentA()
            {
                Id = Fixture.TestData[1].Id,
                Text = "Hello Helena, hello Aurora!"
            });

            var wql = IndexManager.ExecuteWql<UnitTestIndexTestDocumentA>("text = 'Aurora'");
            Assert.NotNull(wql);

            var item = wql.Apply();
            Assert.Equal(1, item.Count());

            wql = IndexManager.ExecuteWql<UnitTestIndexTestDocumentA>("text = 'Helge'");
            Assert.NotNull(wql);

            item = wql.Apply();
            Assert.Empty(item);

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
            IndexManager.Register<UnitTestIndexTestDocumentA>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);
            await IndexManager.ReIndexAsync(Fixture.TestData);

            // test execution
            await IndexManager.UpdateAsync(new UnitTestIndexTestDocumentA()
            {
                Id = randomItem.Id,
                Text = "Hello Helena, hello Aurora!"
            });

            var wql = IndexManager.ExecuteWql<UnitTestIndexTestDocumentA>("text = 'Aurora'");
            Assert.NotNull(wql);

            var item = wql.Apply();
            Assert.Equal(1, item.Count());

            wql = IndexManager.ExecuteWql<UnitTestIndexTestDocumentA>("text = 'Helge'");
            Assert.NotNull(wql);

            item = wql.Apply();
            Assert.Empty(item);

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
            IndexManager.Register<UnitTestIndexTestDocumentA>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);
            IndexManager.ReIndex(Fixture.TestData);

            var documents = IndexManager.All<UnitTestIndexTestDocumentA>();

            Assert.NotNull(documents);
            Assert.True(documents.Any());

            // test execution
            IndexManager.Clear<UnitTestIndexTestDocumentA>();

            documents = IndexManager.All<UnitTestIndexTestDocumentA>();

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
            IndexManager.Register<UnitTestIndexTestDocumentA>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);
            IndexManager.ReIndex(Fixture.TestData);

            // test execution
            var all = IndexManager.All<UnitTestIndexTestDocumentA>();

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
            IndexManager.Register<UnitTestIndexTestDocumentA>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);
            IndexManager.Register<UnitTestIndexTestDocumentB>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);
            IndexManager.Register<UnitTestIndexTestDocumentC>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);
            IndexManager.Register<UnitTestIndexTestDocumentD>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);
            IndexManager.Register<UnitTestIndexTestDocumentE>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);

            // test execution
            var document = IndexManager.GetIndexDocument<UnitTestIndexTestDocumentA>();
            Assert.NotNull(document);
            Assert.True(document.GetType() == typeof(IndexDocument<UnitTestIndexTestDocumentA>));

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
            IndexManager.Register<UnitTestIndexTestDocumentB>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);
            IndexManager.Register<UnitTestIndexTestDocumentC>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);
            IndexManager.Register<UnitTestIndexTestDocumentD>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);
            IndexManager.Register<UnitTestIndexTestDocumentE>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);

            // test execution
            var document = IndexManager.GetIndexDocument<UnitTestIndexTestDocumentA>();
            Assert.Null(document);

            // postconditions
            Postconditions();
        }
    }
}
