//using WebExpress.WebIndex.Storage;
//using WebExpress.WebIndex.Test.Document;
//using WebExpress.WebIndex.Test.Fixture;
//using Xunit.Abstractions;

//namespace WebExpress.WebIndex.Test.Index
//{
//    /// <summary>
//    /// Test class for testing the storage-based document store.
//    /// </summary>
//    /// <param name="fixture">The log.</param>
//    /// <param name="output">The test context.</param>
//    public class UnitTestIndexStorageDocumentStore(UnitTestIndexFixture fixture, ITestOutputHelper output) : IClassFixture<UnitTestIndexFixture>
//    {
//        /// <summary>
//        /// Returns the log.
//        /// </summary>
//        protected ITestOutputHelper Output { get; private set; } = output;

//        /// <summary>
//        /// Returns the test context.
//        /// </summary>
//        protected UnitTestIndexFixture Fixture { get; private set; } = fixture;

//        /// <summary>
//        /// Creates a document store.
//        /// </summary>
//        [Fact]
//        public void CreateA()
//        {
//            // preconditions
//            var context = new IndexContext();
//            context.IndexDirectory = Path.Combine(context.IndexDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));

//            // test execution
//            var documentStore = new IndexStorageDocumentStore<UnitTestIndexTestDocumentA>(context, 5);

//            Assert.NotNull(documentStore);

//            // postconditions            
//            documentStore.Dispose();
//            Directory.Delete(context.IndexDirectory, true);
//        }

//        /// <summary>
//        /// Creates a document store.
//        /// </summary>
//        [Fact]
//        public void CreateB()
//        {
//            // preconditions
//            var context = new IndexContext();
//            context.IndexDirectory = Path.Combine(context.IndexDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
//            var data = UnitTestIndexTestDocumentFactoryB.GenerateTestData();

//            // test execution
//            var documentStore = new IndexStorageDocumentStore<UnitTestIndexTestDocumentB>(context, (uint)data.Count);

//            // postconditions            
//            documentStore.Dispose();
//            Directory.Delete(context.IndexDirectory, true);
//        }

//        /// <summary>
//        /// Creates a document store.
//        /// </summary>
//        [Fact]
//        public void CreateC()
//        {
//            // preconditions
//            var context = new IndexContext();
//            context.IndexDirectory = Path.Combine(context.IndexDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
//            var data = UnitTestIndexTestDocumentFactoryC.GenerateTestData();

//            // test execution
//            var documentStore = new IndexStorageDocumentStore<UnitTestIndexTestDocumentC>(context, (uint)data.Count);

//            // postconditions            
//            documentStore.Dispose();
//            Directory.Delete(context.IndexDirectory, true);
//        }

//        /// <summary>
//        /// Creates a document store.
//        /// </summary>
//        [Fact]
//        public void CreateD()
//        {
//            // preconditions
//            var context = new IndexContext();
//            context.IndexDirectory = Path.Combine(context.IndexDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
//            var data = UnitTestIndexTestDocumentFactoryD.GenerateTestData();

//            // test execution
//            var documentStore = new IndexStorageDocumentStore<UnitTestIndexTestDocumentD>(context, (uint)data.Count);

//            // postconditions            
//            documentStore.Dispose();
//            Directory.Delete(context.IndexDirectory, true);
//        }

//        /// <summary>
//        /// Creates a document store.
//        /// </summary>
//        [Fact]
//        public void CreateE()
//        {
//            // preconditions
//            var context = new IndexContext();
//            context.IndexDirectory = Path.Combine(context.IndexDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
//            var data = UnitTestIndexTestDocumentFactoryE.GenerateTestData();

//            // test execution
//            var documentStore = new IndexStorageDocumentStore<UnitTestIndexTestDocumentE>(context, (uint)data.Count);

//            // postconditions            
//            documentStore.Dispose();
//            Directory.Delete(context.IndexDirectory, true);
//        }

//        /// <summary>
//        /// Opens an existing document store.
//        /// </summary>
//        [Fact]
//        public void OpenA()
//        {
//            // preconditions
//            var context = new IndexContext();
//            context.IndexDirectory = Path.Combine(context.IndexDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
//            var data = UnitTestIndexTestDocumentFactoryA.GenerateTestData();
//            var documentStore = new IndexStorageDocumentStore<UnitTestIndexTestDocumentA>(context, 5);

//            documentStore.Clear();
//            documentStore.Add(data[0]);
//            documentStore.Add(data[1]);
//            documentStore.Dispose();

//            // test execution
//            documentStore = new IndexStorageDocumentStore<UnitTestIndexTestDocumentA>(context, 5);

//            // postconditions            
//            documentStore.Dispose();
//            Directory.Delete(context.IndexDirectory, true);
//        }

//        /// <summary>
//        /// Opens an existing document store.
//        /// </summary>
//        [Fact]
//        public void OpenB()
//        {
//            // preconditions
//            var context = new IndexContext();
//            context.IndexDirectory = Path.Combine(context.IndexDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
//            var data = UnitTestIndexTestDocumentFactoryB.GenerateTestData();
//            var documentStore = new IndexStorageDocumentStore<UnitTestIndexTestDocumentB>(context, (uint)data.Count);

//            foreach (var item in data)
//            {
//                documentStore.Add(item);
//            }

//            documentStore.Dispose();

//            // test execution
//            documentStore = new IndexStorageDocumentStore<UnitTestIndexTestDocumentB>(context, (uint)data.Count);

//            // postconditions            
//            documentStore.Dispose();
//            Directory.Delete(context.IndexDirectory, true);
//        }

//        /// <summary>
//        /// Opens an existing document store.
//        /// </summary>
//        [Fact]
//        public void OpenC()
//        {
//            // preconditions
//            var context = new IndexContext();
//            context.IndexDirectory = Path.Combine(context.IndexDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
//            var data = UnitTestIndexTestDocumentFactoryC.GenerateTestData();
//            var documentStore = new IndexStorageDocumentStore<UnitTestIndexTestDocumentC>(context, (uint)data.Count);

//            foreach (var item in data)
//            {
//                documentStore.Add(item);
//            }

//            documentStore.Dispose();

//            // test execution
//            documentStore = new IndexStorageDocumentStore<UnitTestIndexTestDocumentC>(context, (uint)data.Count);

//            // postconditions            
//            documentStore.Dispose();
//            Directory.Delete(context.IndexDirectory, true);
//        }

//        /// <summary>
//        /// Opens an existing document store.
//        /// </summary>
//        [Fact]
//        public void OpenD()
//        {
//            // preconditions
//            var context = new IndexContext();
//            context.IndexDirectory = Path.Combine(context.IndexDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
//            var data = UnitTestIndexTestDocumentFactoryD.GenerateTestData();
//            var documentStore = new IndexStorageDocumentStore<UnitTestIndexTestDocumentD>(context, (uint)data.Count);

//            foreach (var item in data)
//            {
//                documentStore.Add(item);
//            }

//            documentStore.Dispose();

//            // test execution
//            documentStore = new IndexStorageDocumentStore<UnitTestIndexTestDocumentD>(context, (uint)data.Count);

//            // postconditions
//            documentStore.Dispose();
//            Directory.Delete(context.IndexDirectory, true);
//        }

//        /// <summary>
//        /// Opens an existing document store.
//        /// </summary>
//        [Fact]
//        public void OpenE()
//        {
//            // preconditions
//            var context = new IndexContext();
//            context.IndexDirectory = Path.Combine(context.IndexDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
//            var data = UnitTestIndexTestDocumentFactoryE.GenerateTestData();
//            var documentStore = new IndexStorageDocumentStore<UnitTestIndexTestDocumentE>(context, (uint)data.Count);

//            foreach (var item in data)
//            {
//                documentStore.Add(item);
//            }

//            documentStore.Dispose();

//            // test execution
//            documentStore = new IndexStorageDocumentStore<UnitTestIndexTestDocumentE>(context, (uint)data.Count);

//            // postconditions
//            documentStore.Dispose();
//            Directory.Delete(context.IndexDirectory, true);
//        }

//        /// <summary>
//        /// Adds items to a document store.
//        /// </summary>
//        [Fact]
//        public void AddA()
//        {
//            // preconditions
//            var context = new IndexContext();
//            context.IndexDirectory = Path.Combine(context.IndexDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
//            var data = UnitTestIndexTestDocumentFactoryA.GenerateTestData();
//            var documentStore = new IndexStorageDocumentStore<UnitTestIndexTestDocumentA>(context, 5);

//            documentStore.Clear();

//            // test execution
//            documentStore.Add(data[0]);
//            documentStore.Add(data[1]);

//            Assert.NotNull(documentStore);

//            // postconditions
//            documentStore.Dispose();
//            Directory.Delete(context.IndexDirectory, true);
//        }

//        /// <summary>
//        /// Adds items to a document store.
//        /// </summary>
//        [Fact]
//        public void AddB()
//        {
//            // preconditions
//            var context = new IndexContext();
//            context.IndexDirectory = Path.Combine(context.IndexDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));

//            // test execution
//            var data = UnitTestIndexTestDocumentFactoryB.GenerateTestData();
//            var randomItem = data[new Random().Next() % data.Count];
//            var documentStore = new IndexStorageDocumentStore<UnitTestIndexTestDocumentB>(context, (uint)data.Count);

//            foreach (var item in data)
//            {
//                documentStore.Add(item);
//            }

//            var i = documentStore.GetItem(randomItem.Id);

//            Assert.True(i != null && i.Id == randomItem.Id);

//            // postconditions
//            documentStore.Dispose();
//            Directory.Delete(context.IndexDirectory, true);
//        }

//        /// <summary>
//        /// Adds items to a document store.
//        /// </summary>
//        [Fact]
//        public void AddC()
//        {
//            // preconditions
//            var context = new IndexContext();
//            context.IndexDirectory = Path.Combine(context.IndexDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));

//            // test execution
//            var data = UnitTestIndexTestDocumentFactoryC.GenerateTestData();
//            var randomItem = data.Skip(new Random().Next() % data.Count()).FirstOrDefault();
//            var documentStore = new IndexStorageDocumentStore<UnitTestIndexTestDocumentC>(context, (uint)data.Count);

//            foreach (var item in data)
//            {
//                documentStore.Add(item);
//            }

//            var i = documentStore.GetItem(randomItem.Id);

//            Assert.True(i != null && i.Id == randomItem.Id);

//            // postconditions
//            documentStore.Dispose();
//            Directory.Delete(context.IndexDirectory, true);
//        }

//        /// <summary>
//        /// Adds items to a document store.
//        /// </summary>
//        [Fact]
//        public void AddD()
//        {
//            // preconditions
//            var context = new IndexContext();
//            context.IndexDirectory = Path.Combine(context.IndexDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));

//            // test execution
//            var data = UnitTestIndexTestDocumentFactoryD.GenerateTestData();
//            var randomItem = data[new Random().Next() % data.Count];
//            var documentStore = new IndexStorageDocumentStore<UnitTestIndexTestDocumentD>(context, (uint)data.Count);

//            foreach (var item in data)
//            {
//                documentStore.Add(item);
//            }

//            var i = documentStore.GetItem(randomItem.Id);

//            Assert.True(i != null && i.Id == randomItem.Id);

//            // postconditions
//            documentStore.Dispose();
//            Directory.Delete(context.IndexDirectory, true);
//        }

//        /// <summary>
//        /// Adds items to a document store.
//        /// </summary>
//        [Fact]
//        public void AddE()
//        {
//            // preconditions
//            var context = new IndexContext();
//            context.IndexDirectory = Path.Combine(context.IndexDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));

//            // test execution
//            var data = UnitTestIndexTestDocumentFactoryE.GenerateTestData();
//            var randomItem = data[new Random().Next() % data.Count];
//            var documentStore = new IndexStorageDocumentStore<UnitTestIndexTestDocumentE>(context, (uint)data.Count);

//            foreach (var item in data)
//            {
//                documentStore.Add(item);
//            }

//            var i = documentStore.GetItem(randomItem.Id);

//            Assert.True(i != null && i.Id == randomItem.Id);

//            // postconditions
//            documentStore.Dispose();
//            Directory.Delete(context.IndexDirectory, true);
//        }

//        /// <summary>
//        /// Update an entry in the reverse index where the item has a first name change.
//        /// </summary>
//        [Fact]
//        public void UpdateWithChangeA()
//        {
//            // preconditions
//            var context = new IndexContext();
//            context.IndexDirectory = Path.Combine(context.IndexDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
//            var property = typeof(UnitTestIndexTestDocumentA).GetProperty("Name");
//            var data = UnitTestIndexTestDocumentFactoryA.GenerateTestData();
//            var documentStore = new IndexStorageDocumentStore<UnitTestIndexTestDocumentA>(context, 5);

//            documentStore.Clear();
//            documentStore.Add(data[0]);
//            documentStore.Add(data[1]);

//            // test execution
//            data[0].Name = "Update_" + data[0].Name;

//            documentStore.Update(data[0]);
//            var all = documentStore.All;

//            Assert.True(all.Select(x => x.Id).OrderBy(x => x).SequenceEqual(data.Select(x => x.Id).OrderBy(x => x)));
//            Assert.True(all.Where(x => x.Name == data[0].Name).Any());

//            // postconditions
//            documentStore.Dispose();
//            Directory.Delete(context.IndexDirectory, true);
//        }

//        /// <summary>
//        /// Update an entry in the reverse index where the item has a first name change.
//        /// </summary>
//        [Fact]
//        public void UpdateWithChangeB()
//        {
//            // preconditions
//            var context = new IndexContext();
//            context.IndexDirectory = Path.Combine(context.IndexDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
//            var property = typeof(UnitTestIndexTestDocumentB).GetProperty("Name");
//            var data = UnitTestIndexTestDocumentFactoryB.GenerateTestData();
//            var randomItem = data[new Random().Next() % data.Count];
//            var documentStore = new IndexStorageDocumentStore<UnitTestIndexTestDocumentB>(context, (uint)data.Count);

//            foreach (var item in data)
//            {
//                documentStore.Add(item);
//            }

//            // test execution
//            randomItem.Name = "Update_" + randomItem.Name;

//            documentStore.Update(randomItem);
//            var all = documentStore.All;

//            Assert.True(all.Select(x => x.Id).OrderBy(x => x).SequenceEqual(data.Select(x => x.Id).OrderBy(x => x)));
//            Assert.True(all.Where(x => x.Name == randomItem.Name).Any());

//            // postconditions
//            documentStore.Dispose();
//            Directory.Delete(context.IndexDirectory, true);
//        }

//        /// <summary>
//        /// Update an entry in the reverse index where the item has a first name change.
//        /// </summary>
//        [Fact]
//        public void UpdateWithChangesC()
//        {
//            // preconditions
//            var context = new IndexContext();
//            context.IndexDirectory = Path.Combine(context.IndexDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
//            var property = typeof(UnitTestIndexTestDocumentC).GetProperty("Text");
//            var data = UnitTestIndexTestDocumentFactoryC.GenerateTestData();
//            var randomItem = data[new Random().Next() % data.Count];
//            var documentStore = new IndexStorageDocumentStore<UnitTestIndexTestDocumentC>(context, (uint)data.Count);

//            foreach (var item in data)
//            {
//                documentStore.Add(item);
//            }

//            // test execution
//            randomItem.Text = "Update_" + randomItem.Text;

//            documentStore.Update(randomItem);
//            var all = documentStore.All;

//            Assert.True(all.Select(x => x.Id).OrderBy(x => x).SequenceEqual(data.Select(x => x.Id).OrderBy(x => x)));
//            Assert.True(all.Where(x => x.Text == randomItem.Text).Any());

//            // postconditions
//            documentStore.Dispose();
//            Directory.Delete(context.IndexDirectory, true);
//        }

//        /// <summary>
//        /// Update an entry in the document store where the item has a first name change.
//        /// </summary>
//        [Fact]
//        public void UpdateWithChangesD()
//        {
//            // preconditions
//            var context = new IndexContext();
//            context.IndexDirectory = Path.Combine(context.IndexDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
//            var data = UnitTestIndexTestDocumentFactoryD.GenerateTestData();
//            var randomItem = data[new Random().Next() % data.Count];
//            var documentStore = new IndexStorageDocumentStore<UnitTestIndexTestDocumentD>(context, (uint)data.Count);

//            foreach (var item in data)
//            {
//                documentStore.Add(item);
//            }

//            // test execution
//            randomItem.FirstName = "Update_" + randomItem.FirstName;

//            documentStore.Update(randomItem);
//            var all = documentStore.All;

//            Assert.True(all.Select(x => x.Id).OrderBy(x => x).SequenceEqual(data.Select(x => x.Id).OrderBy(x => x)));
//            Assert.True(all.Where(x => x.FirstName == randomItem.FirstName).Any());

//            // postconditions
//            documentStore.Dispose();
//            Directory.Delete(context.IndexDirectory, true);
//        }

//        /// <summary>
//        /// Update an entry in the document store where the item has a first name change.
//        /// </summary>
//        [Fact]
//        public void UpdateWithChangesE()
//        {
//            // preconditions
//            var context = new IndexContext();
//            context.IndexDirectory = Path.Combine(context.IndexDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
//            var data = UnitTestIndexTestDocumentFactoryE.GenerateTestData();
//            var randomItem = data[new Random().Next() % data.Count];
//            var documentStore = new IndexStorageDocumentStore<UnitTestIndexTestDocumentE>(context, (uint)data.Count);

//            foreach (var item in data)
//            {
//                documentStore.Add(item);
//            }

//            // test execution
//            randomItem.Name = "Update_" + randomItem.Name;

//            documentStore.Update(randomItem);
//            var all = documentStore.All;

//            Assert.True(all.Select(x => x.Id).OrderBy(x => x).SequenceEqual(data.Select(x => x.Id).OrderBy(x => x)));
//            Assert.True(all.Where(x => x.Name == randomItem.Name).Any());

//            // postconditions
//            documentStore.Dispose();
//            Directory.Delete(context.IndexDirectory, true);
//        }

//        /// <summary>
//        /// Changes an entry in the reverse index without the element to be changed having any changes.
//        /// </summary>
//        [Fact]
//        public void UpdateWithoutChangesA()
//        {
//            // preconditions
//            var context = new IndexContext();
//            context.IndexDirectory = Path.Combine(context.IndexDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
//            var property = typeof(UnitTestIndexTestDocumentA).GetProperty("Name");
//            var data = UnitTestIndexTestDocumentFactoryA.GenerateTestData();
//            var documentStore = new IndexStorageDocumentStore<UnitTestIndexTestDocumentA>(context, 5);

//            documentStore.Clear();
//            documentStore.Add(data[0]);
//            documentStore.Add(data[1]);

//            // test execution
//            documentStore.Update(data[0]);
//            var all = documentStore.All;

//            Assert.True(all.Select(x => x.Id).OrderBy(x => x).SequenceEqual(data.Select(x => x.Id).OrderBy(x => x)));
//            Assert.True(all.Where(x => x.Name == data[0].Name).Any());

//            // postconditions
//            documentStore.Dispose();
//            Directory.Delete(context.IndexDirectory, true);
//        }

//        /// <summary>
//        /// Changes an entry in the reverse index without the element to be changed having any changes.
//        /// </summary>
//        [Fact]
//        public void UpdateWithoutChangesB()
//        {
//            // preconditions
//            var context = new IndexContext();
//            context.IndexDirectory = Path.Combine(context.IndexDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
//            var property = typeof(UnitTestIndexTestDocumentB).GetProperty("Name");
//            var data = UnitTestIndexTestDocumentFactoryB.GenerateTestData();
//            var randomItem = data[new Random().Next() % data.Count];
//            var documentStore = new IndexStorageDocumentStore<UnitTestIndexTestDocumentB>(context, (uint)data.Count);

//            foreach (var item in data)
//            {
//                documentStore.Add(item);
//            }

//            // test execution
//            documentStore.Update(randomItem);
//            var all = documentStore.All;

//            Assert.True(all.Select(x => x.Id).OrderBy(x => x).SequenceEqual(data.Select(x => x.Id).OrderBy(x => x)));
//            Assert.True(all.Where(x => x.Name == randomItem.Name).Any());

//            // postconditions
//            documentStore.Dispose();
//            Directory.Delete(context.IndexDirectory, true);
//        }

//        /// <summary>
//        /// Changes an entry in the reverse index without the element to be changed having any changes.
//        /// </summary>
//        [Fact]
//        public void UpdateWithoutChangesC()
//        {
//            // preconditions
//            var context = new IndexContext();
//            context.IndexDirectory = Path.Combine(context.IndexDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
//            var property = typeof(UnitTestIndexTestDocumentC).GetProperty("Text");
//            var data = UnitTestIndexTestDocumentFactoryC.GenerateTestData();
//            var randomItem = data[new Random().Next() % data.Count];
//            var documentStore = new IndexStorageDocumentStore<UnitTestIndexTestDocumentC>(context, (uint)data.Count);

//            foreach (var item in data)
//            {
//                documentStore.Add(item);
//            }

//            // test execution
//            documentStore.Update(data[0]);
//            var all = documentStore.All;

//            Assert.True(all.Select(x => x.Id).OrderBy(x => x).SequenceEqual(data.Select(x => x.Id).OrderBy(x => x)));
//            Assert.True(all.Where(x => x.Text == randomItem.Text).Any());


//            // postconditions
//            documentStore.Dispose();
//            Directory.Delete(context.IndexDirectory, true);
//        }

//        /// <summary>
//        /// Changes an entry in the document store without the element to be changed having any changes.
//        /// </summary>
//        [Fact]
//        public void UpdateWithoutChangesD()
//        {
//            // preconditions
//            var context = new IndexContext();
//            context.IndexDirectory = Path.Combine(context.IndexDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
//            var data = UnitTestIndexTestDocumentFactoryD.GenerateTestData();
//            var randomItem = data[new Random().Next() % data.Count];
//            var documentStore = new IndexStorageDocumentStore<UnitTestIndexTestDocumentD>(context, (uint)data.Count);

//            foreach (var item in data)
//            {
//                documentStore.Add(item);
//            }

//            // test execution
//            documentStore.Update(randomItem);
//            var all = documentStore.All;

//            Assert.True(all.Select(x => x.Id).OrderBy(x => x).SequenceEqual(data.Select(x => x.Id).OrderBy(x => x)));
//            Assert.True(all.Where(x => x.FirstName == randomItem.FirstName).Any());

//            // postconditions
//            documentStore.Dispose();
//            Directory.Delete(context.IndexDirectory, true);
//        }

//        /// <summary>
//        /// Changes an entry in the document store without the element to be changed having any changes.
//        /// </summary>
//        [Fact]
//        public void UpdateWithoutChangesE()
//        {
//            // preconditions
//            var context = new IndexContext();
//            context.IndexDirectory = Path.Combine(context.IndexDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
//            var data = UnitTestIndexTestDocumentFactoryE.GenerateTestData();
//            var randomItem = data[new Random().Next() % data.Count];
//            var documentStore = new IndexStorageDocumentStore<UnitTestIndexTestDocumentE>(context, (uint)data.Count);

//            foreach (var item in data)
//            {
//                documentStore.Add(item);
//            }

//            // test execution
//            documentStore.Update(randomItem);
//            var all = documentStore.All;

//            Assert.True(all.Select(x => x.Id).OrderBy(x => x).SequenceEqual(data.Select(x => x.Id).OrderBy(x => x)));
//            Assert.True(all.Where(x => x.Name == randomItem.Name).Any());

//            // postconditions
//            documentStore.Dispose();
//            Directory.Delete(context.IndexDirectory, true);
//        }

//        /// <summary>
//        /// Removes an entry from the document store.
//        /// </summary>
//        [Fact]
//        public void RemoveA()
//        {
//            // preconditions
//            var context = new IndexContext();
//            context.IndexDirectory = Path.Combine(context.IndexDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
//            var data = UnitTestIndexTestDocumentFactoryA.GenerateTestData();
//            var documentStore = new IndexStorageDocumentStore<UnitTestIndexTestDocumentA>(context, 5);

//            documentStore.Clear();
//            documentStore.Add(data[0]);
//            documentStore.Add(data[1]);

//            var item = documentStore.GetItem(data[0].Id);

//            Assert.NotNull(documentStore);
//            Assert.NotNull(item);

//            // test execution
//            documentStore.Remove(item);

//            item = documentStore.GetItem(data[0].Id);
//            Assert.Null(item);

//            // postconditions
//            documentStore.Dispose();
//            Directory.Delete(context.IndexDirectory, true);
//        }

//        /// <summary>
//        /// Removes an entry from the document store.
//        /// </summary>
//        [Fact]
//        public void RemoveB()
//        {
//            // preconditions
//            var context = new IndexContext();
//            context.IndexDirectory = Path.Combine(context.IndexDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));

//            var data = UnitTestIndexTestDocumentFactoryB.GenerateTestData();
//            var randomItem = data[new Random().Next() % data.Count];
//            var documentStore = new IndexStorageDocumentStore<UnitTestIndexTestDocumentB>(context, (uint)data.Count);

//            foreach (var item in data)
//            {
//                documentStore.Add(item);
//            }

//            // test execution
//            documentStore.Remove(randomItem);
//            var all = documentStore.All;

//            Assert.True(all.Select(x => x.Id).OrderBy(x => x).SequenceEqual(data.Where(x => x.Id != randomItem.Id).Select(x => x.Id).OrderBy(x => x)));

//            // postconditions
//            documentStore.Dispose();
//            Directory.Delete(context.IndexDirectory, true);
//        }

//        /// <summary>
//        /// Removes an entry from the document store.
//        /// </summary>
//        [Fact]
//        public void RemoveC()
//        {
//            // preconditions
//            var context = new IndexContext();
//            context.IndexDirectory = Path.Combine(context.IndexDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));

//            var data = UnitTestIndexTestDocumentFactoryC.GenerateTestData();
//            var randomItem = data.Skip(new Random().Next() % data.Count()).FirstOrDefault();
//            var documentStore = new IndexStorageDocumentStore<UnitTestIndexTestDocumentC>(context, (uint)data.Count);

//            foreach (var item in data)
//            {
//                documentStore.Add(item);
//            }

//            // test execution
//            documentStore.Remove(randomItem);
//            var all = documentStore.All;

//            Assert.True(all.Select(x => x.Id).OrderBy(x => x).SequenceEqual(data.Where(x => x.Id != randomItem.Id).Select(x => x.Id).OrderBy(x => x)));

//            // postconditions
//            documentStore.Dispose();
//            Directory.Delete(context.IndexDirectory, true);
//        }

//        /// <summary>
//        /// Removes an entry from the document store.
//        /// </summary>
//        [Fact]
//        public void RemoveD()
//        {
//            // preconditions
//            var context = new IndexContext();
//            context.IndexDirectory = Path.Combine(context.IndexDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));

//            var data = UnitTestIndexTestDocumentFactoryD.GenerateTestData();
//            var randomItem = data[new Random().Next() % data.Count];
//            var documentStore = new IndexStorageDocumentStore<UnitTestIndexTestDocumentD>(context, (uint)data.Count);

//            foreach (var item in data)
//            {
//                documentStore.Add(item);
//            }

//            // test execution
//            documentStore.Remove(randomItem);
//            var all = documentStore.All;

//            Assert.True(all.Select(x => x.Id).OrderBy(x => x).SequenceEqual(data.Where(x => x.Id != randomItem.Id).Select(x => x.Id).OrderBy(x => x)));

//            // postconditions
//            documentStore.Dispose();
//            Directory.Delete(context.IndexDirectory, true);
//        }

//        /// <summary>
//        /// Removes an entry from the document store.
//        /// </summary>
//        [Fact]
//        public void RemoveE()
//        {
//            // preconditions
//            var context = new IndexContext();
//            context.IndexDirectory = Path.Combine(context.IndexDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));

//            var data = UnitTestIndexTestDocumentFactoryE.GenerateTestData();
//            var randomItem = data[new Random().Next() % data.Count];
//            var documentStore = new IndexStorageDocumentStore<UnitTestIndexTestDocumentE>(context, (uint)data.Count);

//            foreach (var item in data)
//            {
//                documentStore.Add(item);
//            }

//            // test execution
//            documentStore.Remove(randomItem);
//            var all = documentStore.All;

//            Assert.True(all.Select(x => x.Id).OrderBy(x => x).SequenceEqual(data.Where(x => x.Id != randomItem.Id).Select(x => x.Id).OrderBy(x => x)));

//            // postconditions
//            documentStore.Dispose();
//            Directory.Delete(context.IndexDirectory, true);
//        }

//        /// <summary>
//        /// Retrieve a entry of the document store.
//        /// </summary>
//        [Fact]
//        public void RetrieveA()
//        {
//            // preconditions
//            var context = new IndexContext();
//            context.IndexDirectory = Path.Combine(context.IndexDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
//            var data = UnitTestIndexTestDocumentFactoryA.GenerateTestData();
//            var documentStore = new IndexStorageDocumentStore<UnitTestIndexTestDocumentA>(context, 5);

//            documentStore.Clear();
//            documentStore.Add(data[0]);
//            documentStore.Add(data[1]);

//            // test execution
//            var item = documentStore.GetItem(data[0].Id);

//            Assert.NotNull(documentStore);
//            Assert.NotNull(item);

//            // postconditions
//            documentStore.Dispose();
//            Directory.Delete(context.IndexDirectory, true);
//        }

//        /// <summary>
//        /// Retrieve a entry of the document store.
//        /// </summary>
//        [Fact]
//        public void RetrieveB()
//        {
//            // preconditions
//            var context = new IndexContext();
//            context.IndexDirectory = Path.Combine(context.IndexDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
//            var data = UnitTestIndexTestDocumentFactoryB.GenerateTestData();
//            var documentStore = new IndexStorageDocumentStore<UnitTestIndexTestDocumentB>(context, (uint)data.Count);

//            foreach (var i in data)
//            {
//                documentStore.Add(i);
//            }

//            // test execution
//            var item = documentStore.GetItem(data.FirstOrDefault().Id);

//            Assert.NotNull(documentStore);
//            Assert.NotNull(item);

//            // postconditions
//            documentStore.Dispose();
//            Directory.Delete(context.IndexDirectory, true);
//        }

//        /// <summary>
//        /// Retrieve a entry of the document store.
//        /// </summary>
//        [Fact]
//        public void RetrieveC()
//        {
//            // preconditions
//            var context = new IndexContext();
//            context.IndexDirectory = Path.Combine(context.IndexDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
//            var data = UnitTestIndexTestDocumentFactoryC.GenerateTestData();
//            var documentStore = new IndexStorageDocumentStore<UnitTestIndexTestDocumentC>(context, (uint)data.Count);

//            foreach (var i in data)
//            {
//                documentStore.Add(i);
//            }

//            // test execution
//            var item = documentStore.GetItem(data.FirstOrDefault().Id);

//            Assert.NotNull(documentStore);
//            Assert.NotNull(item);

//            // postconditions
//            documentStore.Dispose();
//            Directory.Delete(context.IndexDirectory, true);
//        }

//        /// <summary>
//        /// Retrieve a entry of the document store.
//        /// </summary>
//        [Fact]
//        public void RetrieveD()
//        {
//            // preconditions
//            var context = new IndexContext();
//            context.IndexDirectory = Path.Combine(context.IndexDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
//            var data = UnitTestIndexTestDocumentFactoryD.GenerateTestData();
//            var documentStore = new IndexStorageDocumentStore<UnitTestIndexTestDocumentD>(context, (uint)data.Count);

//            foreach (var i in data)
//            {
//                documentStore.Add(i);
//            }

//            // test execution
//            var item = documentStore.GetItem(data[0].Id);

//            Assert.NotNull(documentStore);
//            Assert.NotNull(item);

//            // postconditions
//            documentStore.Dispose();
//            Directory.Delete(context.IndexDirectory, true);
//        }

//        /// <summary>
//        /// Retrieve a entry of the document store.
//        /// </summary>
//        [Fact]
//        public void RetrieveE()
//        {
//            // preconditions
//            var context = new IndexContext();
//            context.IndexDirectory = Path.Combine(context.IndexDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
//            var data = UnitTestIndexTestDocumentFactoryE.GenerateTestData();
//            var documentStore = new IndexStorageDocumentStore<UnitTestIndexTestDocumentE>(context, (uint)data.Count);

//            foreach (var i in data)
//            {
//                documentStore.Add(i);
//            }

//            // test execution
//            var item = documentStore.GetItem(data[0].Id);

//            Assert.NotNull(documentStore);
//            Assert.NotNull(item);

//            // postconditions
//            documentStore.Dispose();
//            Directory.Delete(context.IndexDirectory, true);
//        }

//        /// <summary>
//        /// Return all entries of the document store.
//        /// </summary>
//        [Fact]
//        public void AllA()
//        {
//            // preconditions
//            var context = new IndexContext();
//            context.IndexDirectory = Path.Combine(context.IndexDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
//            var data = UnitTestIndexTestDocumentFactoryA.GenerateTestData();
//            var documentStore = new IndexStorageDocumentStore<UnitTestIndexTestDocumentA>(context, 5);

//            documentStore.Clear();
//            documentStore.Add(data[0]);
//            documentStore.Add(data[1]);

//            // test execution
//            var all = documentStore.All;

//            Assert.True(all.Select(x => x.Id).OrderBy(x => x).SequenceEqual(data.Select(x => x.Id).OrderBy(x => x)));

//            // postconditions
//            documentStore.Dispose();
//            Directory.Delete(context.IndexDirectory, true);
//        }

//        /// <summary>
//        /// Return all entries of the document store.
//        /// </summary>
//        [Fact]
//        public void AllB()
//        {
//            // preconditions
//            var context = new IndexContext();
//            context.IndexDirectory = Path.Combine(context.IndexDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));

//            var data = UnitTestIndexTestDocumentFactoryB.GenerateTestData();
//            var documentStore = new IndexStorageDocumentStore<UnitTestIndexTestDocumentB>(context, (uint)data.Count);

//            foreach (var item in data)
//            {
//                documentStore.Add(item);
//            }

//            // test execution
//            var all = documentStore.All;

//            Assert.True(all.Select(x => x.Id).OrderBy(x => x).SequenceEqual(data.Select(x => x.Id).OrderBy(x => x)));

//            // postconditions
//            documentStore.Dispose();
//            Directory.Delete(context.IndexDirectory, true);
//        }

//        /// <summary>
//        /// Return all entries of the document store.
//        /// </summary>
//        [Fact]
//        public void AllC()
//        {
//            // preconditions
//            var context = new IndexContext();
//            context.IndexDirectory = Path.Combine(context.IndexDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));

//            var data = UnitTestIndexTestDocumentFactoryC.GenerateTestData();
//            var documentStore = new IndexStorageDocumentStore<UnitTestIndexTestDocumentC>(context, (uint)data.Count);

//            foreach (var item in data)
//            {
//                documentStore.Add(item);
//            }

//            // test execution
//            var all = documentStore.All;

//            Assert.True(all.Select(x => x.Id).OrderBy(x => x).SequenceEqual(data.Select(x => x.Id).OrderBy(x => x)));

//            // postconditions
//            documentStore.Dispose();
//            Directory.Delete(context.IndexDirectory, true);
//        }

//        /// <summary>
//        /// Return all entries of the document store.
//        /// </summary>
//        [Fact]
//        public void AllD()
//        {
//            // preconditions
//            var context = new IndexContext();
//            context.IndexDirectory = Path.Combine(context.IndexDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));

//            var data = UnitTestIndexTestDocumentFactoryD.GenerateTestData();
//            var documentStore = new IndexStorageDocumentStore<UnitTestIndexTestDocumentD>(context, (uint)data.Count);

//            foreach (var item in data)
//            {
//                documentStore.Add(item);
//            }

//            // test execution
//            var all = documentStore.All;

//            Assert.True(all.Select(x => x.Id).OrderBy(x => x).SequenceEqual(data.Select(x => x.Id).OrderBy(x => x)));

//            // postconditions
//            documentStore.Dispose();
//            Directory.Delete(context.IndexDirectory, true);
//        }

//        /// <summary>
//        /// Return all entries of the document store.
//        /// </summary>
//        [Fact]
//        public void AllE()
//        {
//            // preconditions
//            var context = new IndexContext();
//            context.IndexDirectory = Path.Combine(context.IndexDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));

//            var data = UnitTestIndexTestDocumentFactoryE.GenerateTestData();
//            var documentStore = new IndexStorageDocumentStore<UnitTestIndexTestDocumentE>(context, (uint)data.Count);

//            foreach (var item in data)
//            {
//                documentStore.Add(item);
//            }

//            // test execution
//            var all = documentStore.All;

//            Assert.True(all.Select(x => x.Id).OrderBy(x => x).SequenceEqual(data.Select(x => x.Id).OrderBy(x => x)));

//            // postconditions
//            documentStore.Dispose();
//            Directory.Delete(context.IndexDirectory, true);
//        }
//    }
//}
