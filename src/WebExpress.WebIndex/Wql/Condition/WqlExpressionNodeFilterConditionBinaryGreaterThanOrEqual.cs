using System;
using System.Collections;
using System.Linq;

namespace WebExpress.WebIndex.Wql.Condition
{
    /// <summary>
    /// Represents a filter condition that checks if a property value is greater than or equal to a specified value.
    /// </summary>
    /// <typeparam name="TIndexItem">The type of the index item.</typeparam>
    public class WqlExpressionNodeFilterConditionBinaryGreaterThanOrEqual<TIndexItem> : WqlExpressionNodeFilterConditionBinary<TIndexItem>
        where TIndexItem : IIndexItem
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="op">The operator.</param>
        public WqlExpressionNodeFilterConditionBinaryGreaterThanOrEqual()
            : base(">=")
        {

        }

        /// <summary>
        /// Applies the filter to the index.
        /// </summary>
        /// <returns>The data ids from the index.</returns>
        public override IQueryable<Guid> Apply()
        {
            var property = Attribute?.Property;
            var value = Parameter.GetValue();

            //var filtered = unfiltered.Where
            //(
            //    x => property != null && property.GetValue(x).Equals(value)
            //);

            return null; //filtered.AsQueryable();
        }

        /// <summary>
        /// Applies the filter to the unfiltered data object.
        /// </summary>
        /// <param name="unfiltered">The unfiltered data.</param>
        /// <returns>The filtered data.</returns>
        public override IQueryable<TIndexItem> Apply(IQueryable<TIndexItem> unfiltered)
        {
            var comparer = new Comparer(Culture);
            var property = Attribute.Property;
            var value = Parameter.GetValue();
            var filtered = unfiltered.Where
            (
                x => comparer.Compare(property.GetValue(x), value) >= 0
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
            var value = Parameter.Value;

            return $"{property.Name} like '{value}'";
        }
    }
}
