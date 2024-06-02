namespace WebExpress.WebIndex.Studio.Views;

public partial class ObjectDetailsDataPropertyView : ContentView
{
    public ObjectDetailsDataPropertyView(Model.Object @object, object item)
    {
        InitializeComponent();

        BindingContext = item;

        var stackLayout = new StackLayout();

        var idLlabel = new Label { Text = $"Id:", Margin = new Thickness(2) };
        var idValue = new Label { Margin = new Thickness(8) };
        idValue.SetBinding(Label.TextProperty, "Id");

        stackLayout.Children.Add(idLlabel);
        stackLayout.Children.Add(idValue);

        foreach (var attribute in @object?.Attributes ?? [])
        {

            var label = new Label { Text = $"{attribute.Name}:", Margin = new Thickness(2) };
            var entry = new Entry { Placeholder = "enter value", Margin = new Thickness(8) };
            entry.SetBinding(Entry.TextProperty, attribute.Name);

            stackLayout.Children.Add(label);
            stackLayout.Children.Add(entry);

        }

        Content = stackLayout;
    }
}