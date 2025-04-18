using System.Collections.Generic;

namespace WebExpress.WebIndex.Wql.Condition
{
    /// <summary>
    /// Represents s filter conditions for sets in a WQL expression node.
    /// </summary>
    /// <typeparam name="TIndexItem">The type of the index item.</typeparam>
    public abstract class WqlExpressionNodeFilterConditionSet<TIndexItem> : WqlExpressionNodeFilterCondition<TIndexItem>
        where TIndexItem : IIndexItem
    {
        /// <summary>
        /// Returns the parameter expressions.
        /// </summary>
        public IEnumerable<WqlExpressionNodeParameter<TIndexItem>> Parameters { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="token">One or more tokens that determine the operation. Multiple tokens are separated by spaces.</param>
        protected WqlExpressionNodeFilterConditionSet(string token)
            : base(token)
        {
        }

        /// <summary>
        /// Converts the condition expression to a string.
        /// </summary>
        /// <returns>The condition expression as a string.</returns>
        public override string ToString()
        {
            return string.Format("{0} {1} ({2})", Attribute.ToString(), Operator.ToLower(), string.Join(", ", Parameters)).Trim();
        }
    }
}