using WebExpress.WebIndex.Studio.Model;

namespace WebExpress.WebIndex.Studio.Views;

/// <summary>
/// A view that displays a list of objects.
/// </summary>
public partial class ObjectListView : ContentView
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectListView"/> class.
    /// </summary>
    /// <param name="objectType">The object type to be displayed.</param>
    public ObjectListView(ObjectType objectType)
    {
        InitializeComponent();

        BindingContext = objectType;

        ContentArea.Content = new ObjectDetailView(objectType, objectType?.Data?.FirstOrDefault());
    }

    /// <summary>
    /// Handles the event when an item is selected from the list.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Event arguments.</param>
    private void OnSelectItem(object sender, SelectedItemChangedEventArgs e)
    {
        ContentArea.Content = new ObjectDetailView(BindingContext as ObjectType, e.SelectedItem);
    }
}