using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WebExpress.WebIndex.Memory;
using WebExpress.WebIndex.Storage;
using WebExpress.WebIndex.WebAttribute;

namespace WebExpress.WebIndex
{
    /// <summary>
    /// The IndexDocument is a segment that provides indexes (for each property of a data type).
    /// Key: The field name.
    /// Value: The reverse index.
    /// </summary>
    public class IndexDocument<T> : Dictionary<PropertyInfo, IIndexReverse<T>>, IIndexDocument<T> where T : IIndexItem
    {
        /// <summary>
        /// Event that is triggered when the schema has changed.
        /// </summary>
        public event EventHandler<IndexSchemaMigrationEventArgs> SchemaChanged;

        /// <summary>
        /// Returns the document store.
        /// </summary>
        public IIndexDocumentStore<T> DocumentStore { get; private set; }

        /// <summary>
        /// Returns the index schema associated with this index document.
        /// </summary>
        public IIndexSchema<T> Schema { get; private set; }

        /// <summary>
        /// Returns the index type.
        /// </summary>
        public IndexType IndexType { get; private set; }

        /// <summary>
        /// Return the index field names.
        /// </summary>
        public IEnumerable<string> Fields => Keys.Select(x => x.Name);

        /// <summary>
        /// Returns the index context.
        /// </summary>
        public IIndexDocumemntContext Context { get; private set; }

        /// <summary>
        /// Returns the culture.
        /// </summary>
        public CultureInfo Culture { get; private set; }

        /// <summary>
        /// Returns all documents from the index.
        /// </summary>
        public IEnumerable<T> All => DocumentStore.All;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">The index context.</param>
        /// <param name="indexType">The index type.</param>
        /// <param name="culture">The culture.</param>
        public IndexDocument(IIndexDocumemntContext context, IndexType indexType, CultureInfo culture)
        {
            Context = context;
            IndexType = indexType;
            Culture = culture;

            ReBuild(ushort.MaxValue);
        }

        /// <summary>
        /// Rebuilds the index.
        /// </summary>
        /// <param name="capacity">The predicted capacity (number of items to store) of the index.</param>
        protected virtual void ReBuild(uint capacity)
        {
            if (DocumentStore == null || capacity > DocumentStore.Capacity)
            {
                switch (IndexType)
                {
                    case IndexType.Memory:
                        {
                            Schema = new IndexMemorySchema<T>(Context);
                            DocumentStore = new IndexMemoryDocumentStore<T>(Context, capacity);

                            break;
                        }
                    default:
                        {
                            Schema = new IndexStorageSchema<T>(Context);
                            DocumentStore = new IndexStorageDocumentStore<T>(Context, capacity);

                            break;
                        }
                }

                if (Schema.HasSchemaChanged())
                {
                    var args = new IndexSchemaMigrationEventArgs
                    {
                        SchemaType = typeof(T),
                        PerformMigration = () =>
                        {
                            Schema.Migrate();

                            return true;
                        },
                        PerformMigrationAsync = async () =>
                        {
                            Schema.Migrate();

                            return await Task.FromResult(true);
                        }
                    };

                    SchemaChanged?.Invoke(this, args);
                }
            }

            foreach (var property in typeof(T).GetProperties())
            {
                Add(property);
            }
        }

        /// <summary>
        /// Performs an asynchronous rebuild of the index.
        /// </summary>
        /// <param name="capacity">The predicted capacity (number of items to store) of the index.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        protected virtual async Task ReBuildAsync(uint capacity)
        {
            if (DocumentStore == null || capacity > DocumentStore.Capacity)
            {
                switch (IndexType)
                {
                    case IndexType.Memory:
                        {
                            DocumentStore = new IndexMemoryDocumentStore<T>(Context, capacity);

                            break;
                        }
                    default:
                        {
                            var indexSchema = new IndexStorageSchema<T>(Context);
                            DocumentStore = new IndexStorageDocumentStore<T>(Context, capacity);

                            break;
                        }
                }

                if (Schema.HasSchemaChanged())
                {
                    var args = new IndexSchemaMigrationEventArgs
                    {
                        SchemaType = typeof(T),
                        PerformMigration = () =>
                        {
                            Schema.Migrate();

                            return true;
                        },
                        PerformMigrationAsync = async () =>
                        {
                            Schema.Migrate();

                            return await Task.FromResult(true);
                        }
                    };

                    SchemaChanged?.Invoke(this, args);
                }
            }

            var properties = typeof(T).GetProperties();
            var tasks = properties.Select(property => Task.Run(() => Add(property)));

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Adds a field name to the index.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexData interface.</typeparam>
        /// <param name="property">The property that makes up the index.</param>
        public virtual void Add(PropertyInfo property)
        {
            if (property.GetCustomAttribute<IndexIgnoreAttribute>() != null)
            {
                return;
            }

            switch (IndexType)
            {
                case IndexType.Memory:
                    {
                        if (!ContainsKey(property))
                        {
                            Add(property, new IndexMemoryReverse<T>(Context, property, Culture));
                        }

                        break;
                    }
                default:
                    {
                        if (!ContainsKey(property))
                        {
                            Add(property, new IndexStorageReverse<T>(Context, property, Culture));
                        }

                        break;
                    }
            }
        }

        /// <summary>
        /// Adds a item to the index.
        /// </summary>
        /// <param name="item">The data to be added to the index.</param>
        public virtual void Add(T item)
        {
            if (item == null)
            {
                return;
            }

            foreach (var property in typeof(T).GetProperties())
            {
                if (GetReverseIndex(property) is IIndexReverse<T> reverseIndex)
                {
                    reverseIndex.Add(item);
                }
            }

            DocumentStore.Add(item);
        }

        /// <summary>
        /// Performs an asynchronous addition of an item in the index.
        /// </summary>
        /// <param name="item">The data to be added to the index.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public virtual async Task AddAsync(T item)
        {
            if (item == null)
            {
                return;
            }

            var tasks = new List<Task>
            {
                Task.Run(() => DocumentStore.Add(item))
            };

            var properties = typeof(T).GetProperties();
            var reverseIndexes = properties
                .Select(GetReverseIndex)
                .Where(x => x != null);

            tasks.AddRange(reverseIndexes.Select(async reverseIndex =>
            {
                await Task.Run(() => reverseIndex.Add(item));
            }));

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Updates a item in the index.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
        /// <param name="item">The data to be updated to the index.</param>
        public virtual void Update(T item)
        {
            if (item == null)
            {
                return;
            }

            var currentItem = DocumentStore.GetItem(item.Id);

            foreach (var property in typeof(T).GetProperties())
            {
                var currentValue = property?.GetValue(currentItem)?.ToString();
                var currentTerms = Context.TokenAnalyzer.Analyze(currentValue, Culture);

                var changedValue = property?.GetValue(item)?.ToString();
                var changedTerms = Context.TokenAnalyzer.Analyze(changedValue, Culture);

                if (GetReverseIndex(property) is IIndexReverse<T> reverseIndex)
                {
                    var deleteTerms = currentTerms.Except(changedTerms);
                    var addTerms = changedTerms.Except(currentTerms);

                    reverseIndex.Remove(item, deleteTerms);
                    reverseIndex.Add(item, addTerms);
                }
            }

            DocumentStore.Update(item);
        }

        /// <summary>
        /// Performs an asynchronous update of an item in the index.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexItem interface.</typeparam>
        /// <param name="item">The data to be updated to the index.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public virtual async Task UpdateAsync(T item)
        {
            if (item == null)
            {
                return;
            }

            var currentItem = DocumentStore.GetItem(item.Id);

            var tasks = new List<Task>
            {
                Task.Run(() => DocumentStore.Add(item))
            };

            var properties = typeof(T).GetProperties();
            var reverseIndexes = properties
                .Select(property => new { Index = GetReverseIndex(property), Property = property })
                .Where(x => x.Index != null);

            tasks.AddRange(reverseIndexes.Select(async reverseIndex =>
            {
                var property = reverseIndex.Property;
                var index = reverseIndex.Index;

                await Task.Run(() =>
                {
                    var currentValue = property?.GetValue(currentItem)?.ToString();
                    var currentTerms = Context.TokenAnalyzer.Analyze(currentValue, Culture);

                    var changedValue = property?.GetValue(item)?.ToString();
                    var changedTerms = Context.TokenAnalyzer.Analyze(changedValue, Culture);

                    if (GetReverseIndex(property) is IIndexReverse<T> reverseIndex)
                    {
                        var deleteTerms = currentTerms.Except(changedTerms);
                        var addTerms = changedTerms.Except(currentTerms);

                        index.Remove(item, deleteTerms);
                        index.Add(item, addTerms);
                    }

                });
            }));

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// The data to be removed from the index.
        /// </summary>
        /// <param name="item">The data to be removed from the index.</param>
        public virtual void Remove(T item)
        {
            if (item == null)
            {
                return;
            }

            foreach (var property in typeof(T).GetProperties())
            {
                if (GetReverseIndex(property) is IIndexReverse<T> reverseIndex)
                {
                    reverseIndex.Remove(item);
                }
            }

            DocumentStore.Remove(item);
        }

        /// <summary>
        /// Removes an item from the index asynchronously.
        /// </summary>
        /// <param name="item">The data to be removed from the index.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public virtual async Task RemoveAsync(T item)
        {
            if (item == null)
            {
                return;
            }

            var tasks = new List<Task>
            {
                Task.Run(() => DocumentStore.Remove(item))
            };

            foreach (var property in typeof(T).GetProperties())
            {
                if (GetReverseIndex(property) is IIndexReverse<T> reverseIndex)
                {
                    tasks.Add(Task.Run(() => reverseIndex.Remove(item)));
                }
            }

            await Task.WhenAll(tasks);
        }
        /// <summary>
        /// Returns an index field based on its name.
        /// </summary>
        /// <param name="property">The property that makes up the index.</param>
        /// <returns>The index field or null.</returns>
        public virtual IIndexReverse<T> GetReverseIndex(PropertyInfo property)
        {
            if (TryGetValue(property, out var reverseIndex))
            {
                return reverseIndex;
            }

            return null;
        }

        /// <summary>
        /// Removed all data from the index.
        /// </summary>
        public virtual new void Clear()
        {
            foreach (var property in typeof(T).GetProperties())
            {
                if (GetReverseIndex(property) is IIndexReverse<T> reverseIndex)
                {
                    reverseIndex.Clear();
                }
            }

            DocumentStore.Clear();
        }

        /// <summary>
        /// Removed all data from the index asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public virtual async Task ClearAsync()
        {
            var tasks = new List<Task>
            {
                Task.Run(() => DocumentStore.Clear())
            };

            foreach (var property in typeof(T).GetProperties())
            {
                if (GetReverseIndex(property) is IIndexReverse<T> reverseIndex)
                {
                    tasks.Add(Task.Run(() => reverseIndex.Clear()));
                }
            }

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, 
        /// or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            DocumentStore.Dispose();

            foreach (var property in typeof(T).GetProperties())
            {
                if (GetReverseIndex(property) is IIndexReverse<T> reverseIndex)
                {
                    reverseIndex.Dispose();
                }
            }
        }
    }
}
