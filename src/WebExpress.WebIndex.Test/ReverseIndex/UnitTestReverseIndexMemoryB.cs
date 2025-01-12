using System.Globalization;
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
    [Collection("NonParallelTests")]
    public class UnitTestReverseIndexMemoryB(UnitTestIndexFixtureIndexB fixture, ITestOutputHelper output) : UnitTestReverseIndex<UnitTestIndexFixtureIndexB>(fixture, output)
    {
        /// <summary>
        /// Returns the property.
        /// </summary>
        protected static PropertyInfo Property => typeof(UnitTestIndexTestDocumentB).GetProperty("Name");

        /// <summary>
        /// Creates a reverse index.
        /// </summary>
        [Fact]
        public void Create()
        {
            // preconditions
            Preconditions();

            // test execution
            var reverseIndex = new IndexMemoryReverse<UnitTestIndexTestDocumentB>(Context, Property, CultureInfo.GetCultureInfo("en"));

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
            var reverseIndex = new IndexMemoryReverse<UnitTestIndexTestDocumentB>(Context, Property, CultureInfo.GetCultureInfo("en"));

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
            var reverseIndex = new IndexMemoryReverse<UnitTestIndexTestDocumentB>(Context, Property, CultureInfo.GetCultureInfo("en"));

            reverseIndex.Clear();
            foreach (var item in Fixture.TestData)
            {
                reverseIndex.Add(item);
            }

            var token = Context.TokenAnalyzer.Analyze("Aurora", CultureInfo.GetCultureInfo("en"));

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
            var randomItem = Fixture.RandomItem;
            var reverseIndex = new IndexMemoryReverse<UnitTestIndexTestDocumentB>(Context, Property, CultureInfo.GetCultureInfo("en"));

            reverseIndex.Clear();
            foreach (var item in Fixture.TestData)
            {
                // test execution
                reverseIndex.Add(item);
            }

            var before = reverseIndex.Retrieve(randomItem.Name, new IndexRetrieveOptions()).ToList();
            Assert.NotEmpty(before);

            // test execution
            reverseIndex.Delete(randomItem);

            var after = reverseIndex.Retrieve(randomItem.Name, new IndexRetrieveOptions()).ToList();
            Assert.True(before.Count - 1 == after.Count);

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
            var reverseIndex = new IndexMemoryReverse<UnitTestIndexTestDocumentB>(Context, Property, CultureInfo.GetCultureInfo("en"));

            reverseIndex.Clear();
            foreach (var item in Fixture.TestData)
            {
                reverseIndex.Add(item);
            }

            var token = Context.TokenAnalyzer.Analyze("Aurora", CultureInfo.GetCultureInfo("en"));
            reverseIndex.Add(randomItem, token.TakeLast(1));

            // test execution
            reverseIndex.Delete(randomItem, token.TakeLast(1));

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
            var randomItem = Fixture.RandomItem;
            var reverseIndex = new IndexMemoryReverse<UnitTestIndexTestDocumentB>(Context, Property, CultureInfo.GetCultureInfo("en"));

            reverseIndex.Clear();
            foreach (var item in Fixture.TestData)
            {
                // test execution
                reverseIndex.Add(item);
            }

            // test execution
            var items = reverseIndex.Retrieve(randomItem.Name, new IndexRetrieveOptions());
            Assert.True(items.Any());

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
            var reverseIndex = new IndexMemoryReverse<UnitTestIndexTestDocumentB>(Context, Property, CultureInfo.GetCultureInfo("en"));

            reverseIndex.Clear();
            foreach (var item in Fixture.TestData)
            {
                // test execution
                reverseIndex.Add(item);
            }

            // test execution
            var all = reverseIndex.All;

            Assert.Equal(all.OrderBy(x => x), Fixture.TestData.Select(x => x.Id).OrderBy(x => x));

            // postconditions
            reverseIndex.Dispose();
            Postconditions();
        }
    }
}
