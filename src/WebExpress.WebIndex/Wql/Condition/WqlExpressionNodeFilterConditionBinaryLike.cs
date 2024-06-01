using System;
using System.Linq;

namespace WebExpress.WebIndex.Wql.Condition
{
    public class WqlExpressionNodeFilterConditionBinaryLike<T> : WqlExpressionNodeFilterConditionBinary<T> where T : IIndexItem
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="op">The operator.</param>
        public WqlExpressionNodeFilterConditionBinaryLike()
            : base("~")
        {
        }

        /// <summary>
        /// Applies the filter to the index.
        /// </summary>
        /// <returns>The data ids from the index.</returns>
        public override IQueryable<Guid> Apply()
        {
            var value = Parameter.GetValue();

            return Attribute.ReverseIndex?.Retrieve(value.ToString(), new IndexRetrieveOptions()
            {
                Method = IndexRetrieveMethod.Default,
                Distance = Options.Distance.HasValue ? Options.Distance.Value : 0
            }).AsQueryable();
        }

        /// <summary>
        /// Applies the filter to the unfiltered data object.
        /// </summary>
        /// <param name="unfiltered">The unfiltered data.</param>
        /// <returns>The filtered data.</returns>
        public override IQueryable<T> Apply(IQueryable<T> unfiltered)
        {
            var property = Attribute.Property;
            var value = Parameter.GetValue().ToString();
            var filtered = unfiltered.Where
            (
                x => property.GetValue(x).ToString().Contains
                (
                    value,
                    System.StringComparison.OrdinalIgnoreCase
                )
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

            return $"{property.Name} like '%{value}%'";
        }
    }
}
