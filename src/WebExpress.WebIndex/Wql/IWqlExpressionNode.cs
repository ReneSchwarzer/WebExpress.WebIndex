
namespace WebExpress.WebIndex.Wql
{
    /// <summary>
    /// Interface of a WQL expression node.
    /// </summary>
    /// <typeparam name="TIndexItem">The type of the index item.</typeparam>
    public interface IWqlExpressionNode<TIndexItem>
        where TIndexItem : IIndexItem
    {

    }
}