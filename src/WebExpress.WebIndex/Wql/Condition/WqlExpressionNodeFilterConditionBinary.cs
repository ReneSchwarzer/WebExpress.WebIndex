namespace WebExpress.WebIndex.Wql.Condition
{
    /// <summary>
    /// Describes the binary condition expression of a wql statement.
    /// </summary>
    /// <param name="token">One or more tokens that determine the operation. Multiple tokens are separated by spaces.</param>
    public abstract class WqlExpressionNodeFilterConditionBinary<T>(string token) : WqlExpressionNodeFilterCondition<T>(token) where T : IIndexItem
    {
        /// <summary>
        /// Returns the parameter expression.
        /// </summary>
        public WqlExpressionNodeParameter<T> Parameter { get; internal set; }

        /// <summary>
        /// Returns the parameter options expression.
        /// </summary>
        public WqlExpressionNodeParameterOption<T> Options { get; internal set; } = new WqlExpressionNodeParameterOption<T>();

        /// <summary>
        /// Converts the condition expression to a string.
        /// </summary>
        /// <returns>The condition expression as a string.</returns>
        public override string ToString()
        {
            return $"{Attribute} {Operator} {Parameter} {Options}".Trim();
        }
    }
}