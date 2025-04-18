﻿using System;
using System.Linq;

namespace WebExpress.WebIndex.Wql.Condition
{
    /// <summary>
    /// Represents a binary equal condition in a WQL expression node.
    /// </summary>
    /// <typeparam name="TIndexItem">The type of the index item.</typeparam>
    public class WqlExpressionNodeFilterConditionBinaryEqual<TIndexItem> : WqlExpressionNodeFilterConditionBinary<TIndexItem>
        where TIndexItem : IIndexItem
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="op">The operator.</param>
        public WqlExpressionNodeFilterConditionBinaryEqual()
            : base("=")
        {
        }

        /// <summary>
        /// Applies the filter to the index.
        /// </summary>
        /// <returns>The data ids from the index.</returns>
        public override IQueryable<Guid> Apply()
        {
            var value = Parameter.GetValue()?.ToString();

            return Attribute.ReverseIndex?.Retrieve(value, new IndexRetrieveOptions()
            {
                Method = IndexRetrieveMethod.Phrase,
                Distance = Options.Distance.HasValue ? Options.Distance.Value : 0
            }).AsQueryable();
        }

        /// <summary>
        /// Applies the filter to the unfiltered data object.
        /// </summary>
        /// <param name="unfiltered">The unfiltered data.</param>
        /// <returns>The filtered data.</returns>
        public override IQueryable<TIndexItem> Apply(IQueryable<TIndexItem> unfiltered)
        {
            var property = Attribute?.Property;
            var value = Parameter.GetValue();

            var filtered = unfiltered.Where
            (
                x => property != null && property.GetValue(x).Equals(value)
            );

            return filtered.AsQueryable();
        }

        /// <summary>
        /// Returns the sql query string.
        /// </summary>
        /// <returns>The sql part of the node.</returns>
        public override string GetSqlQueryString()
        {
            var property = Attribute?.Name;
            var value = Parameter.Value;

            return $"{property} like {value}";
        }
    }
}
