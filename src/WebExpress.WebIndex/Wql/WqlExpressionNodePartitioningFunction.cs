﻿using System.Linq;

namespace WebExpress.WebIndex.Wql
{
    /// <summary>
    /// Describes the partitioning expression of a wql statement.
    /// </summary>
    /// <typeparam name="TIndexItem">The type of the index item.</typeparam>
    public class WqlExpressionNodePartitioningFunction<TIndexItem> : IWqlExpressionNode<TIndexItem>
        where TIndexItem : IIndexItem
    {
        /// <summary>
        /// Returns the operator expressions.
        /// </summary>
        public WqlExpressionNodePartitioningOperator Operator { get; internal set; }

        /// <summary>
        /// Returns the value expressions.
        /// </summary>
        public int Value { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        internal WqlExpressionNodePartitioningFunction()
        {
        }

        /// <summary>
        /// Applies the filter to the unfiltered data object.
        /// </summary>
        /// <param name="unfiltered">The unfiltered data.</param>
        /// <returns>The filtered data.</returns>
        public IQueryable<Q> Apply<Q>(IQueryable<Q> unfiltered)
        {
            var filtered = unfiltered;

            if (Operator == WqlExpressionNodePartitioningOperator.Skip)
            {
                filtered = filtered.Skip(Value);
            }

            if (Operator == WqlExpressionNodePartitioningOperator.Take)
            {
                filtered = filtered.Take(Value);
            }

            return filtered.AsQueryable();
        }

        /// <summary>
        /// Converts the partitioning expression to a string.
        /// </summary>
        /// <returns>The partitioning expression as a string.</returns>
        public override string ToString()
        {
            return string.Format("{0} {1}", Operator.ToString().ToLower(), Value).Trim();
        }
    }
}