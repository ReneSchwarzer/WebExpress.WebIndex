using System.Reflection;

namespace WebExpress.WebIndex.Wql
{
    /// <summary>
    /// Describes the attribute expression node of a WQL statement.
    /// </summary>
    /// <typeparam name="TIndexItem">The type of the index item.</typeparam>
    public class WqlExpressionNodeAttribute<TIndexItem> : IWqlExpressionNode<TIndexItem>
        where TIndexItem : IIndexItem
    {
        /// <summary>
        /// Returns the name of the attribute.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Returns the property info of the attribute.
        /// </summary>
        public PropertyInfo Property { get; internal set; }

        /// <summary>
        /// Returns the reverse index.
        /// </summary>
        public IIndexReverse<TIndexItem> ReverseIndex { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        internal WqlExpressionNodeAttribute()
        {
        }

        /// <summary>
        /// Converts the attribute expression to a string.
        /// </summary>
        /// <returns>The attribute expression as a string.</returns>
        public override string ToString()
        {
            return string.Format("{0}", Name).Trim();
        }
    }
}