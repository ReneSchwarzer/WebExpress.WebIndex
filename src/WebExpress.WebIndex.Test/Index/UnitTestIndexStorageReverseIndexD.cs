using System.Globalization;
using System.Reflection;
using WebExpress.WebIndex.Memory;
using WebExpress.WebIndex.Test.Document;
using WebExpress.WebIndex.Test.Fixture;
using Xunit.Abstractions;

namespace WebExpress.WebIndex.Test.Index
{
    /// <summary>
    /// Test class for testing the storage-based reverse index.
    /// </summary>
    public class UnitTestIndexStorageReverseIndexD : UnitTestIndexStorage<UnitTestIndexFixtureIndexD>
    {
        /// <summary>
        /// Test class for testing the storage-based reverse index.
        /// </summary>
        protected PropertyInfo Property => typeof(UnitTestIndexTestDocumentD).GetProperty("FirstName");

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fixture">The log.</param>
        /// <param name="output">The test context.</param>
        public UnitTestIndexStorageReverseIndexD(UnitTestIndexFixtureIndexD fixture, ITestOutputHelper output)
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
            var reverseIndex = new IndexMemoryReverse<UnitTestIndexTestDocumentD>(IndexManager.Context, Property, CultureInfo.GetCultureInfo("en"));

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
            var reverseIndex = new IndexMemoryReverse<UnitTestIndexTestDocumentD>(IndexManager.Context, Property, CultureInfo.GetCultureInfo("en"));

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
        /// Update an entry in the reverse index where the item has a first name change.
        /// </summary>
        [Fact]
        public void UpdateWithChange()
        {
            // preconditions
            Preconditions();
            var randomItem = Fixture.RandomItem;
            var reverseIndex = new IndexMemoryReverse<UnitTestIndexTestDocumentD>(IndexManager.Context, Property, CultureInfo.GetCultureInfo("en"));

            reverseIndex.Clear();
            foreach (var item in Fixture.TestData)
            {
                // test execution
                reverseIndex.Add(item);
            }

            // test execution
            var newItem = new UnitTestIndexTestDocumentD()
            {
                Id = randomItem.Id,
                FirstName = "Update_" + randomItem.FirstName
            };

            //reverseIndex.Update(newItem);
            //var all = reverseIndex.All;

            //Assert.True(all.Select(x => x.Id).OrderBy(x => x).SequenceEqual(data.Select(x => x.Id).OrderBy(x => x)));
            //Assert.True(all.Where(x => x.FirstName == randomItem.FirstName).Any());

            // postconditions
            reverseIndex.Dispose();
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
            var reverseIndex = new IndexMemoryReverse<UnitTestIndexTestDocumentD>(IndexManager.Context, Property, CultureInfo.GetCultureInfo("en"));

            reverseIndex.Clear();
            foreach (var item in Fixture.TestData)
            {
                // test execution
                reverseIndex.Add(item);
            }

            // test execution
            //reverseIndex.Update(randomItem);
            //var all = reverseIndex.All;

            //Assert.True(all.Select(x => x.Id).OrderBy(x => x).SequenceEqual(data.Select(x => x.Id).OrderBy(x => x)));
            //Assert.True(all.Where(x => x.FirstName == randomItem.FirstName).Any());

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
            var reverseIndex = new IndexMemoryReverse<UnitTestIndexTestDocumentD>(IndexManager.Context, Property, CultureInfo.GetCultureInfo("en"));

            reverseIndex.Clear();
            foreach (var item in Fixture.TestData)
            {
                // test execution
                reverseIndex.Add(item);
            }

            var before = reverseIndex.Collect(randomItem.FirstName).ToList();
            Assert.True(before.Any());

            // test execution
            reverseIndex.Remove(randomItem);

            var after = reverseIndex.Collect(randomItem.FirstName).ToList();
            Assert.True(before.Count - 1 == after.Count);

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
            var reverseIndex = new IndexMemoryReverse<UnitTestIndexTestDocumentD>(IndexManager.Context, Property, CultureInfo.GetCultureInfo("en"));

            reverseIndex.Clear();
            foreach (var item in Fixture.TestData)
            {
                // test execution
                reverseIndex.Add(item);
            }

            // test execution
            var items = reverseIndex.Collect(randomItem.FirstName);
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
            var reverseIndex = new IndexMemoryReverse<UnitTestIndexTestDocumentD>(IndexManager.Context, Property, CultureInfo.GetCultureInfo("en"));

            reverseIndex.Clear();
            foreach (var item in Fixture.TestData)
            {
                // test execution
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
