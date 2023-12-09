using System.Globalization;
using WebExpress.WebIndex.Storage;
using Xunit.Abstractions;

namespace WebExpress.WebIndex.Test.Index
{
    [Collection("UnitTestIndexCollectionFixture")]
    public class UnitTestIndexStorageForward
    {
        public ITestOutputHelper Output { get; private set; }
        protected UnitTestIndexFixture Fixture { get; set; }

        public UnitTestIndexStorageForward(UnitTestIndexFixture fixture, ITestOutputHelper output)
        {
            Fixture = fixture;
            Output = output;
        }

        [Fact]
        public void Create()
        {
            var context = new IndexContext();
            context.IndexDirectory = Path.Combine(context.IndexDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));

            var data = UnitTestIndexTestMockA.GenerateTestData();
            var forwardIndex = new IndexStorageForward<UnitTestIndexTestMockA>(context, new CultureInfo("en"), (uint)data.Count);

            forwardIndex.Dispose();

            Directory.Delete(context.IndexDirectory, true);
        }

        [Fact]
        public void Open()
        {
            var context = new IndexContext();
            context.IndexDirectory = Path.Combine(context.IndexDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));

            var data = UnitTestIndexTestMockA.GenerateTestData();
            var forwardIndex = new IndexStorageForward<UnitTestIndexTestMockA>(context, new CultureInfo("en"), (uint)data.Count);

            foreach (var item in data)
            {
                forwardIndex.Add(item);
            }

            forwardIndex.Dispose();

            forwardIndex = new IndexStorageForward<UnitTestIndexTestMockA>(context, new CultureInfo("en"), (uint)data.Count);

            forwardIndex.Dispose();

            Directory.Delete(context.IndexDirectory, true);
        }

        [Fact]
        public void Add()
        {
            var context = new IndexContext();
            context.IndexDirectory = Path.Combine(context.IndexDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));

            var data = UnitTestIndexTestMockA.GenerateTestData();
            var randomItem = data[new Random().Next() % data.Count];
            var forwardIndex = new IndexStorageForward<UnitTestIndexTestMockA>(context, new CultureInfo("en"), (uint)data.Count);

            foreach (var item in data)
            {
                forwardIndex.Add(item);
            }

            var i = forwardIndex.GetItem(randomItem.Id);

            Assert.True(i != null && i.Id == randomItem.Id);

            forwardIndex.Dispose();

            Directory.Delete(context.IndexDirectory, true);
        }

        [Fact]
        public void All()
        {
            var context = new IndexContext();
            context.IndexDirectory = Path.Combine(context.IndexDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));

            var data = UnitTestIndexTestMockA.GenerateTestData();
            var forwardIndex = new IndexStorageForward<UnitTestIndexTestMockA>(context, new CultureInfo("en"), (uint)data.Count);

            foreach (var item in data)
            {
                forwardIndex.Add(item);
            }

            var all = forwardIndex.All;

            Assert.True(all.Select(x => x.Id).SequenceEqual(data.Select(x => x.Id)));

            forwardIndex.Dispose();

            Directory.Delete(context.IndexDirectory, true);
        }


    }
}
