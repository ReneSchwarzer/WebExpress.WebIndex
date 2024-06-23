namespace WebExpress.WebIndex.Test.Fixture
{
    public abstract class UnitTestIndexFixture : IDisposable
    {
        /// <summary>
        /// The random number generator.
        /// </summary>
        protected static Random Rand { get; } = new(10);

        /// <summary>
        /// Disposes of the resources used by the current instance.
        /// </summary>
        public abstract void Dispose();
    }
}
