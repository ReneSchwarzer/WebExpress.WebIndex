﻿using WebExpress.WebIndex.Wql.Function;

namespace WebExpress.WebIndex.Wql
{
    /// <summary>
    /// Describes the parameter expression of a wql statement.
    /// </summary>
    public class WqlExpressionNodeParameter<T> : IWqlExpressionNode<T> where T : IIndexItem
    {
        /// <summary>
        /// Returns the value expressions.
        /// </summary>
        public WqlExpressionNodeValue<T> Value { get; internal set; }

        /// <summary>
        /// Returns the function expressions.
        /// </summary>
        public WqlExpressionNodeFilterFunction<T> Function { get; internal set; }

        /// <summary>
        /// Constructor
        /// </summary>
        internal WqlExpressionNodeParameter()
        {
        }

        /// <summary>
        /// Returns the value.
        /// </summary>
        /// <returns>The value.</returns>
        public object GetValue()
        {
            return Function != null ? Function.Execute() : Value.GetValue();
        }

        /// <summary>
        /// Converts the parameter expression to a string.
        /// </summary>
        /// <returns>The parameter expression as a string.</returns>
        public override string ToString()
        {
            return Value != null ? Value.ToString() : Function.ToString().Trim();
        }
    }
}