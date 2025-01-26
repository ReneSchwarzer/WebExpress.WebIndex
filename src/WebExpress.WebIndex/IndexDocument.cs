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
    public class IndexDocument<TIndexItem> : IIndexDocument<TIndexItem>
        where TIndexItem : IIndexItem
    {
        private readonly Dictionary<PropertyInfo, IIndexReverse<TIndexItem>> _dict = [];
        private readonly List<IndexFieldData> _fields = [];

        /// <summary>
        /// Event that is triggered when the schema has changed.
        /// </summary>
        public event EventHandler<IndexSchemaMigrationEventArgs> SchemaChanged;

        /// <summary>
        /// Returns the document store.
        /// </summary>
        public IIndexDocumentStore<TIndexItem> DocumentStore { get; private set; }

        /// <summary>
        /// Returns the index schema associated with this index document.
        /// </summary>
        public IIndexSchema<TIndexItem> Schema { get; private set; }

        /// <summary>
        /// Returns the index type.
        /// </summary>
        public IndexType IndexType { get; private set; }

        /// <summary>
        /// Return the index field names.
        /// </summary>
        public IEnumerable<IndexFieldData> Fields => _fields;

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
        public IEnumerable<TIndexItem> All => DocumentStore.All;

        /// <summary>
        /// Initializes a new instance of the class.
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
                            Schema = new IndexMemorySchema<TIndexItem>(Context);
                            DocumentStore = new IndexMemoryDocumentStore<TIndexItem>(Context, capacity);

                            break;
                        }
                    default:
                        {
                            Schema = new IndexStorageSchema<TIndexItem>(Context);
                            DocumentStore = new IndexStorageDocumentStore<TIndexItem>(Context, capacity);

                            break;
                        }
                }

                if (Schema.HasSchemaChanged())
                {
                    var args = new IndexSchemaMigrationEventArgs
                    {
                        SchemaType = typeof(TIndexItem),
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

            _fields.Clear();
            _fields.AddRange(GetFieldData(typeof(TIndexItem)));
            _dict.Clear();

            foreach (var field in _fields)
            {
                Add(field);
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
                            DocumentStore = new IndexMemoryDocumentStore<TIndexItem>(Context, capacity);

                            break;
                        }
                    default:
                        {
                            var indexSchema = new IndexStorageSchema<TIndexItem>(Context);
                            DocumentStore = new IndexStorageDocumentStore<TIndexItem>(Context, capacity);

                            break;
                        }
                }

                if (Schema.HasSchemaChanged())
                {
                    var args = new IndexSchemaMigrationEventArgs
                    {
                        SchemaType = typeof(TIndexItem),
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

            var properties = GetFieldData(typeof(TIndexItem));
            var tasks = properties.Select(property => Task.Run(() => Add(property)));

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Adds a field name to the index.
        /// </summary>
        /// <typeparam name="T">The data type. This must have the IIndexData interface.</typeparam>
        /// <param name="property">The property that makes up the index.</param>
        public virtual void Add(IndexFieldData property)
        {
            if (property.PropertyInfo.GetCustomAttribute<IndexIgnoreAttribute>() != null || _dict.ContainsKey(property.PropertyInfo))
            {
                return;
            }

            switch (IndexType)
            {
                case IndexType.Memory:
                    {
                        if (IsNumericType(property.PropertyInfo))
                        {
                            _dict.Add
                            (
                                property.PropertyInfo,
                                new IndexMemoryReverseNumeric<TIndexItem>(Context, property, Culture)
                            );
                        }
                        else
                        {
                            _dict.Add
                            (
                                property.PropertyInfo,
                                new IndexMemoryReverseTerm<TIndexItem>(Context, property, Culture)
                            );
                        }
                        break;
                    }
                default:
                    {
                        if (IsNumericType(property.PropertyInfo))
                        {
                            _dict.Add
                            (
                                property.PropertyInfo,
                                new IndexStorageReverseNumeric<TIndexItem>(Context, property, Culture)
                            );
                        }
                        else
                        {
                            _dict.Add
                            (
                                property.PropertyInfo,
                                new IndexStorageReverseTerm<TIndexItem>(Context, property, Culture)
                            );
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// Adds a item to the index.
        /// </summary>
        /// <param name="item">The data to be added to the index.</param>
        public virtual void Add(TIndexItem item)
        {
            if (item == null)
            {
                return;
            }

            foreach (var field in Fields)
            {
                if (GetReverseIndex(field) is IIndexReverse<TIndexItem> reverseIndex)
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
        public virtual async Task AddAsync(TIndexItem item)
        {
            if (item == null)
            {
                return;
            }

            var tasks = new List<Task>
            {
                Task.Run(() => DocumentStore.Add(item))
            };

            var reverseIndexes = Fields
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
        public virtual void Update(TIndexItem item)
        {
            if (item == null)
            {
                return;
            }

            var currentItem = DocumentStore.GetItem(item.Id);

            foreach (var field in Fields)
            {
                var currentValue = field.GetPropertyValue(currentItem)?.ToString();
                var currentTerms = Context.TokenAnalyzer.Analyze(currentValue, Culture);

                var changedValue = field.GetPropertyValue(item)?.ToString();
                var changedTerms = Context.TokenAnalyzer.Analyze(changedValue, Culture);

                if (GetReverseIndex(field) is IIndexReverse<TIndexItem> reverseIndex)
                {
                    var deleteTerms = currentTerms.Except(changedTerms);
                    var addTerms = changedTerms.Except(currentTerms);

                    reverseIndex.Delete(item, deleteTerms);
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
        public virtual async Task UpdateAsync(TIndexItem item)
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

            var reverseIndexes = Fields
                .Select(property => new { Index = GetReverseIndex(property), Field = property })
                .Where(x => x.Index != null);

            tasks.AddRange(reverseIndexes.Select(async reverseIndex =>
            {
                var field = reverseIndex.Field;
                var index = reverseIndex.Index;

                await Task.Run(() =>
                {
                    var currentValue = field.GetPropertyValue(currentItem)?.ToString();
                    var currentTerms = Context.TokenAnalyzer.Analyze(currentValue, Culture);

                    var changedValue = field.GetPropertyValue(item)?.ToString();
                    var changedTerms = Context.TokenAnalyzer.Analyze(changedValue, Culture);

                    if (GetReverseIndex(field) is IIndexReverse<TIndexItem> reverseIndex)
                    {
                        var deleteTerms = currentTerms.Except(changedTerms);
                        var addTerms = changedTerms.Except(currentTerms);

                        index.Delete(item, deleteTerms);
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
        public virtual void Remove(TIndexItem item)
        {
            if (item == null)
            {
                return;
            }

            foreach (var field in Fields)
            {
                if (GetReverseIndex(field) is IIndexReverse<TIndexItem> reverseIndex)
                {
                    reverseIndex.Delete(item);
                }
            }

            DocumentStore.Delete(item);
        }

        /// <summary>
        /// Removes an item from the index asynchronously.
        /// </summary>
        /// <param name="item">The data to be removed from the index.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public virtual async Task RemoveAsync(TIndexItem item)
        {
            if (item == null)
            {
                return;
            }

            var tasks = new List<Task>
            {
                Task.Run(() => DocumentStore.Delete(item))
            };

            foreach (var field in Fields)
            {
                if (GetReverseIndex(field) is IIndexReverse<TIndexItem> reverseIndex)
                {
                    tasks.Add(Task.Run(() => reverseIndex.Delete(item)));
                }
            }

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Returns the number of items.
        /// </summary>
        /// <returns>The number of items.</returns>
        public new uint Count()
        {
            return DocumentStore.Count();
        }

        /// <summary>
        /// Performs an asynchronous determination of the number of elements.
        /// </summary>
        /// <returns>A task representing the asynchronous operation with the number of items.</returns>
        public async Task<uint> CountAsync()
        {
            return await Task.Run(() => DocumentStore.Count());
        }

        /// <summary>
        /// Returns an index field based on its name.
        /// </summary>
        /// <param name="field">The field that makes up the index.</param>
        /// <returns>The index field or null.</returns>
        public virtual IIndexReverse<TIndexItem> GetReverseIndex(IndexFieldData field)
        {
            if (_dict.TryGetValue(field.PropertyInfo, out var reverseIndex))
            {
                return reverseIndex;
            }

            return null;
        }

        /// <summary>
        /// Drop all index documents of type T.
        /// </summary>
        public void Drop()
        {
            foreach (var field in Fields)
            {
                if (GetReverseIndex(field) is IIndexReverse<TIndexItem> reverseIndex)
                {
                    reverseIndex.Drop();
                }
            }

            DocumentStore.Drop();
            Schema.Drop();
        }

        /// <summary>
        /// Asynchronously drops all index documents of type T.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task DropAsync()
        {
            var tasks = new List<Task>
            {
                Task.Run(() => DocumentStore.Drop()),
                Task.Run(() => Schema.Drop())
            };

            foreach (var field in Fields)
            {
                if (GetReverseIndex(field) is IIndexReverse<TIndexItem> reverseIndex)
                {
                    tasks.Add(Task.Run(() => reverseIndex.Drop()));
                }
            }

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Removed all data from the index.
        /// </summary>
        public virtual new void Clear()
        {
            foreach (var fielld in Fields)
            {
                if (GetReverseIndex(fielld) is IIndexReverse<TIndexItem> reverseIndex)
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

            foreach (var field in Fields)
            {
                if (GetReverseIndex(field) is IIndexReverse<TIndexItem> reverseIndex)
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

            foreach (var field in Fields)
            {
                if (GetReverseIndex(field) is IIndexReverse<TIndexItem> reverseIndex)
                {
                    reverseIndex.Dispose();
                }
            }

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Determines if the given property is of a numeric type.
        /// </summary>
        /// <param name="property">The property to check.</param>
        /// <returns>True if the property is of a numeric type, otherwise false.</returns>
        private static bool IsNumericType(PropertyInfo property)
        {
            var type = property.PropertyType;

            return type == typeof(byte) || type == typeof(sbyte) ||
                   type == typeof(short) || type == typeof(ushort) ||
                   type == typeof(int) || type == typeof(uint) ||
                   type == typeof(long) || type == typeof(ulong) ||
                   type == typeof(float) || type == typeof(double) ||
                   type == typeof(decimal);
        }

        /// <summary>
        /// Recursively retrieves the field names of the specified type.
        /// </summary>
        /// <param name="type">The type whose field names to retrieve.</param>
        /// <param name="prefix">The prefix to prepend to each field name.</param>
        /// <param name="processedTypes">A set of types that have already been processed to avoid circular references.</param>
        /// <returns>An enumerable collection of field names.</returns>
        private static IEnumerable<IndexFieldData> GetFieldData(Type type, string prefix = "", HashSet<Type> processedTypes = null)
        {
            processedTypes ??= [];

            if (processedTypes.Contains(type))
            {
                yield break;
            }

            processedTypes.Add(type);

            foreach (var property in type.GetProperties())
            {
                string propertyName = string.IsNullOrEmpty(prefix) ? property.Name : $"{prefix}.{property.Name}";

                yield return new IndexFieldData
                {
                    Name = propertyName,
                    Type = property.PropertyType,
                    PropertyInfo = property
                };

                if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                {
                    foreach (var subProperty in GetFieldData(property.PropertyType, propertyName, processedTypes))
                    {
                        yield return subProperty;
                    }
                }
            }
        }
    }
}
