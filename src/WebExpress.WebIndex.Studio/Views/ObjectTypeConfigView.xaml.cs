using WebExpress.WebIndex.Studio.Model;

namespace WebExpress.WebIndex.Studio.Views;

public partial class ObjectTypeConfigView : ContentView
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectTypeConfigView"/> class.
    /// </summary>
    /// <param name="objectType">The object type to be displayed.</param>
    public ObjectTypeConfigView(ObjectType objectType)
    {
        InitializeComponent();

        BindingContext = objectType;
    }
}