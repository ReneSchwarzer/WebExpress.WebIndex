namespace WebExpress.WebIndex.Memory
{
    /// <summary>
    /// Represents a index schema file.
    /// </summary>
    /// <typeparam name="TIndexItem">The data type. This must have the IIndexItem interface.</typeparam>
    public class IndexMemorySchema<TIndexItem> : IIndexSchema<TIndexItem> where TIndexItem : IIndexItem
    {
        /// <summary>
        /// Returns the index context.
        /// </summary>
        public IIndexContext Context { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexSchema"/> class.
        /// </summary>
        /// <param name="context">The index context.</param>
        public IndexMemorySchema(IIndexContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Checks if the schema of the object has changed.
        /// </summary>
        /// <returns>
        /// Returns allways true.
        /// </returns>
        public bool HasSchemaChanged()
        {
            return false;
        }

        /// <summary>
        /// Migrates the schema if it has changed.
        /// </summary>
        public void Migrate()
        {
        }

        /// <summary>
        /// Delete this file from storage.
        /// </summary>
        public void Drop()
        {
            Dispose();
        }

        /// <summary>
        /// Is called to free up resources.
        /// </summary>
        public virtual void Dispose()
        {

        }
    }
}