﻿using WebExpress.WebIndex.Memory;
using WebExpress.WebIndex.Test.Document;
using WebExpress.WebIndex.Test.Fixture;
using Xunit.Abstractions;

namespace WebExpress.WebIndex.Test.DocumentStore
{
    /// <summary>
    /// Test class for testing the memory-based document store.
    /// </summary>
    public class UnitTestDocumentStoreMemoryB : UnitTestDocumentStore<UnitTestIndexFixtureIndexB>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fixture">The log.</param>
        /// <param name="output">The test context.</param>
        public UnitTestDocumentStoreMemoryB(UnitTestIndexFixtureIndexB fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
        }

        /// <summary>
        /// Creates a document store.
        /// </summary>
        [Fact]
        public void Create()
        {
            // preconditions
            var context = new IndexContext();

            // test execution
            var documentStore = new IndexMemoryDocumentStore<UnitTestIndexTestDocumentB>(context, (uint)Fixture.TestData.Count);

            // postconditions
            documentStore.Dispose();
        }

        /// <summary>
        /// Adds items to a document store.
        /// </summary>
        [Fact]
        public void Add()
        {
            // preconditions
            Preconditions();
            var documentStore = new IndexMemoryDocumentStore<UnitTestIndexTestDocumentB>(Context, (uint)Fixture.TestData.Count);

            documentStore.Clear();

            // test execution
            foreach (var item in Fixture.TestData)
            {
                documentStore.Add(item);
            }

            var i = documentStore.GetItem(Fixture.TestData[0].Id);

            Assert.True(i != null && i.Id == Fixture.TestData[0].Id);

            // postconditions
            documentStore.Dispose();
            Postconditions();
        }

        /// <summary>
        /// Update an entry in the reverse index where the item has a first name change.
        /// </summary>
        [Fact]
        public void UpdateWithChange()
        {
            // preconditions
            Preconditions();
            var documentStore = new IndexMemoryDocumentStore<UnitTestIndexTestDocumentB>(Context, (uint)Fixture.TestData.Count);
            var randomItem = Fixture.RandomItem;

            documentStore.Clear();
            foreach (var item in Fixture.TestData)
            {
                documentStore.Add(item);
            }

            var name = "Update_" + Fixture.TestData[0].Name;
            var changed = new UnitTestIndexTestDocumentB
            {
                Id = randomItem.Id,
                Name = name,
            };

            // test execution
            documentStore.Update(changed);

            var all = documentStore.All;

            Assert.True(all.Select(x => x.Id).OrderBy(x => x).SequenceEqual(Fixture.TestData.Select(x => x.Id).OrderBy(x => x)));
            Assert.True(all.Where(x => x.Name == name).Any());

            // postconditions
            documentStore.Dispose();
            Postconditions();
        }

        /// <summary>
        /// Changes an entry in the reverse index without the element to be changed having any changes.
        /// </summary>
        [Fact]
        public void UpdateWithoutChanges()
        {
            // preconditions
            Preconditions();
            var documentStore = new IndexMemoryDocumentStore<UnitTestIndexTestDocumentB>(Context, (uint)Fixture.TestData.Count);
            var randomItem = Fixture.RandomItem;

            documentStore.Clear();
            foreach (var item in Fixture.TestData)
            {
                documentStore.Add(item);
            }

            // test execution
            documentStore.Update(randomItem);
            var all = documentStore.All;

            Assert.True(all.Select(x => x.Id).OrderBy(x => x).SequenceEqual(Fixture.TestData.Select(x => x.Id).OrderBy(x => x)));
            Assert.True(all.Where(x => x.Name == randomItem.Name).Any());

            // postconditions
            documentStore.Dispose();
            Postconditions();
        }

        /// <summary>
        /// Removes an entry from the document store.
        /// </summary>
        [Fact]
        public void Remove()
        {
            // preconditions
            Preconditions();
            var documentStore = new IndexMemoryDocumentStore<UnitTestIndexTestDocumentB>(Context, (uint)Fixture.TestData.Count);

            documentStore.Clear();
            foreach (var item in Fixture.TestData)
            {
                documentStore.Add(item);
            }

            // test execution
            documentStore.Delete(Fixture.TestData[0]);
            var all = documentStore.All;

            Assert.True(all.Select(x => x.Id).SequenceEqual(Fixture.TestData.Where(x => x.Id != Fixture.TestData[0].Id).Select(x => x.Id)));

            // postconditions
            documentStore.Dispose();
            Postconditions();
        }

        /// <summary>
        /// Retrieve a entry of the reverse index.
        /// </summary>
        [Fact]
        public void Retrieve()
        {
            // preconditions
            Preconditions();
            var documentStore = new IndexMemoryDocumentStore<UnitTestIndexTestDocumentB>(Context, (uint)Fixture.TestData.Count);

            documentStore.Clear();
            foreach (var document in Fixture.TestData)
            {
                documentStore.Add(document);
            }

            // test execution
            var item = documentStore.GetItem(Fixture.TestData[0].Id);

            Assert.NotNull(documentStore);
            Assert.NotNull(item);

            // postconditions
            documentStore.Dispose();
            Postconditions();
        }

        /// <summary>
        /// Return all entries of the document store.
        /// </summary>
        [Fact]
        public void All()
        {
            // preconditions
            Preconditions();
            var documentStore = new IndexMemoryDocumentStore<UnitTestIndexTestDocumentB>(Context, (uint)Fixture.TestData.Count);

            documentStore.Clear();
            foreach (var item in Fixture.TestData)
            {
                documentStore.Add(item);
            }

            // test execution
            var all = documentStore.All;

            Assert.True(all.Select(x => x.Id).OrderBy(x => x).SequenceEqual(Fixture.TestData.Select(x => x.Id).OrderBy(x => x)));

            // postconditions
            documentStore.Dispose();
            Postconditions();
        }
    }
}
