using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
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
        /// Returns the document store.
        /// </summary>
        public IIndexDocumentStore<T> DocumentStore { get; private set; }

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
        public IIndexContext Context { get; private set; }

        /// <summary>
        /// Returns the culture.
        /// </summary>
        public CultureInfo Culture { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">The index context.</param>
        /// <param name="indexType">The index type.</param>
        /// <param name="culture">The culture.</param>
        public IndexDocument(IIndexContext context, IndexType indexType, CultureInfo culture)
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
        public virtual void ReBuild(uint capacity)
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
                            DocumentStore = new IndexStorageDocumentStore<T>(Context, capacity);

                            break;
                        }
                }
            }

            foreach (var property in typeof(T).GetProperties())
            {
                Add(property);
            }
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
            foreach (var property in typeof(T).GetProperties().Where(x => x.GetCustomAttribute<IndexIgnoreAttribute>() == null))
            {
                if (GetReverseIndex(property) is IIndexReverse<T> reverseIndex)
                {
                    reverseIndex.Add(item);
                }
            }

            DocumentStore.Add(item);
        }

        /// <summary>
        /// Removed all data from the index.
        /// </summary>
        public virtual new void Clear()
        {
            foreach (var property in typeof(T).GetProperties().Where(x => x.GetCustomAttribute<IndexIgnoreAttribute>() == null))
            {
                if (GetReverseIndex(property) is IIndexReverse<T> reverseIndex)
                {
                    reverseIndex.Clear();
                }
            }

            DocumentStore.Clear();
        }

        /// <summary>
        /// The data to be removed from the index.
        /// </summary>
        /// <param name="item">The data to be removed from the index.</param>
        public virtual void Remove(T item)
        {
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
        /// Returns an index field based on its name.
        /// </summary>
        /// <param name="property">The property that makes up the index.</param>
        /// <returns>The index field or null.</returns>
        public virtual IIndexReverse<T> GetReverseIndex(PropertyInfo property)
        {
            return ContainsKey(property) ? this[property] : null;
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
