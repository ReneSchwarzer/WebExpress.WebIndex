using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebExpress.WebIndex.Term;
using WebExpress.WebIndex.Term.Pipeline;
using WebExpress.WebIndex.Wql;

namespace WebExpress.WebIndex
{
    public abstract class IndexManager : IDisposable
    {
        /// <summary>
        /// Returns an enumeration of the existing index documents.
        /// </summary>
        private Dictionary<Type, IIndexDocument> Documents { get; } = [];

        /// <summary>
        /// Returns or sets the index context.
        /// </summary>
        public IIndexContext Context { get; private set; }

        /// <summary>
        /// Returns the token analyzer.
        /// </summary>
        protected IndexTokenAnalyzer TokenAnalyzer { get; private set; }

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
            Context = context;
            Directory.CreateDirectory(Context.IndexDirectory);
            TokenAnalyzer = new IndexTokenAnalyzer(context);
        }

        /// <summary>
        /// Registers a data type in the index.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
        /// <param name="culture">The culture.</param>
        /// <param name="type">The index type.</param>
        public void Register<T>(CultureInfo culture, IndexType type = IndexType.Memory) where T : IIndexItem
        {
            if (!Documents.ContainsKey(typeof(T)))
            {
                var context = new IndexDocumemntContext(Context, TokenAnalyzer);

                Documents.Add(typeof(T), new IndexDocument<T>(context, type, culture));
            }
        }

        /// <summary>
        /// Registers a pipe state for processing the tokens.
        /// </summary>
        /// <param name="pipeState">The pipe stage to add.</param>
        public void Register(IIndexPipeStage pipeStage)
        {
            TokenAnalyzer.Register(pipeStage);
        }

        /// <summary>
        /// Rebuilds the index.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
        /// <param name="items">The data to be added to the index.</param>
        public void ReIndex<T>(IEnumerable<T> items) where T : IIndexItem
        {
            if (GetIndexDocument<T>() is IIndexDocument<T> document)
            {
                document.ReBuild((uint)items.Count());

                foreach (var item in items)
                {
                    document.Add(item);
                };

                //Parallel.ForEach(items, document.Add);
            }
        }

        /// <summary>
        /// Performs an asynchronous re-indexing of a collection of index items.</summary>
        /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
        /// <param name="items">The collection of items to be re-indexed.</param>
        /// <param name="progress">An optional IProgress object that tracks the progress of the re-indexing.</param>
        /// <param name="token">An optional CancellationToken that is used to cancel the re-indexing.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public async Task ReIndexAsync<T>(IEnumerable<T> items, IProgress<int> progress = null, CancellationToken token = default(CancellationToken)) where T : IIndexItem
        {
            if (GetIndexDocument<T>() is IIndexDocument<T> document)
            {
                int i = 0;
                var count = items.Count();

                await document.ReBuildAsync((uint)count);

                foreach (var item in items)
                {
                    await document.AddAsync(item);

                    if (progress != null)
                    {
                        progress.Report(i++ / count * 100);
                    }

                    if (token.IsCancellationRequested)
                    {
                        return;
                    }
                };

                //var tasks = items.Select(async (item, index) =>
                //{
                //    if (token.IsCancellationRequested)
                //    {
                //        return;
                //    }

                //    await document.AddAsync(item);

                //    if (progress != null)
                //    {
                //        progress.Report((index + 1) / count * 100);
                //    }
                //});

                //await Task.WhenAll(tasks);
            }
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
            if (GetIndexDocument<T>() is IIndexDocument<T> document)
            {
                document.Update(item);
            }
        }

        /// <summary>
        /// Clear all data from index document.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
        public void Clear<T>() where T : IIndexItem
        {
            if (GetIndexDocument<T>() is IIndexDocument<T> document)
            {
                document.Clear();
            }
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
        /// Returns all documents from the index.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
        /// <returns>An enumeration of the documents</returns>
        public IEnumerable<T> All<T>() where T : IIndexItem
        {
            if (GetIndexDocument<T>() is IIndexDocument<T> document)
            {
                return document.All;
            }

            return [];
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

        /// <summary>
        /// Disposes of the resources used by the current instance.
        /// </summary>
        public void Dispose()
        {
            foreach (var document in Documents)
            {
                document.Value.Dispose();
            }

            Documents.Clear();
            TokenAnalyzer?.Dispose();
        }
    }
}
