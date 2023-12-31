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
    public class WebIndexDocument<T> : Dictionary<PropertyInfo, IWebIndexReverse<T>>, IIndexDocument<T> where T : IWebIndexItem
    {
        /// <summary>
        /// Returns the forward index.
        /// </summary>
        public IWebIndexForward<T> ForwardIndex { get; private set; }

        /// <summary>
        /// Returns the index type.
        /// </summary>
        public WebIndexType IndexType { get; private set; }

        /// <summary>
        /// Return the index field names.
        /// </summary>
        public IEnumerable<string> Fields => Keys.Select(x => x.Name);

        /// <summary>
        /// Returns the index context.
        /// </summary>
        public IWebIndexContext Context { get; private set; }

        /// <summary>
        /// Returns the predicted capacity (number of items to store) of the index.
        /// </summary>
        public uint Capacity { get; private set; }

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
        /// <param name="capacity">The predicted capacity (number of items to store) of the index.</param>
        public WebIndexDocument(IWebIndexContext context, WebIndexType indexType, CultureInfo culture, uint capacity)
        {
            Context = context;
            IndexType = indexType;
            Culture = culture;
            Capacity = capacity;

            switch (IndexType)
            {
                case WebIndexType.Memory:
                    {
                        ForwardIndex = new WebIndexMemoryForward<T>(Context, Capacity);

                        break;
                    }
                default:
                    {
                        ForwardIndex = new WebIndexStorageForward<T>(Context, Capacity);

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
            if (property.GetCustomAttribute<WebIndexIgnore>() != null)
            {
                return;
            }

            switch (IndexType)
            {
                case WebIndexType.Memory:
                    {
                        if (!ContainsKey(property))
                        {
                            Add(property, new WebIndexMemoryReverse<T>(Context, property, Culture, Capacity));
                        }

                        break;
                    }
                default:
                    {
                        if (!ContainsKey(property))
                        {
                            Add(property, new WebIndexStorageReverse<T>(Context, property, Culture, Capacity));
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
            foreach (var property in typeof(T).GetProperties().Where(x => x.GetCustomAttribute<WebIndexIgnore>() == null))
            {
                if (GetReverseIndex(property) is IWebIndexReverse<T> reverseIndex)
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
            foreach (var property in typeof(T).GetProperties().Where(x => x.GetCustomAttribute<WebIndexIgnore>() == null))
            {
                if (GetReverseIndex(property) is IWebIndexReverse<T> reverseIndex)
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
                if (GetReverseIndex(property) is IWebIndexReverse<T> reverseIndex)
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
        public IWebIndexReverse<T> GetReverseIndex(PropertyInfo property)
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
                if (GetReverseIndex(property) is IWebIndexReverse<T> reverseIndex)
                {
                    reverseIndex.Dispose();
                }
            }
        }
    }
}
