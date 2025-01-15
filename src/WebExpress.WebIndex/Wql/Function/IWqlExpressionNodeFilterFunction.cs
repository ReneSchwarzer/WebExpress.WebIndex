namespace WebExpress.WebIndex.Wql.Function
{
    /// <summary>
    /// Interface for a WQL expression node filter function.
    /// </summary>
    public interface IWqlExpressionNodeFilterFunction
    {
        /// <summary>
        /// Returns the function name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Executes the function.
        /// </summary>
        /// <returns>The return value.</returns>
        object Execute();
    }

    /// <summary>
    /// Interface for a WQL expression node filter function.
    /// </summary>
    /// <typeparam name="TIndexItem">The type of the index item.</typeparam>
    public interface IWqlExpressionNodeFilterFunction<TIndexItem> : IWqlExpressionNodeFilterFunction, IWqlExpressionNode<TIndexItem>
        where TIndexItem : IIndexItem
    {
    }
}
