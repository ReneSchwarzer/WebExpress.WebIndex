﻿using System.Globalization;
using WebExpress.WebIndex.Test.Document;
using WebExpress.WebIndex.Test.Fixture;
using Xunit.Abstractions;

namespace WebExpress.WebIndex.Test.IndexManager
{
    /// <summary>
    /// Test class for testing the memory-based index manager.
    /// </summary>
    [Collection("NonParallelTests")]
    public class UnitTestIndexManagerMemoryD : UnitTestIndexManager<UnitTestIndexFixtureIndexD>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fixture">The log.</param>
        /// <param name="output">The test context.</param>
        public UnitTestIndexManagerMemoryD(UnitTestIndexFixtureIndexD fixture, ITestOutputHelper output)
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
            IndexManager.Create<UnitTestIndexTestDocumentD>(CultureInfo.GetCultureInfo("en"), IndexType.Memory);

            Assert.NotNull(IndexManager.GetIndexDocument<UnitTestIndexTestDocumentD>());

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
            var randomItem = Fixture.RandomItem;
            IndexManager.Create<UnitTestIndexTestDocumentD>(CultureInfo.GetCultureInfo(culture), IndexType.Memory);

            // test execution
            IndexManager.ReIndex(Fixture.TestData);

            var wql = IndexManager.Retrieve<UnitTestIndexTestDocumentD>($"firstname = '{randomItem.FirstName}'");
            Assert.NotNull(wql);

            var item = wql.Apply();
            Assert.NotEmpty(item);

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
            var randomItem = Fixture.RandomItem;
            IndexManager.Create<UnitTestIndexTestDocumentD>(CultureInfo.GetCultureInfo(culture), IndexType.Memory);

            // test execution
            await IndexManager.ReIndexAsync(Fixture.TestData);

            var wql = IndexManager.Retrieve<UnitTestIndexTestDocumentD>($"firstname = '{randomItem.FirstName}'");
            Assert.NotNull(wql);

            var item = wql.Apply();
            Assert.NotEmpty(item);

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
            var randomItem = Fixture.RandomItem;
            IndexManager.Create<UnitTestIndexTestDocumentD>(CultureInfo.GetCultureInfo("en"), IndexType.Memory);
            IndexManager.ReIndex(Fixture.TestData);

            var wql = IndexManager.Retrieve<UnitTestIndexTestDocumentD>($"firstname = '{randomItem.FirstName}'");
            Assert.NotNull(wql);

            var before = wql.Apply().ToList();
            Assert.True(before.Any());

            // test execution
            IndexManager.Delete(randomItem);

            wql = IndexManager.Retrieve<UnitTestIndexTestDocumentD>($"firstname = '{randomItem.FirstName}'");
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
            IndexManager.Create<UnitTestIndexTestDocumentD>(CultureInfo.GetCultureInfo("en"), IndexType.Memory);
            IndexManager.ReIndex(Fixture.TestData);

            // test execution
            IndexManager.Insert(new UnitTestIndexTestDocumentD()
            {
                Id = Guid.Parse("ED242C79-E41B-4214-BFBC-C4673E87433B"),
                FirstName = "Aurora"
            });

            var wql = IndexManager.Retrieve<UnitTestIndexTestDocumentD>("firstname = 'Aurora'");
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
            IndexManager.Create<UnitTestIndexTestDocumentD>(CultureInfo.GetCultureInfo("en"), IndexType.Memory);
            IndexManager.ReIndex(Fixture.TestData);

            // test execution
            IndexManager.Update(new UnitTestIndexTestDocumentD()
            {
                Id = randomItem.Id,
                FirstName = "Aurora"
            });

            var wql = IndexManager.Retrieve<UnitTestIndexTestDocumentD>("firstname = 'Aurora'");
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
        public async Task UpdateAsync()
        {
            // preconditions
            Preconditions();
            var randomItem = Fixture.RandomItem;
            IndexManager.Create<UnitTestIndexTestDocumentD>(CultureInfo.GetCultureInfo("en"), IndexType.Memory);
            await IndexManager.ReIndexAsync(Fixture.TestData);

            // test execution
            await IndexManager.UpdateAsync(new UnitTestIndexTestDocumentD()
            {
                Id = randomItem.Id,
                FirstName = "Aurora"
            });

            var wql = IndexManager.Retrieve<UnitTestIndexTestDocumentD>("firstname = 'Aurora'");
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
            IndexManager.Create<UnitTestIndexTestDocumentD>(CultureInfo.GetCultureInfo("en"), IndexType.Memory);
            IndexManager.ReIndex(Fixture.TestData);

            var documents = IndexManager.All<UnitTestIndexTestDocumentD>();

            Assert.NotNull(documents);
            Assert.True(documents.Any());

            // test execution
            IndexManager.Clear<UnitTestIndexTestDocumentD>();

            documents = IndexManager.All<UnitTestIndexTestDocumentD>();

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
            IndexManager.Create<UnitTestIndexTestDocumentD>(CultureInfo.GetCultureInfo("en"), IndexType.Memory);
            IndexManager.ReIndex(Fixture.TestData);

            // test execution
            var all = IndexManager.All<UnitTestIndexTestDocumentD>();

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
            IndexManager.Create<UnitTestIndexTestDocumentA>(CultureInfo.GetCultureInfo("en"), IndexType.Memory);
            IndexManager.Create<UnitTestIndexTestDocumentB>(CultureInfo.GetCultureInfo("en"), IndexType.Memory);
            IndexManager.Create<UnitTestIndexTestDocumentC>(CultureInfo.GetCultureInfo("en"), IndexType.Memory);
            IndexManager.Create<UnitTestIndexTestDocumentD>(CultureInfo.GetCultureInfo("en"), IndexType.Memory);
            IndexManager.Create<UnitTestIndexTestDocumentE>(CultureInfo.GetCultureInfo("en"), IndexType.Memory);

            // test execution
            var document = IndexManager.GetIndexDocument<UnitTestIndexTestDocumentD>();
            Assert.NotNull(document);
            Assert.True(document.GetType() == typeof(IndexDocument<UnitTestIndexTestDocumentD>));

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
            IndexManager.Create<UnitTestIndexTestDocumentA>(CultureInfo.GetCultureInfo("en"), IndexType.Memory);
            IndexManager.Create<UnitTestIndexTestDocumentB>(CultureInfo.GetCultureInfo("en"), IndexType.Memory);
            IndexManager.Create<UnitTestIndexTestDocumentC>(CultureInfo.GetCultureInfo("en"), IndexType.Memory);
            IndexManager.Create<UnitTestIndexTestDocumentE>(CultureInfo.GetCultureInfo("en"), IndexType.Memory);

            // test execution
            var document = IndexManager.GetIndexDocument<UnitTestIndexTestDocumentD>();
            Assert.Null(document);

            // postconditions
            Postconditions();
        }
    }
}
