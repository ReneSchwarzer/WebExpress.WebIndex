using WebExpress.WebIndex.Studio.Model;
using WebExpress.WebIndex.Studio.Views;

namespace WebExpress.WebIndex.Studio.Pages;

[QueryProperty(nameof(ProjectId), "id")]
public partial class ObjectTypePage : ContentPage
{
    /// <summary>
    /// Define a property to store the value of the query parameter
    /// </summary>
    public string ProjectId { set; get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectTypePage"/> class.
    /// </summary>
    public ObjectTypePage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Called when the page is appearing.
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();

        BindingContext = App.ViewModel.GetProject(ProjectId);
    }

    /// <summary>
    /// Handles the event when the add button is clicked.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Event data.</param>
    private void OnAddClicked(object sender, EventArgs e)
    {
        var newObjectType = new ObjectType() { Name = $"Object_{(BindingContext as Project).Objects.Count + 1}" };
        (BindingContext as Project).Objects.Add(newObjectType);

        ObjectList.SelectedItem = newObjectType;
        ContentArea.Content = new ObjectTypeDetailView(newObjectType, "config");
    }

    /// <summary>
    /// Handles the event when the edit button is clicked.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Event data.</param>
    private void OnEditClicked(object sender, EventArgs e)
    {
        ContentArea.Content = new ObjectTypeDetailView(ObjectList.SelectedItem as ObjectType, "config");
    }

    /// <summary>
    /// Handles the event when the delete button is clicked.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Event data.</param>
    private void OnDeleteClicked(object sender, EventArgs e)
    {
        var obj = ObjectList.SelectedItem as ObjectType;

        (BindingContext as Project).RemoveObject(obj);

        ObjectList.SelectedItem = (BindingContext as Project).Objects.FirstOrDefault();
    }

    /// <summary>
    /// Handles the event when an object is selected from the list.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Event data containing the selected item.</param>
    private void OnSelectObjectClicked(object sender, SelectedItemChangedEventArgs e)
    {
        // displays the details view of the selected object in the content area
        ContentArea.Content = new ObjectTypeDetailView(e.SelectedItem as ObjectType);
    }
}