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
        /// Returns the forward index.
        /// </summary>
        public IIndexForward<T> ForwardIndex { get; private set; }

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

            switch (IndexType)
            {
                case IndexType.Memory:
                    {
                        ForwardIndex = new IndexMemoryForward<T>(Context, ushort.MaxValue);

                        break;
                    }
                default:
                    {
                        ForwardIndex = new IndexStorageForward<T>(Context, ushort.MaxValue);

                        break;
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
        public void Add(PropertyInfo property)
        {
            if (property.GetCustomAttribute<IndexIgnore>() != null)
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
        public void Add(T item)
        {
            foreach (var property in typeof(T).GetProperties().Where(x => x.GetCustomAttribute<IndexIgnore>() == null))
            {
                if (GetReverseIndex(property) is IIndexReverse<T> reverseIndex)
                {
                    reverseIndex.Add(item);
                }
            }

            ForwardIndex.Add(item);
        }

        /// <summary>
        /// Removed all data from the index.
        /// </summary>
        public new void Clear()
        {
            foreach (var property in typeof(T).GetProperties().Where(x => x.GetCustomAttribute<IndexIgnore>() == null))
            {
                if (GetReverseIndex(property) is IIndexReverse<T> reverseIndex)
                {
                    reverseIndex.Clear();
                }
            }

            ForwardIndex.Clear();
        }

        /// <summary>
        /// The data to be removed from the index.
        /// </summary>
        /// <param name="item">The data to be removed from the index.</param>
        public void Remove(T item)
        {
            foreach (var property in typeof(T).GetProperties())
            {
                if (GetReverseIndex(property) is IIndexReverse<T> reverseIndex)
                {
                    reverseIndex.Remove(item);
                }
            }

            ForwardIndex.Remove(item);
        }

        /// <summary>
        /// Returns an index field based on its name.
        /// </summary>
        /// <param name="property">The property that makes up the index.</param>
        /// <returns>The index field or null.</returns>
        public IIndexReverse<T> GetReverseIndex(PropertyInfo property)
        {
            return ContainsKey(property) ? this[property] : null;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, 
        /// or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            ForwardIndex.Dispose();

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
