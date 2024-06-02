namespace WebExpress.WebIndex.Studio.Views;

public partial class ObjectDetailsDataView : ContentView
{
    public ObjectDetailsDataView(Model.Object @object)
    {
        InitializeComponent();

        BindingContext = @object;

        Frame.Content = new ObjectDetailsDataPropertyView(@object, @object?.Data?.FirstOrDefault());
    }

    private void OnSelectItem(object sender, SelectedItemChangedEventArgs e)
    {
        Frame.Content = new ObjectDetailsDataPropertyView(BindingContext as Model.Object, e.SelectedItem);
    }
}