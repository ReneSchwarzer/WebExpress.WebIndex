namespace WebExpress.WebIndex.Test.Fixture
{
    /// <summary>
    /// Provides a base class for unit test fixtures that require disposable resources.
    /// </summary>
    public class UnitTestIndexFixture : IDisposable
    {
        /// <summary>
        /// The random number generator.
        /// </summary>
        protected static Random Rand { get; } = new(10);

        /// <summary>
        /// Disposes of the resources used by the current instance.
        /// </summary>
        public virtual void Dispose()
        {
        }
    }
}
