using Xunit.Abstractions;

namespace WebExpress.WebIndex.Test.Index
{
    /// <summary>
    /// Test class for testing the memory-based document store.
    /// </summary>
    /// <param name="fixture">The log.</param>
    /// <param name="output">The test context.</param>
    public abstract class UnitTestIndexMemory<T>(T fixture, ITestOutputHelper output) : IClassFixture<T> where T : class
    {
        /// <summary>
        /// Returns the index manager.
        /// </summary>
        public IndexManager IndexManager { get; } = new IndexManagerTest();

        /// <summary>
        /// Returns the log.
        /// </summary>
        protected ITestOutputHelper Output { get; private set; } = output;

        /// <summary>
        /// Returns the test context.
        /// </summary>
        protected T Fixture { get; private set; } = fixture;

        /// <summary>
        /// It sets up the preconditions for a unit test.
        /// </summary>
        protected void Preconditions()
        {
            var context = new IndexContext();

            context.IndexDirectory = Path.Combine(context.IndexDirectory, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
            IndexManager.Initialization(context);
        }

        /// <summary>
        /// It performs cleanup tasks after a unit test.
        /// </summary>
        protected void Postconditions()
        {
            IndexManager.Dispose();
            Directory.Delete(IndexManager.Context.IndexDirectory, true);
        }
    }
}
