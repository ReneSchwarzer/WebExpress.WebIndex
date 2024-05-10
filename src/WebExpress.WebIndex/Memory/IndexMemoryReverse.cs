using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using WebExpress.WebIndex.Term;

namespace WebExpress.WebIndex.Memory
{
    /// <summary>
    /// Provides a reverse index that manages the data in the main memory.
    /// Key: The terms.
    /// Value: The index item.
    /// </summary>
    public class IndexMemoryReverse<T> : IIndexReverse<T> where T : IIndexItem
    {
        /// <summary>
        /// Returns the field name for the reverse index.
        /// </summary>
        public string Field => Property?.Name;

        /// <summary>
        /// A delegate that determines the value of the current property.
        /// </summary>
        private Func<T, object> GetValueDelegate { get; set; }

        /// <summary>
        /// The property that makes up the index.
        /// </summary>
        private PropertyInfo Property { get; set; }

        /// <summary>
        /// Returns the index context.
        /// </summary>
        public IIndexDocumemntContext Context { get; private set; }

        /// <summary>
        /// Returns the culture.
        /// </summary>
        public CultureInfo Culture { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public IndexMemoryReverseTerm<T> Root { get; private set; } = new ();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">The index context.</param>
        /// <param name="property">The property that makes up the index.</param>
        /// <param name="culture">The culture.</param>
        public IndexMemoryReverse(IIndexDocumemntContext context, PropertyInfo property, CultureInfo culture)
        {
            Context = context;
            Property = property;
            Culture = culture;
            GetValueDelegate = CreateDelegate(property);
        }

        /// <summary>
        /// Adds a item to the index.
        /// </summary>
        /// <param name="item">The data to be added to the index.</param>
        public void Add(T item)
        {
            var value = GetValueDelegate(item);
            var terms = Context.TokenAnalyzer.Analyze(value?.ToString(), Culture);

            Add(item, terms);
        }

        /// <summary>
        /// Adds a item to the index.
        /// </summary>
        /// <param name="item">The data to be added to the index.</param>
        /// <param name="terms">The terms to add to the reverse index for the given item.</param>
        public void Add(T item, IEnumerable<IndexTermToken> terms)
        {
            foreach (var term in terms)
            {
                Root.Add(item, term.Value, term.Position);
            }
        }

        /// <summary>
        /// The data to be removed from the index.
        /// </summary>
        /// <param name="item">The data to be removed from the field.</param>
        public void Remove(T item)
        {
            var value = GetValueDelegate(item);
            var terms = Context.TokenAnalyzer.Analyze(value?.ToString(), Culture);

            Remove(item, terms);
        }

        /// <summary>
        /// The data to be removed from the index.
        /// </summary>
        /// <param name="item">The data to be removed from the field.</param>
        /// <param name="terms">The terms to add to the reverse index for the given item.</param>
        public void Remove(T item, IEnumerable<IndexTermToken> terms)
        {
            foreach (var term in terms)
            {
                Root.Remove(term.Value, item);
            }
        }

        /// <summary>
        /// Removed all data from the index.
        /// </summary>
        public void Clear()
        {
            Root = new IndexMemoryReverseTerm<T>();
        }

        /// <summary>
        /// Return all items for a given term.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <returns>An enumeration of the data ids.</returns>
        public IEnumerable<Guid> Collect(object term)
        {
            var terms = Context.TokenAnalyzer.Analyze(term?.ToString(), Culture);

            return terms.SelectMany(x => Root.Collect(x.Value)).Distinct();
        }

        /// <summary>
        /// Creates a delegate for faster access to the value of the property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>A delegate that determines the value of the current property.</returns>
        private static Func<T, object> CreateDelegate(PropertyInfo property)
        {
            var helper = typeof(IndexMemoryReverse<T>).GetMethod("CreateDelegateInternal", BindingFlags.Static | BindingFlags.NonPublic);
            var method = helper.MakeGenericMethod(typeof(T), property.PropertyType);

            return (Func<T, object>)method.Invoke(null, new object[] { property.GetGetMethod() });
        }

        /// <summary>
        /// An auxiliary function used to determine the value of a property.
        /// </summary>
        /// <param name="methodInfo">The method Info.</param>
        /// <returns>A delegate that determines the value of the current property.</returns>
        private static Func<X, object> CreateDelegateInternal<X, TReturn>(MethodInfo methodInfo)
        {
            var f = (Func<X, TReturn>)System.Delegate.CreateDelegate(typeof(Func<X, TReturn>), methodInfo);
            return t => (object)f(t);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, 
        /// or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }
    }
}
