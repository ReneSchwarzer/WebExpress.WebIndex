namespace WebExpress.WebIndex.Studio.Views;

public partial class ObjectDetailsAttributes : ContentView
{
    public ObjectDetailsAttributes(Model.Object @object)
    {
        InitializeComponent();

        BindingContext = @object;
    }
}