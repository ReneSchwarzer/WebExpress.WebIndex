namespace WebExpress.WebIndex.Wql.Condition
{
    /// <summary>
    /// Represents the context for a WQL expression node filter condition.
    /// </summary>
    /// <typeparam name="TIndexItem">The type of the index item.</typeparam>
    public class WqlExpressionNodeFilterConditionContext<TIndexItem> : IWqlExpressionNodeFilterConditionContext
        where TIndexItem : IIndexItem
    {
    }
}
