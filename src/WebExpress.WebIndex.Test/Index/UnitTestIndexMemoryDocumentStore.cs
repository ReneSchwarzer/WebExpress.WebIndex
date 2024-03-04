using WebExpress.WebIndex.Memory;
using Xunit.Abstractions;

namespace WebExpress.WebIndex.Test.Index
{
    [Collection("UnitTestIndexCollectionFixture")]
    public class UnitTestIndexMemoryDocumentStore(UnitTestIndexFixture fixture, ITestOutputHelper output)
    {
        public ITestOutputHelper Output { get; private set; } = output;
        protected UnitTestIndexFixture Fixture { get; set; } = fixture;

        [Fact]
        public void Create()
        {
            var context = new IndexContext();

            var data = UnitTestIndexTestMockD.GenerateTestData();
            var forwardIndex = new IndexMemoryStore<UnitTestIndexTestMockD>(context, (uint)data.Count);

            forwardIndex.Dispose();

            Directory.Delete(context.IndexDirectory, true);
        }

        [Fact]
        public void Add()
        {
            var context = new IndexContext();

            var data = UnitTestIndexTestMockD.GenerateTestData();
            var randomItem = data[new Random().Next() % data.Count];
            var forwardIndex = new IndexMemoryStore<UnitTestIndexTestMockD>(context, (uint)data.Count);

            foreach (var item in data)
            {
                forwardIndex.Add(item);
            }

            var i = forwardIndex.GetItem(randomItem.Id);

            Assert.True(i != null && i.Id == randomItem.Id);

            forwardIndex.Dispose();
        }

        [Fact]
        public void All()
        {
            var context = new IndexContext();

            var data = UnitTestIndexTestMockD.GenerateTestData();
            var forwardIndex = new IndexMemoryStore<UnitTestIndexTestMockD>(context, (uint)data.Count);

            foreach (var item in data)
            {
                forwardIndex.Add(item);
            }

            var all = forwardIndex.All;

            Assert.True(all.Select(x => x.Id).SequenceEqual(data.Select(x => x.Id)));

            forwardIndex.Dispose();
        }

        [Fact]
        public void Remove()
        {
            var context = new IndexContext();

            var data = UnitTestIndexTestMockD.GenerateTestData();
            var randomItem = data[new Random().Next() % data.Count];
            var forwardIndex = new IndexMemoryStore<UnitTestIndexTestMockD>(context, (uint)data.Count);

            foreach (var item in data)
            {
                forwardIndex.Add(item);
            }

            forwardIndex.Remove(randomItem);
            var all = forwardIndex.All;

            Assert.True(all.Select(x => x.Id).SequenceEqual(data.Where(x => x.Id != randomItem.Id).Select(x => x.Id)));

            forwardIndex.Dispose();
        }
    }
}
