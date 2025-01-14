using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace WebExpress.WebIndex.Wql.Condition
{
    /// <summary>
    /// Interface of a condition expression.
    /// </summary>
    public abstract class WqlExpressionNodeFilterCondition<TIndexItem> : IWqlExpressionNodeFilterCondition<TIndexItem> where TIndexItem : IIndexItem
    {
        /// <summary>
        /// Returns the attribute expression.
        /// </summary>
        public WqlExpressionNodeAttribute<TIndexItem> Attribute { get; internal set; }

        /// <summary>
        /// Returns the operator expression.
        /// </summary>
        public string Operator { get; internal set; }

        /// <summary>
        /// Returns the culture in which to run the wql.
        /// </summary>
        public CultureInfo Culture { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="token">One or more tokens that determine the operation. Multiple tokens are separated by spaces.</param>
        protected WqlExpressionNodeFilterCondition(string token)
        {
            Operator = token;
        }

        /// <summary>
        /// Applies the filter to the index.
        /// </summary>
        /// <returns>The data ids from the index.</returns>
        public abstract IEnumerable<Guid> Apply();

        /// <summary>
        /// Applies the filter to the unfiltered data object.
        /// </summary>
        /// <param name="unfiltered">The unfiltered data.</param>
        /// <returns>The filtered data.</returns>
        public abstract IQueryable<TIndexItem> Apply(IQueryable<TIndexItem> unfiltered);

        /// <summary>
        /// Returns the sql query string.
        /// </summary>
        /// <returns>The sql part of the node.</returns>
        public abstract string GetSqlQueryString();
    }
}