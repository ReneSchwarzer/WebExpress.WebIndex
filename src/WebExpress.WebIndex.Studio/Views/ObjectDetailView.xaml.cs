using WebExpress.WebIndex.Studio.Model;

namespace WebExpress.WebIndex.Studio.Views;

public partial class ObjectDetailView : ContentView
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectDetailsView"/> class.
    /// </summary>
    /// <param name="objectType">The object type to be displayed.</param>
    /// <param name="item">The item to be displayed.</param>
    public ObjectDetailView(ObjectType objectType, object item)
    {
        InitializeComponent();

        BindingContext = item;

        var stackLayout = new StackLayout();

        var idLlabel = new Label { Text = $"Id:", Margin = new Thickness(2) };
        var idValue = new Label { Margin = new Thickness(8) };
        idValue.SetBinding(Label.TextProperty, "Id");

        stackLayout.Children.Add(idLlabel);
        stackLayout.Children.Add(idValue);

        foreach (var attribute in objectType?.Attributes ?? [])
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