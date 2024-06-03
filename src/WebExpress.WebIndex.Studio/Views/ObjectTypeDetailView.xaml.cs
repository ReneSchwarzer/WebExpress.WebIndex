using WebExpress.WebIndex.Studio.Model;

namespace WebExpress.WebIndex.Studio.Views;

/// <summary>
/// Represents a view for different object types within the WebExpress.WebIndex.Studio.
/// </summary>
public partial class ObjectTypeDetailView : ContentView
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectTypeDetailView"/> class.
    /// </summary>
    /// <param name="objectType">The object type to be displayed.</param>
    /// <param name="target">The specific view to be displayed, can be property, attribute, or data view.</param>
    public ObjectTypeDetailView(ObjectType objectType, string target = "")
    {
        InitializeComponent();

        BindingContext = objectType;

        ContentArea.Content = target?.ToLower() switch
        {
            "config" => new ObjectTypeConfigView(BindingContext as ObjectType),
            "attribute" => new ObjectTypeAttributeView(BindingContext as ObjectType),
            _ => new ObjectListView(BindingContext as ObjectType)
        };
    }

    /// <summary>
    /// Event handler for when the data view is clicked.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Event arguments.</param>
    private void OnDataClicked(object sender, EventArgs e)
    {
        ContentArea.Content = new ObjectListView(BindingContext as ObjectType);
    }

    /// <summary>
    /// Event handler for when the attributes view is clicked.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Event arguments.</param>
    private void OnAttributesClicked(object sender, EventArgs e)
    {
        ContentArea.Content = new ObjectTypeAttributeView(BindingContext as ObjectType);
    }

    /// <summary>
    /// Event handler for when the property view is clicked.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Event arguments.</param>
    private void OnPropertyClicked(object sender, EventArgs e)
    {
        ContentArea.Content = new ObjectTypeConfigView(BindingContext as ObjectType);
    }

}