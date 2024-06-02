﻿using System;
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
    /// <summary>
    /// The IndexManager serves as the primary component for interacting with the indexing functions (CRUD).
    /// </summary>
    public abstract class IndexManager : IDisposable
    {
        /// <summary>
        /// Returns an enumeration of the existing index documents.
        /// </summary>
        private Dictionary<Type, IIndexDocument> Documents { get; } = [];

        /// <summary>
        /// Returns the index context.
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
        /// Initialization of the IndexManager.
        /// </summary>
        /// <param name="context">The reference to the context.</param>
        public void Initialization(IIndexContext context)
        {
            Context = context;
            Directory.CreateDirectory(Context.IndexDirectory);
            TokenAnalyzer = new IndexTokenAnalyzer(context);
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
        /// Reindexing the index.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
        /// <param name="items">The data to be added to the index.</param>
        public void ReIndex<T>(IEnumerable<T> items) where T : IIndexItem
        {
            if (GetIndexDocument<T>() is IIndexDocument<T> document)
            {
                foreach (var item in items)
                {
                    document.Add(item);
                };
            }
        }

        /// <summary>
        /// Performs an asynchronous reindexing of a collection of index items.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
        /// <param name="items">The collection of items to be re-indexed.</param>
        /// <param name="progress">An optional IProgress object that tracks the progress of the re-indexing.</param>
        /// <param name="token">An optional CancellationToken that is used to cancel the re-indexing.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ReIndexAsync<T>(IEnumerable<T> items, IProgress<int> progress = null, CancellationToken token = default(CancellationToken)) where T : IIndexItem
        {
            if (GetIndexDocument<T>() is IIndexDocument<T> document)
            {
                int i = 0;
                var count = items.Count();

                foreach (var item in items)
                {
                    await document.AddAsync(item);

                    if (progress != null)
                    {
                        var percent = (i++ / (float)count) * 100;
                        progress.Report((int)percent);
                    }

                    if (token.IsCancellationRequested)
                    {
                        return;
                    }
                };
            }
        }

        /// <summary>
        /// Registers a data type in the index.
        /// </summary>
        /// <param name="dataType">The data type. This must have the IIndexItem interface.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="type">The index type.</param>
        public void Create(Type dataType, CultureInfo culture, IndexType type = IndexType.Memory)
        {
            var genericMethod = typeof(IndexManager).GetMethod("Create", 1, [typeof(CultureInfo), typeof(IndexType)]);
            var specificMethod = genericMethod.MakeGenericMethod(dataType);

            specificMethod.Invoke(this, [culture, type]);
        }

        /// <summary>
        /// Registers a data type in the index.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
        /// <param name="culture">The culture.</param>
        /// <param name="type">The index type.</param>
        public void Create<T>(CultureInfo culture, IndexType type = IndexType.Memory) where T : IIndexItem
        {
            if (!Documents.ContainsKey(typeof(T)))
            {
                var context = new IndexDocumemntContext(Context, TokenAnalyzer);

                Documents.Add(typeof(T), new IndexDocument<T>(context, type, culture));
            }
        }

        /// <summary>
        /// Removes all index documents of type.
        /// </summary>
        /// <param name="dataType">The data type. This must have the IIndexItem interface.</param>
        public void Drop(Type dataType)
        {
            var genericMethod = typeof(IndexManager).GetMethod("Drop", 1, []);
            var specificMethod = genericMethod.MakeGenericMethod(dataType);

            specificMethod.Invoke(this, []);
        }

        /// <summary>
        /// Removes all index documents of type T.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
        public void Drop<T>() where T : IIndexItem
        {
            if (GetIndexDocument<T>() != null)
            {
                Documents.Remove(typeof(T), out IIndexDocument value);

                value.Dispose();
            }
        }

        /// <summary>
        /// Asynchronously removes all index documents of type T.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task DropAsync<T>() where T : IIndexItem
        {
            if (GetIndexDocument<T>() != null)
            {
                await Task.Run(() =>
                {
                    Documents.Remove(typeof(T), out IIndexDocument value);

                    value.Dispose();
                });
            }
        }

        /// <summary>
        /// Adds a item to the index.
        /// </summary>
        /// <param name="dataType">The data type. This must have the IIndexItem interface.</param>
        /// <param name="item">The data to be added to the index.</param>
        public void Insert(Type dataType, object item)
        {
            var genericMethod = typeof(IndexManager).GetMethods()
                .Where(m => m.Name == "Insert" && m.IsGenericMethodDefinition)
                .First();
            var specificMethod = genericMethod.MakeGenericMethod(dataType);

            specificMethod.Invoke(this, [item]);
        }

        /// <summary>
        /// Adds a item to the index.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
        /// <param name="item">The data to be added to the index.</param>
        public void Insert<T>(T item) where T : IIndexItem
        {
            if (GetIndexDocument<T>() is IIndexDocument<T> document)
            {
                document.Add(item);
            }
        }

        /// <summary>
        /// Performs an asynchronous addition of an item in the index.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
        /// <param name="item">The data to be added to the index.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task InsertAsync<T>(T item) where T : IIndexItem
        {
            if (GetIndexDocument<T>() is IIndexDocument<T> document)
            {
                await document.AddAsync(item);
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
        /// Performs an asynchronous update of an item in the index.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
        /// <param name="item">The data to be updated to the index.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task UpdateAsync<T>(T item) where T : IIndexItem
        {
            if (GetIndexDocument<T>() is IIndexDocument<T> document)
            {
                await document.UpdateAsync(item);
            }
        }

        /// <summary>
        /// Removes an item from the index.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
        /// <param name="item">The data to be removed from the index.</param>
        public void Delete<T>(T item) where T : IIndexItem
        {
            if (GetIndexDocument<T>() is IIndexDocument<T> document)
            {
                document.Remove(item);
            }
        }

        /// <summary>
        /// Removes an item from the index asynchronously.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
        /// <param name="item">The data to be removed from the index.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task DeleteAsync<T>(T item) where T : IIndexItem
        {
            if (GetIndexDocument<T>() is IIndexDocument<T> document)
            {
                await document.RemoveAsync(item);
            }
        }
        /// <summary>
        /// Clear all data from index document.
        /// </summary>
        /// <param name="dataType">The data type. This must have the IIndexItem interface.</param>
        public void Clear(Type dataType)
        {
            var genericMethod = typeof(IndexManager).GetMethod("Clear", 1, []);
            var specificMethod = genericMethod.MakeGenericMethod(dataType);

            specificMethod.Invoke(this, []);
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
        /// Removed all data from the index asynchronously.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ClearAsync<T>() where T : IIndexItem
        {
            if (GetIndexDocument<T>() is IIndexDocument<T> document)
            {
                await document.ClearAsync();
            }
        }

        /// <summary>
        /// Executes a wql statement.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
        /// <param name="wql">The wql statement.</param>
        /// <returns>The WQL statement.</returns>
        public IWqlStatement<T> Retrieve<T>(string wql) where T : IIndexItem
        {
            if (GetIndexDocument<T>() is IIndexDocument<T> document)
            {
                var parser = new WqlParser<T>(document);
                return parser.Parse(wql);
            }

            return null;
        }

        /// <summary>
        /// Executes a wql statement asynchronously.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
        /// <param name="wql">The wql statement.</param>
        /// <returns>A task that represents the asynchronous operation using the WQL statement.</returns>
        public async Task<IWqlStatement<T>> RetrieveAsync<T>(string wql) where T : IIndexItem
        {
            if (GetIndexDocument<T>() is IIndexDocument<T> document)
            {
                return await Task.Run(() =>
                {
                    var parser = new WqlParser<T>(document);
                    return parser.Parse(wql);
                });
            }

            return null;
        }

        /// <summary>
        /// Returns all documents from the index.
        /// </summary>
        /// <param name="dataType">The data type. This must have the IIndexItem interface.</param>
        /// <returns>An enumeration of the documents</returns>
        public IEnumerable<object> All(Type dataType)
        {
            var genericMethod = typeof(IndexManager).GetMethod("All", 1, []);
            var specificMethod = genericMethod.MakeGenericMethod(dataType);

            return specificMethod.Invoke(this, []) as IEnumerable<object>;
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
        /// Returns an index type based on its type.
        /// </summary>
        /// <returns>The index type or null.</returns>
        public IIndexDocument<T> GetIndexDocument<T>() where T : IIndexItem
        {
            if (Documents.TryGetValue(typeof(T), out IIndexDocument res))
            {
                return res as IIndexDocument<T>;
            }

            return null;
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
