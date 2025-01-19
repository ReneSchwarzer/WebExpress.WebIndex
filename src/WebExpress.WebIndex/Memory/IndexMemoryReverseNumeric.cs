using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using WebExpress.WebIndex.Term;

namespace WebExpress.WebIndex.Memory
{
    /// <summary>
    /// Provides a reverse index for numeric values that manages the data in the main memory.
    /// Key: The terms.
    /// Value: The index item.
    /// </summary>
    /// <param name="context">The index context.</param>
    /// <param name="property">The property that makes up the index.</param>
    /// <param name="culture">The culture.</param>
    public class IndexMemoryReverseNumeric<TIndexItem>(IIndexDocumemntContext context, PropertyInfo property, CultureInfo culture) : IIndexReverse<TIndexItem>
        where TIndexItem : IIndexItem
    {
        /// <summary>
        /// Returns the field name for the reverse index.
        /// </summary>
        public string Field => Property?.Name;

        /// <summary>
        /// A delegate that determines the value of the current property.
        /// </summary>
        private Func<TIndexItem, object> GetValueDelegate { get; set; } = CreateDelegate(property);

        /// <summary>
        /// The property that makes up the index.
        /// </summary>
        private PropertyInfo Property { get; set; } = property;

        /// <summary>
        /// Returns the index context.
        /// </summary>
        public IIndexDocumemntContext Context { get; private set; } = context;

        /// <summary>
        /// Returns the culture.
        /// </summary>
        public CultureInfo Culture { get; private set; } = culture;

        /// <summary>
        /// The root term.
        /// </summary>
        public IndexMemorySegmentNumericNode Numeric { get; private set; } = new();

        /// <summary>
        /// Returns all items.
        /// </summary>
        public IEnumerable<Guid> All => Numeric.All.Distinct();

        /// <summary>
        /// Adds a item to the index.
        /// </summary>
        /// <param name="item">The data to be added to the index.</param>
        public void Add(TIndexItem item)
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
        public void Add(TIndexItem item, IEnumerable<IndexTermToken> terms)
        {
            foreach (var term in terms)
            {
                Numeric.Add(item.Id, Convert.ToDecimal(term.Value));
            }
        }

        /// <summary>
        /// The data to be removed from the index.
        /// </summary>
        /// <param name="item">The data to be removed from the field.</param>
        public void Delete(TIndexItem item)
        {
            var value = GetValueDelegate(item);
            var terms = Context.TokenAnalyzer.Analyze(value?.ToString(), Culture);

            Delete(item, terms);
        }

        /// <summary>
        /// The data to be removed from the index.
        /// </summary>
        /// <param name="item">The data to be removed from the field.</param>
        /// <param name="terms">The terms to add to the reverse index for the given item.</param>
        public void Delete(TIndexItem item, IEnumerable<IndexTermToken> terms)
        {
            foreach (var term in terms)
            {
                //Numeric.Remove(term.Value.ToString(), item.Id);
            }
        }

        /// <summary>
        /// Removed all data from the index.
        /// </summary>
        public void Clear()
        {
            Numeric = new IndexMemorySegmentNumericNode();
        }

        /// <summary>
        /// Drop the reverse index.
        /// </summary>
        public void Drop()
        {

        }

        /// <summary>
        /// Return all items for a given input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="options">The retrieve options.</param>
        /// <returns>An enumeration of the data ids.</returns>
        public IEnumerable<Guid> Retrieve(object input, IndexRetrieveOptions options)
        {
            if (decimal.TryParse(input?.ToString(), out decimal value))
            {
                return options.Method switch
                {
                    IndexRetrieveMethod.Phrase => Numeric.Retrieve(value, options),
                    IndexRetrieveMethod.GratherThan => Numeric.Retrieve(value, options),
                    IndexRetrieveMethod.GratherThanOrEqual => Numeric.Retrieve(value, options),
                    IndexRetrieveMethod.LessThan => Numeric.Retrieve(value, options),
                    IndexRetrieveMethod.LessThanOrEqual => Numeric.Retrieve(value, options),
                    _ => []
                };
            }

            return [];
        }

        /// <summary>
        /// Creates a delegate for faster access to the value of the property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>A delegate that determines the value of the current property.</returns>
        private static Func<TIndexItem, object> CreateDelegate(PropertyInfo property)
        {
            var helper = typeof(IndexMemoryReverseTerm<TIndexItem>).GetMethod("CreateDelegateInternal", BindingFlags.Static | BindingFlags.NonPublic);
            var method = helper.MakeGenericMethod(typeof(TIndexItem), property.PropertyType);

            return (Func<TIndexItem, object>)method.Invoke(null, [property.GetGetMethod()]);
        }

        /// <summary>
        /// An auxiliary function used to determine the value of a property.
        /// </summary>
        /// <param name="methodInfo">The method Info.</param>
        /// <returns>A delegate that determines the value of the current property.</returns>
        [SuppressMessage("CodeQuality", "IDE0051")]
        private static Func<X, object> CreateDelegateInternal<X, TReturn>(MethodInfo methodInfo)
        {
            var f = (Func<X, TReturn>)System.Delegate.CreateDelegate(typeof(Func<X, TReturn>), methodInfo);
            return t => (object)f(t);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, 
        /// or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
        }
    }
}
