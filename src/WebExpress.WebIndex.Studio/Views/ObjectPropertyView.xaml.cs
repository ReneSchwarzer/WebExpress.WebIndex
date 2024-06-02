namespace WebExpress.WebIndex.Studio.Views;

public partial class ObjectPropertyView : ContentView
{
    public ObjectPropertyView(Model.Object @object)
    {
        InitializeComponent();

        BindingContext = @object;
    }
}