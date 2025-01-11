using WebExpress.WebIndex.WiUI.Model;

namespace WebExpress.WebIndex.WiUI.Pages;

/// <summary>
/// Represents the project page in the application.
/// </summary>
public partial class ProjectPage : ContentPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectPage"/> class.
    /// </summary>
    public ProjectPage()
    {
        InitializeComponent();

        BindingContext = new ProjectViewModel();
    }
}