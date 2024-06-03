using WebExpress.WebIndex.Studio.Model;

namespace WebExpress.WebIndex.Studio.Views;

public partial class ObjectTypeAttributeView : ContentView
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectListView"/> class.
    /// </summary>
    /// <param name="objectType">The object type to be displayed.</param>
    public ObjectTypeAttributeView(ObjectType objectType)
    {
        InitializeComponent();

        BindingContext = objectType;
    }
}