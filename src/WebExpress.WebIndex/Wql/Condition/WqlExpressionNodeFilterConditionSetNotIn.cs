using System;
using System.Linq;

namespace WebExpress.WebIndex.Wql.Condition
{
    /// <summary>
    /// Represents a WQL expression node filter condition for the "not in" set operation.
    /// </summary>
    /// <typeparam name="TIndexItem">The type of the index item.</typeparam>
    public class WqlExpressionNodeFilterConditionSetNotIn<TIndexItem> : WqlExpressionNodeFilterConditionSet<TIndexItem>
        where TIndexItem : IIndexItem
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="op">The operator.</param>
        public WqlExpressionNodeFilterConditionSetNotIn()
            : base("not in")
        {
        }

        /// <summary>
        /// Applies the filter to the index.
        /// </summary>
        /// <returns>The data ids from the index.</returns>
        public override IQueryable<Guid> Apply()
        {
            return null;
        }

        /// <summary>
        /// Applies the filter to the unfiltered data object.
        /// </summary>
        /// <param name="unfiltered">The unfiltered data.</param>
        /// <returns>The filtered data.</returns>
        public override IQueryable<TIndexItem> Apply(IQueryable<TIndexItem> unfiltered)
        {
            var property = Attribute.Property;
            var values = Parameters.Select(y => y.GetValue());
            var filtered = unfiltered.Where
            (
                x => !values.Contains(property.GetValue(x))
            );

            return filtered;
        }

        /// <summary>
        /// Returns the sql query string.
        /// </summary>
        /// <returns>The sql part of the node.</returns>
        public override string GetSqlQueryString()
        {
            var property = Attribute?.Property;
            var values = Parameters;

            return $"{property.Name} not in {string.Join(", ", values.Select(x => $"'{x}'"))}";
        }


    }
}