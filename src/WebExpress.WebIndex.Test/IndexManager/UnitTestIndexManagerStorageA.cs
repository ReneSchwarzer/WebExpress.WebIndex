using System.Globalization;
using WebExpress.WebIndex.Test.Document;
using WebExpress.WebIndex.Test.Fixture;
using Xunit.Abstractions;

namespace WebExpress.WebIndex.Test.IndexManager
{
    /// <summary>
    /// Test class for testing the storage-based index manager.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestIndexManagerStorageA : UnitTestIndexManager<UnitTestIndexFixtureIndexA>
    {
        /// <summary>
        /// Initializes a new instance of the class.
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
        public void Create()
        {
            // preconditions
            Preconditions();

            // test execution
            IndexManager.Create<UnitTestIndexTestDocumentA>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);

            Assert.NotNull(IndexManager.GetIndexDocument<UnitTestIndexTestDocumentA>());

            // postconditions
            Postconditions();
        }

        /// <summary>
        /// Tests the reindex function from the index manager.
        /// </summary>
        [Theory]
        [InlineData("en")]
        [InlineData("de")]
        [InlineData("de-DE")]
        [InlineData("fr")]
        public void ReIndex(string culture)
        {
            // preconditions
            Preconditions();
            IndexManager.Create<UnitTestIndexTestDocumentA>(CultureInfo.GetCultureInfo(culture), IndexType.Storage);

            // test execution
            IndexManager.ReIndex(Fixture.TestData);

            var wql = IndexManager.Retrieve<UnitTestIndexTestDocumentA>("text ~ 'Helena'");
            Assert.NotNull(wql);

            var item = wql.Apply();
            Assert.Equal(4, item.Count());

            // postconditions
            Postconditions();
        }

        /// <summary>
        /// Tests the reindex function from the index manager.
        /// </summary>
        [Theory]
        [InlineData("en")]
        [InlineData("de")]
        [InlineData("de-DE")]
        [InlineData("fr")]
        public async Task ReIndexAsync(string culture)
        {
            // preconditions
            Preconditions();
            IndexManager.Create<UnitTestIndexTestDocumentA>(CultureInfo.GetCultureInfo(culture), IndexType.Storage);

            // test execution
            await IndexManager.ReIndexAsync(Fixture.TestData);

            var wql = IndexManager.Retrieve<UnitTestIndexTestDocumentA>("text ~ 'Helena'");
            Assert.NotNull(wql);

            var item = wql.Apply();
            Assert.Equal(4, item.Count());

            // postconditions
            Postconditions();
        }

        /// <summary>
        /// Tests the removal of a document from the index manager.
        /// </summary>
        [Fact]
        public void Delete()
        {
            // preconditions
            Preconditions();
            IndexManager.Create<UnitTestIndexTestDocumentA>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);
            IndexManager.ReIndex(Fixture.TestData);

            var wql = IndexManager.Retrieve<UnitTestIndexTestDocumentA>("text ~ 'Helena'");
            Assert.NotNull(wql);

            var item = wql.Apply();
            Assert.Equal(4, item.Count());

            // test execution
            IndexManager.Delete(Fixture.TestData[0]);

            wql = IndexManager.Retrieve<UnitTestIndexTestDocumentA>("text ~ 'Helena'");
            Assert.NotNull(wql);

            item = wql.Apply();
            Assert.Equal(3, item.Count());

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
            IndexManager.Create<UnitTestIndexTestDocumentA>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);
            IndexManager.ReIndex(Fixture.TestData);

            // test execution
            IndexManager.Insert(new UnitTestIndexTestDocumentA()
            {
                Id = Guid.Parse("ED242C79-E41B-4214-BFBC-C4673E87433B"),
                Text = "Hello Aurora!"
            });

            var wql = IndexManager.Retrieve<UnitTestIndexTestDocumentA>("text ~ 'Aurora'");
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
            IndexManager.Create<UnitTestIndexTestDocumentA>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);
            IndexManager.ReIndex(Fixture.TestData);

            // test execution
            IndexManager.Update(new UnitTestIndexTestDocumentA()
            {
                Id = Fixture.TestData[1].Id,
                Text = "Hello Helena, hello Aurora!"
            });

            var wql = IndexManager.Retrieve<UnitTestIndexTestDocumentA>("text ~ 'Aurora'");
            Assert.NotNull(wql);

            var item = wql.Apply();
            Assert.Equal(1, item.Count());

            wql = IndexManager.Retrieve<UnitTestIndexTestDocumentA>("text ~ 'Helge'");
            Assert.NotNull(wql);

            item = wql.Apply();
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
            IndexManager.Create<UnitTestIndexTestDocumentA>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);
            await IndexManager.ReIndexAsync(Fixture.TestData);

            // test execution
            await IndexManager.UpdateAsync(new UnitTestIndexTestDocumentA()
            {
                Id = new Guid("c7d8f9e0-3a2b-4c5d-8e6f-9a1b0c2d4e5f"),
                Text = "Hello Helena, hello Aurora!"
            });

            var wql = IndexManager.Retrieve<UnitTestIndexTestDocumentA>("text ~ 'Aurora'");
            Assert.NotNull(wql);

            var item = wql.Apply();
            Assert.Equal(1, item.Count());

            wql = IndexManager.Retrieve<UnitTestIndexTestDocumentA>("text ~ 'Helge'");
            Assert.NotNull(wql);

            item = wql.Apply();
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
            IndexManager.Create<UnitTestIndexTestDocumentA>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);
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
            IndexManager.Create<UnitTestIndexTestDocumentA>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);
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
            IndexManager.Create<UnitTestIndexTestDocumentA>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);
            IndexManager.Create<UnitTestIndexTestDocumentB>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);
            IndexManager.Create<UnitTestIndexTestDocumentC>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);
            IndexManager.Create<UnitTestIndexTestDocumentD>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);
            IndexManager.Create<UnitTestIndexTestDocumentE>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);

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
            IndexManager.Create<UnitTestIndexTestDocumentB>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);
            IndexManager.Create<UnitTestIndexTestDocumentC>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);
            IndexManager.Create<UnitTestIndexTestDocumentD>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);
            IndexManager.Create<UnitTestIndexTestDocumentE>(CultureInfo.GetCultureInfo("en"), IndexType.Storage);

            // test execution
            var document = IndexManager.GetIndexDocument<UnitTestIndexTestDocumentA>();
            Assert.Null(document);

            // postconditions
            Postconditions();
        }

        /// <summary>
        /// Tests the close and open function from the index manager.
        /// </summary>
        [Theory]
        [InlineData("en")]
        [InlineData("de")]
        [InlineData("de-DE")]
        [InlineData("fr")]
        public void Reopen(string culture)
        {
            // preconditions
            Preconditions();
            IndexManager.Create<UnitTestIndexTestDocumentA>(CultureInfo.GetCultureInfo(culture), IndexType.Storage);
            IndexManager.ReIndex(Fixture.TestData);
            var wql = IndexManager.Retrieve<UnitTestIndexTestDocumentA>("text ~ 'Helena'");
            Assert.NotNull(wql);

            var item = wql.Apply();
            var count = item.Count();

            // test execution
            IndexManager.Close<UnitTestIndexTestDocumentA>();

            IndexManager.Create<UnitTestIndexTestDocumentA>(CultureInfo.GetCultureInfo(culture), IndexType.Storage);
            wql = IndexManager.Retrieve<UnitTestIndexTestDocumentA>("text ~ 'Helena'");
            Assert.NotNull(wql);

            item = wql.Apply();
            Assert.Equal(count, item.Count());

            // postconditions
            Postconditions();
        }
    }
}
