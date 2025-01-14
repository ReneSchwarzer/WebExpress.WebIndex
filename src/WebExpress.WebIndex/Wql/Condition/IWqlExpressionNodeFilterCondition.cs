using System.Globalization;

namespace WebExpress.WebIndex.Wql.Condition
{
    /// <summary>
    /// Represents a filter condition for a WQL expression node.
    /// </summary>
    /// <typeparam name="TIndexItem">The type of the index item.</typeparam>
    public interface IWqlExpressionNodeFilterCondition<TIndexItem> : IWqlExpressionNodeApply<TIndexItem> where TIndexItem : IIndexItem
    {
        /// <summary>
        /// Returns the attribute expression.
        /// </summary>
        WqlExpressionNodeAttribute<TIndexItem> Attribute { get; }

        /// <summary>
        /// Returns the operator expression.
        /// </summary>
        string Operator { get; }

        /// <summary>
        /// Returns the culture in which to run the wql.
        /// </summary>
        CultureInfo Culture { get; }
    }
}
