namespace WebExpress.WebIndex.Studio.Views;

public partial class ObjectDetailsView : ContentView
{
    public ObjectDetailsView(Model.Object @object)
    {
        InitializeComponent();

        BindingContext = @object;

        Frame.Content = new ObjectDetailsDataView(BindingContext as Model.Object);
    }

    private void OnDataClicked(object sender, EventArgs e)
    {
        Frame.Content = new ObjectDetailsDataView(BindingContext as Model.Object);
    }

    private void OnAttributesClicked(object sender, EventArgs e)
    {
        Frame.Content = new ObjectDetailsAttributes(BindingContext as Model.Object);
    }
    private void OnPropertyClicked(object sender, EventArgs e)
    {
        Frame.Content = new ObjectPropertyView(BindingContext as Model.Object);
    }

}