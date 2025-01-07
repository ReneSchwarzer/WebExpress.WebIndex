﻿using System.Linq;

namespace WebExpress.WebIndex.Wql
{
    /// <summary>
    /// Describes the order attribute of a wql statement.
    /// </summary>
    public class WqlExpressionNodeOrderAttribute<T> : IWqlExpressionNode<T> where T : IIndexItem
    {
        /// <summary>
        /// Returns the attribute expressions.
        /// </summary>
        public WqlExpressionNodeAttribute<T> Attribute { get; internal set; }

        /// <summary>
        /// Returns the descending expressions.
        /// </summary>
        public bool Descending { get; internal set; }

        /// <summary>
        /// Returns the position of the attbibute within the order by statement.
        /// </summary>
        public int Position { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        internal WqlExpressionNodeOrderAttribute()
        {
        }

        /// <summary>
        /// Applies the filter to the unfiltered data object.
        /// </summary>
        /// <param name="unfiltered">The unfiltered data.</param>
        /// <returns>The filtered data.</returns>
        public IQueryable<T> Apply(IQueryable<T> unfiltered)
        {
            var property = Attribute.Property;

            if (Position > 0 && unfiltered is IOrderedQueryable<T> orderedQueryable)
            {
                if (Descending)
                {
                    return orderedQueryable.ThenByDescending(x => property.GetValue(x));
                }
                else
                {
                    return orderedQueryable.ThenBy(x => property.GetValue(x));
                }
            }
            else
            {
                if (Descending)
                {
                    return unfiltered.OrderByDescending(x => property.GetValue(x));
                }
                else
                {
                    return unfiltered.OrderBy(x => property.GetValue(x));
                }
            }
        }

        /// <summary>
        /// Converts the order expression to a string.
        /// </summary>
        /// <returns>The order expression as a string.</returns>
        public override string ToString()
        {
            return string.Format("{0} {1}", Attribute.ToString(), Descending ? "desc" : "asc").Trim();
        }
    }
}