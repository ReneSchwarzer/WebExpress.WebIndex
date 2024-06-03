/// <summary>
/// Represents a control item for a tab interface.
/// </summary>
public class TabControlItem
{
    /// <summary>
    /// Returns or sets the name of the tab control item.
    /// </summary>
    /// <value>The name of the tab control item.</value>
    public string Name { get; set; }

    /// <summary>
    /// Returns or sets the content view of the tab control item.
    /// </summary>
    /// <value>The content view displayed within the tab.</value>
    public View Content { get; set; }
}