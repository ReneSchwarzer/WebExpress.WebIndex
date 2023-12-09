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
        public void Add()
        {
            var context = new IndexContext();
            context.IndexDirectory = Path.Combine(context.IndexDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));

            var data = UnitTestIndexTestDocumentA.GenerateTestData();
            var randomItem = data[new Random().Next() % data.Count];
            var forwardIndex = new IndexStorageForward<UnitTestIndexTestDocumentA>(context, new CultureInfo("en"), (uint)data.Count);

            foreach (var item in data)
            {
                forwardIndex.Add(item);
            }

            var i = forwardIndex.GetItem(randomItem.Id);

            Assert.True(i != null && i.Id == randomItem.Id);

            forwardIndex.Dispose();

            Directory.Delete(context.IndexDirectory, true);
        }
    }
}
