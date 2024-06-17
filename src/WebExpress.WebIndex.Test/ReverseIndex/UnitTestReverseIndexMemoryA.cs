﻿using System.Globalization;
using System.Reflection;
using WebExpress.WebIndex.Memory;
using WebExpress.WebIndex.Test.Document;
using WebExpress.WebIndex.Test.Fixture;
using Xunit.Abstractions;

namespace WebExpress.WebIndex.Test.ReverseIndex
{
    /// <summary>
    /// Test class for testing the memory-based reverse index.
    /// </summary>
    /// <param name="fixture">The log.</param>
    /// <param name="output">The test context.</param>
    public class UnitTestReverseIndexMemoryA(UnitTestIndexFixtureIndexA fixture, ITestOutputHelper output) : UnitTestReverseIndex<UnitTestIndexFixtureIndexA>(fixture, output)
    {
        /// <summary>
        /// Returns the property.
        /// </summary>
        protected static PropertyInfo Property => typeof(UnitTestIndexTestDocumentA).GetProperty("Text");

        /// <summary>
        /// Creates a reverse index.
        /// </summary>
        [Fact]
        public void Create()
        {
            // preconditions
            Preconditions();

            // test execution
            var reverseIndex = new IndexMemoryReverse<UnitTestIndexTestDocumentA>(Context, Property, CultureInfo.GetCultureInfo("en"));

            // postconditions
            reverseIndex.Dispose();
            Postconditions();
        }

        /// <summary>
        /// Adds items to a reverse index.
        /// </summary>
        [Fact]
        public void Add()
        {
            // preconditions
            Preconditions();
            var reverseIndex = new IndexMemoryReverse<UnitTestIndexTestDocumentA>(Context, Property, CultureInfo.GetCultureInfo("en"));

            reverseIndex.Clear();

            foreach (var item in Fixture.TestData)
            {
                // test execution
                reverseIndex.Add(item);
            }

            Assert.NotNull(reverseIndex);

            // postconditions
            Postconditions();
        }

        /// <summary>
        /// Adds a token to an existing entry in the reverse index.
        /// </summary>
        [Fact]
        public void AddToken()
        {
            // preconditions
            Preconditions();
            var randomItem = Fixture.RandomItem;
            var reverseIndex = new IndexMemoryReverse<UnitTestIndexTestDocumentA>(Context, Property, CultureInfo.GetCultureInfo("en"));

            reverseIndex.Clear();
            foreach (var item in Fixture.TestData)
            {
                reverseIndex.Add(item);
            }

            var token = Context.TokenAnalyzer.Analyze(randomItem.Text + ", hello Aurora", CultureInfo.GetCultureInfo("en"));

            // test execution
            reverseIndex.Add(randomItem, token.TakeLast(1));
            var all = reverseIndex.Retrieve("aurora", new IndexRetrieveOptions());

            Assert.Contains(randomItem.Id, all);

            // postconditions
            reverseIndex.Dispose();
            Postconditions();
        }

        /// <summary>
        /// Removes an entry from the reverse index.
        /// </summary>
        [Fact]
        public void Remove()
        {
            // preconditions
            Preconditions();
            var reverseIndex = new IndexMemoryReverse<UnitTestIndexTestDocumentA>(Context, Property, CultureInfo.GetCultureInfo("en"));

            reverseIndex.Clear();
            foreach (var item in Fixture.TestData)
            {
                reverseIndex.Add(item);
            }

            _ = reverseIndex.Root.Terms;
            var items = reverseIndex.Retrieve("Helena", new IndexRetrieveOptions());

            Assert.NotNull(reverseIndex);
            Assert.Equal(4, items.Count());
            var randomItem = items.Skip(new Random().Next() % items.Count()).FirstOrDefault();

            // test execution
            reverseIndex.Remove(Fixture.TestData.Where(x => x.Id == randomItem).FirstOrDefault());

            items = reverseIndex.Retrieve("Helena", new IndexRetrieveOptions());
            Assert.Equal(3, items.Count());

            // postconditions
            reverseIndex.Dispose();
            Postconditions();
        }

        /// <summary>
        /// Removes a token without deleting the entire entry.
        /// </summary>
        [Fact]
        public void RemoveToken()
        {
            // preconditions
            Preconditions();
            var randomItem = Fixture.RandomItem;
            var reverseIndex = new IndexMemoryReverse<UnitTestIndexTestDocumentA>(Context, Property, CultureInfo.GetCultureInfo("en"));

            reverseIndex.Clear();
            foreach (var item in Fixture.TestData)
            {
                reverseIndex.Add(item);
            }

            var token = Context.TokenAnalyzer.Analyze(randomItem.Text + ", hello Aurora", CultureInfo.GetCultureInfo("en"));
            reverseIndex.Add(randomItem, token.TakeLast(1));

            // test execution
            reverseIndex.Remove(randomItem, token.TakeLast(1));

            var items = reverseIndex.Retrieve("aurora", new IndexRetrieveOptions());
            Assert.Empty(items);

            // postconditions
            reverseIndex.Dispose();
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
            var reverseIndex = new IndexMemoryReverse<UnitTestIndexTestDocumentA>(Context, Property, CultureInfo.GetCultureInfo("en"));

            reverseIndex.Clear();
            foreach (var item in Fixture.TestData)
            {
                reverseIndex.Add(item);
            }

            // test execution
            var items = reverseIndex.Retrieve("Helena", new IndexRetrieveOptions());

            Assert.NotNull(reverseIndex);
            Assert.Equal(4, items.Count());

            // postconditions
            reverseIndex.Dispose();
            Postconditions();
        }

        /// <summary>
        /// Return all entries of the reverse index.
        /// </summary>
        [Fact]
        public void All()
        {
            // preconditions
            Preconditions();
            var reverseIndex = new IndexMemoryReverse<UnitTestIndexTestDocumentA>(Context, Property, CultureInfo.GetCultureInfo("en"));

            reverseIndex.Clear();
            foreach (var item in Fixture.TestData)
            {
                reverseIndex.Add(item);
            }

            // test execution
            //var all = reverseIndex.All;

            //Assert.True(all.Select(x => x.DocumentID).SequenceEqual(data.Select(x => x.DocumentID)));

            // postconditions
            reverseIndex.Dispose();
            Postconditions();
        }
    }
}
