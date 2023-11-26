using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using WebExpress.WebIndex.Term.Filter;
using WebExpress.WebIndex.Wql;

namespace WebExpress.WebIndex
{
    public abstract class IndexManager
    {
        /// <summary>
        /// Returns an enumeration of the existing index documents.
        /// </summary>
        private Dictionary<Type, IIndexDocument> Documents { get; } = new Dictionary<Type, IIndexDocument>();

        /// <summary>
        /// Returns or sets the index context.
        /// </summary>
        public IIndexContext Context { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public IndexManager()
        {

        }

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="context">The reference to the context.</param>
        public void Initialization(IIndexContext context)
        {
            var assembly = typeof(IndexManager).Assembly;
            string[] fileNames = ["StopWords.en", "StopWords.de", "MisspelledWords.en", "MisspelledWords.de"];

            Context = context;

            Directory.CreateDirectory(Context.IndexDirectory);

            foreach (var fileName in fileNames)
            {
                var path = Path.Combine(Context.IndexDirectory, fileName.ToLower());
                var resources = assembly.GetManifestResourceNames();
                var resource = resources
                    .Where(x => x.EndsWith(fileName, StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefault();

                if (resource == null)
                {
                    continue;
                }

                using var file = File.Open(path, FileMode.OpenOrCreate);
                using var stream = assembly.GetManifestResourceStream(resource);
                stream.CopyTo(file);
            }

            IndexTermFilterStopWord.Initialization(context);
        }

        /// <summary>
        /// Registers a data type in the index.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
        /// <param name="culture">The culture.</param>
        /// <param name="capacity">The predicted capacity (number of items to store) of the index.</param>
        /// <param name="type">The index type.</param>
        public void Register<T>(CultureInfo culture, uint capacity = ushort.MaxValue, IndexType type = IndexType.Memory) where T : IIndexItem
        {
            if (!Documents.ContainsKey(typeof(T)))
            {
                Documents.Add(typeof(T), new IndexDocument<T>(Context, type, culture, capacity));
            }
        }

        /// <summary>
        /// Rebuilds the index.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
        /// <param name="items">The data to be added to the index.</param>
        public void ReIndex<T>(IEnumerable<T> items) where T : IIndexItem
        {
            foreach (var item in items)
            {
                Add(item);
            };
        }

        /// <summary>
        /// Adds a item to the index.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
        /// <param name="item">The data to be added to the index.</param>
        public void Add<T>(T item) where T : IIndexItem
        {
            if (GetIndexDocument<T>() is IIndexDocument<T> document)
            {
                document.Add(item);
            }
        }

        /// <summary>
        /// Updates a item in the index.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
        /// <param name="item">The data to be updated to the index.</param>
        public void Update<T>(T item) where T : IIndexItem
        {
            Remove(item);
            Add(item);
        }

        /// <summary>
        /// Removes a index document.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
        public void Remove<T>() where T : IIndexItem
        {
            if (GetIndexDocument<T>() != null)
            {
                Documents.Remove(typeof(T), out IIndexDocument value);

                value.Dispose();
            }
        }

        /// <summary>
        /// The data to be removed from the index.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
        /// <param name="item">The data to be removed from the index.</param>
        public void Remove<T>(T item) where T : IIndexItem
        {
            if (GetIndexDocument<T>() is IIndexDocument<T> document)
            {
                document.Remove(item);
            }
        }

        /// <summary>
        /// Executes a wql statement.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
        /// <param name="wql">Tje wql statement.</param>
        /// <returns>The WQL parser.</returns>
        public IWqlStatement<T> ExecuteWql<T>(string wql) where T : IIndexItem
        {
            if (GetIndexDocument<T>() is IIndexDocument<T> document)
            {
                var parser = new WqlParser<T>(document);
                return parser.Parse(wql);
            }

            return null;
        }

        /// <summary>
        /// Returns an index type based on its type.
        /// </summary>
        /// <returns>The index type or null.</returns>
        public IIndexDocument<T> GetIndexDocument<T>() where T : IIndexItem
        {
            return Documents.ContainsKey(typeof(T)) ? Documents[typeof(T)] as IIndexDocument<T> : null;
        }
    }
}
