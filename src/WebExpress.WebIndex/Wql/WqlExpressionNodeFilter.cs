using System;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebIndex.Wql.Condition;

namespace WebExpress.WebIndex.Wql
{
    /// <summary>
    /// Describes the filter expression of a WQL statement.
    /// </summary>
    /// <typeparam name="TIndexItem">The type of the index item.</typeparam>
    public class WqlExpressionNodeFilter<TIndexItem> : IWqlExpressionNodeApply<TIndexItem>
        where TIndexItem : IIndexItem
    {
        /// <summary>
        /// Returns the condition expression.
        /// </summary>
        public WqlExpressionNodeFilterCondition<TIndexItem> Condition { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        internal WqlExpressionNodeFilter()
        {
        }

        /// <summary>
        /// Applies the filter to the index.
        /// </summary>
        /// <returns>The data from the index.</returns>
        public virtual IEnumerable<Guid> Apply()
        {
            return Condition?.Apply() ?? [];
        }

        /// <summary>
        /// Applies the filter to the unfiltered data object.
        /// </summary>
        /// <param name="unfiltered">The unfiltered data.</param>
        /// <returns>The filtered data.</returns>
        public virtual IQueryable<TIndexItem> Apply(IQueryable<TIndexItem> unfiltered)
        {
            return Condition?.Apply(unfiltered) ?? unfiltered;
        }

        /// <summary>
        /// Returns the sql query string.
        /// </summary>
        /// <returns>The sql part of the node.</returns>
        public virtual string GetSqlQueryString()
        {
            var sql = Condition?.GetSqlQueryString() ?? "";

            return sql;
        }

        /// <summary>
        /// Converts the filter expression to a string.
        /// </summary>
        /// <returns>The filter expression as a string.</returns>
        public override string ToString()
        {
            return string.Format("{0}", Condition).Trim();
        }
    }
}