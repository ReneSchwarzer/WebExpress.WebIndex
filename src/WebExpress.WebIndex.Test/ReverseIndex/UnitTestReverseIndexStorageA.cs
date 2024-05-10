using System.Globalization;
using System.Reflection;
using WebExpress.WebIndex.Memory;
using WebExpress.WebIndex.Test.Document;
using WebExpress.WebIndex.Test.Fixture;
using Xunit.Abstractions;

namespace WebExpress.WebIndex.Test.ReverseIndex
{
    /// <summary>
    /// Test class for testing the storage-based reverse index.
    /// </summary>
    public class UnitTestReverseIndexStorageA : UnitTestReverseIndex<UnitTestIndexFixtureIndexA>
    {
        /// <summary>
        /// Returns the property.
        /// </summary>
        protected PropertyInfo Property => typeof(UnitTestIndexTestDocumentA).GetProperty("Text");

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fixture">The log.</param>
        /// <param name="output">The test context.</param>
        public UnitTestReverseIndexStorageA(UnitTestIndexFixtureIndexA fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
        }

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

            // test execution
            foreach (var item in Fixture.TestData)
            {
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

            randomItem.Text += ", hello Aurora!";
            var token = Context.TokenAnalyzer.Analyze(randomItem.Text, CultureInfo.GetCultureInfo("en"));

            // test execution
            reverseIndex.Add(randomItem, token.TakeLast(1));
            var all = reverseIndex.Collect("aurora");

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
            var reverseIndex = new IndexMemoryReverse<UnitTestIndexTestDocumentA>(Context, Property, CultureInfo.GetCultureInfo("en"));

            reverseIndex.Clear();
            foreach (var item in Fixture.TestData)
            {
                reverseIndex.Add(item);
            }

            var items = reverseIndex.Collect("Helena");

            Assert.NotNull(reverseIndex);
            Assert.Equal(2, items.Count());

            // test execution
            reverseIndex.Remove(randomItem);

            items = reverseIndex.Collect("Helena");
            Assert.Single(items);

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

            randomItem.Text += ", hello Aurora!";
            var token = Context.TokenAnalyzer.Analyze(randomItem.Text, CultureInfo.GetCultureInfo("en"));
            reverseIndex.Add(randomItem, token.TakeLast(1));

            // test execution
            reverseIndex.Remove(randomItem, token.TakeLast(1));

            var items = reverseIndex.Collect("aurora");
            Assert.Empty(items);

            items = reverseIndex.Collect("helena");
            Assert.Equal(2, items.Count());

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
            var items = reverseIndex.Collect("Helena");

            Assert.Equal(2, items.Count());

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

            //Assert.True(all.Select(x => x.Id).SequenceEqual(data.Select(x => x.Id)));

            // postconditions
            reverseIndex.Dispose();
            Postconditions();
        }
    }
}
