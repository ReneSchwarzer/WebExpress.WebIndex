namespace WebExpress.WebIndex
{
    public interface IWebIndexContext
    {
        /// <summary>
        /// Returns the data directory where the index data is located.
        /// </summary>
        public string IndexDirectory { get; }
    }
}
